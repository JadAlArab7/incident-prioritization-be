# Incident Status Flow Documentation

## Overview

This document describes the data-driven incident status workflow system. The workflow manages incident lifecycle transitions through a configurable state machine stored in the database.

## Status Flow Diagram

```
┌─────────┐    send_to_review    ┌───────────┐
│  DRAFT  │ ──────────────────► │ IN_REVIEW │
└─────────┘                      └───────────┘
     ▲                                │
     │                          ┌─────┴─────┐
     │                          │           │
     │                       accept      reject
     │                          │           │
     │                          ▼           ▼
     │                    ┌──────────┐ ┌──────────┐
     │                    │ ACCEPTED │ │ REJECTED │
     │                    └──────────┘ └──────────┘
     │                                       │
     └───────────── send_to_review ──────────┘
```

## Status Codes

| Status Code | Description | Terminal |
|-------------|-------------|----------|
| `draft` | Initial status when incident is created | No |
| `in_review` | Incident is being reviewed by an officer | No |
| `accepted` | Incident has been accepted by the officer | Yes |
| `rejected` | Incident has been rejected by the officer | No |

## Actions

| Action Code | From Status | To Status | Initiator | Description |
|-------------|-------------|-----------|-----------|-------------|
| `send_to_review` | draft | in_review | creator | Creator sends incident for review |
| `send_to_review` | rejected | in_review | creator | Creator resubmits after rejection |
| `accept` | in_review | accepted | officer | Officer accepts the incident |
| `reject` | in_review | rejected | officer | Officer rejects the incident |

## API Endpoints

### 1. Update Incident Status

**Endpoint:** `PATCH /api/incidents/{id}/status`

**Authentication:** Required (JWT Bearer token)

**Request Body:**
```json
{
  "action": "send_to_review | accept | reject",
  "comment": "Optional comment for the status change",
  "newSentToUserId": "uuid (required for send_to_review if not already assigned)"
}
```

**Response (200 OK):**
```json
{
  "incident": {
    "id": "uuid",
    "title": "string",
    "description": "string",
    "incidentTypeId": "uuid",
    "incidentTypeName": "string",
    "statusId": "uuid",
    "statusName": "string",
    "locationId": "uuid",
    "location": { ... },
    "createdByUserId": "uuid",
    "createdByUserName": "string",
    "sentToUserId": "uuid",
    "sentToUserName": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  },
  "nextActions": ["accept", "reject"],
  "canSendToReview": false,
  "canAccept": true,
  "canReject": true,
  "canEdit": false
}
```

**Error Responses:**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid action, missing required fields, or transition not allowed |
| 403 Forbidden | User is not authorized to perform this action |
| 404 Not Found | Incident not found |
| 409 Conflict | Concurrent modification detected |

### 2. Get Incident with Action Flags

**Endpoint:** `GET /api/incidents/{id}/details`

**Authentication:** Required (JWT Bearer token)

**Response (200 OK):**
```json
{
  "id": "uuid",
  "title": "string",
  "description": "string",
  "incidentTypeId": "uuid",
  "incidentTypeName": "string",
  "statusId": "uuid",
  "statusCode": "draft",
  "statusName": "Draft",
  "locationId": "uuid",
  "location": { ... },
  "createdByUserId": "uuid",
  "createdByUserName": "string",
  "sentToUserId": "uuid",
  "sentToUserName": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "nextActions": ["send_to_review"],
  "canSendToReview": true,
  "canAccept": false,
  "canReject": false,
  "canEdit": true
}
```

### 3. Get Action Flags Only

**Endpoint:** `GET /api/incidents/{id}/actions`

**Authentication:** Required (JWT Bearer token)

**Response (200 OK):**
```json
{
  "nextActions": ["send_to_review"],
  "canSendToReview": true,
  "canAccept": false,
  "canReject": false,
  "canEdit": true
}
```

## Action Flags Explained

| Flag | Description |
|------|-------------|
| `canSendToReview` | User can send the incident for review |
| `canAccept` | User can accept the incident |
| `canReject` | User can reject the incident |
| `canEdit` | User can edit the incident details |
| `nextActions` | List of all action codes available to the current user |

## Authorization Rules

### Creator Actions
- **send_to_review**: Only the incident creator can perform this action
- **canEdit**: Creator can edit while status is `draft` or `rejected`

### Officer Actions
- **accept/reject**: Only the assigned officer (sent_to_user_id) can perform these actions
- User must have the `officer` role

## Database Schema

### incident_status_transitions
Stores the allowed transitions between statuses.

```sql
CREATE TABLE incident_status_transitions (
  id UUID PRIMARY KEY,
  from_status_id UUID NOT NULL REFERENCES incident_statuses(id),
  to_status_id UUID NOT NULL REFERENCES incident_statuses(id),
  action_code TEXT NOT NULL,
  initiator TEXT NOT NULL,  -- 'creator' or 'officer'
  is_active BOOLEAN NOT NULL DEFAULT TRUE
);
```

### incident_status_history
Audit trail of all status changes.

```sql
CREATE TABLE incident_status_history (
  id UUID PRIMARY KEY,
  incident_id UUID NOT NULL REFERENCES incidents(id),
  from_status_id UUID NOT NULL REFERENCES incident_statuses(id),
  to_status_id UUID NOT NULL REFERENCES incident_statuses(id),
  changed_by_user_id UUID NOT NULL REFERENCES users(id),
  comment TEXT,
  changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

## Usage Examples

### Example 1: Creator sends incident for review

```bash
curl -X PATCH "https://api.example.com/api/incidents/{incident-id}/status" \
  -H "Authorization: Bearer {jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "action": "send_to_review",
    "comment": "Please review this incident",
    "newSentToUserId": "officer-user-id"
  }'
```

### Example 2: Officer accepts the incident

```bash
curl -X PATCH "https://api.example.com/api/incidents/{incident-id}/status" \
  -H "Authorization: Bearer {jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "action": "accept",
    "comment": "Incident verified and accepted"
  }'
```

### Example 3: Officer rejects the incident

```bash
curl -X PATCH "https://api.example.com/api/incidents/{incident-id}/status" \
  -H "Authorization: Bearer {jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "action": "reject",
    "comment": "Missing required information"
  }'
```

### Example 4: Get incident with action flags

```bash
curl -X GET "https://api.example.com/api/incidents/{incident-id}/details" \
  -H "Authorization: Bearer {jwt-token}"
```

## Frontend Integration

The frontend should use the action flags to conditionally render buttons:

```javascript
// Example React component
function IncidentActions({ incident }) {
  return (
    <div>
      {incident.canEdit && (
        <button onClick={handleEdit}>Edit</button>
      )}
      {incident.canSendToReview && (
        <button onClick={handleSendToReview}>Send to Review</button>
      )}
      {incident.canAccept && (
        <button onClick={handleAccept}>Accept</button>
      )}
      {incident.canReject && (
        <button onClick={handleReject}>Reject</button>
      )}
    </div>
  );
}
```

## Database Setup

Run the following SQL scripts in order:

1. `sql/205_create_incident_transitions.sql` - Creates transitions table and seeds data
2. `sql/206_create_incident_status_history.sql` - Creates history table
3. `sql/207_triggers_incident_flow.sql` - Creates validation trigger

## Extending the Workflow

To add new transitions:

1. Insert a new row into `incident_status_transitions`:
```sql
INSERT INTO incident_status_transitions (from_status_id, to_status_id, action_code, initiator)
VALUES ('from-status-uuid', 'to-status-uuid', 'new_action', 'creator');
```

2. The API will automatically recognize the new transition without code changes.

## Error Handling

The API returns structured error responses:

```json
{
  "error": "Action 'accept' is not allowed from status 'draft'"
}
```

Common error scenarios:
- Attempting an action not allowed from current status
- User not authorized (not creator/officer)
- Missing `newSentToUserId` when sending to review
- Target user doesn't have officer role