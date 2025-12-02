# SignalR Real-Time Notification System - Complete Documentation

## Overview
A complete real-time notification system using SignalR for .NET 8 with JWT authentication, PostgreSQL database, and three-layer architecture.

---

## 📋 Database Schema

### Tables Created

#### 1. `incident.notification_types`
Lookup table for notification categories.

```sql
CREATE TABLE incident.notification_types (
  id UUID PRIMARY KEY,
  code TEXT NOT NULL UNIQUE,
  name TEXT NOT NULL,
  name_ar TEXT NOT NULL,
  description TEXT
);
```

**Pre-seeded Types:**
- `incident_assigned` - When incident assigned to user
- `incident_status_changed` - When incident status changes
- `incident_created` - When new incident is created
- `system_notification` - General notifications

#### 2. `incident.notifications`
Main notifications table linked to users.

```sql
CREATE TABLE incident.notifications (
  id UUID PRIMARY KEY,
  user_id UUID REFERENCES incident.users(id),
  notification_type_id UUID REFERENCES incident.notification_types(id),
  title TEXT NOT NULL,
  message TEXT NOT NULL,
  related_entity_type TEXT,
  related_entity_id UUID,
  is_read BOOLEAN DEFAULT FALSE,
  read_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  metadata JSONB
);
```

**Indexes:**
- `idx_notifications_user_id` - For user queries
- `idx_notifications_user_unread` - For unread notifications
- `idx_notifications_created_at` - For sorting

---

## 🏗️ Architecture

### Three-Layer Implementation

```
┌─────────────────────────────────────┐
│         Controllers Layer           │
│   NotificationsController.cs        │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│          Services Layer             │
│   NotificationService.cs            │
│   + SignalR Hub Context             │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│        Repository Layer             │
│   NotificationRepository.cs         │
│   + Database Access                 │
└─────────────────────────────────────┘
```

---

## 🔌 SignalR Hub

### Hub Implementation

```csharp
[Authorize]
public class NotificationHub : Hub
{
    // Automatically manages user connections
    // Users join their personal group: "user_{userId}"
    
    public static string GetUserGroupName(Guid userId) 
        => $"user_{userId}";
}
```

**Hub URL:** `https://your-api/hubs/notifications`

---

## 📡 SignalR Events (Client-Side)

### Events Sent from Server to Client

#### 1. **ReceiveNotification**
Sent when a new notification is created for the user.

```typescript
connection.on("ReceiveNotification", (notification: NotificationDto) => {
  console.log("New notification:", notification);
  // Update UI, show toast, play sound, etc.
});
```

**Payload:**
```json
{
  "id": "uuid",
  "userId": "uuid",
  "title": "Incident Status Updated",
  "message": "Your incident 'Fire at Building A' status has been changed to In Review",
  "typeCode": "incident_status_changed",
  "typeName": "Incident Status Changed",
  "relatedEntityType": "incident",
  "relatedEntityId": "uuid",
  "isRead": false,
  "readAt": null,
  "createdAt": "2025-12-01T10:30:00Z"
}
```

#### 2. **NotificationRead**
Sent when a single notification is marked as read.

```typescript
connection.on("NotificationRead", (data) => {
  console.log("Notification marked as read:", data.NotificationId);
  // Update UI to reflect read status
});
```

**Payload:**
```json
{
  "notificationId": "uuid"
}
```

#### 3. **AllNotificationsRead**
Sent when all notifications are marked as read.

```typescript
connection.on("AllNotificationsRead", () => {
  console.log("All notifications marked as read");
  // Update UI
});
```

#### 4. **UnreadCountUpdated**
Sent whenever the unread count changes.

```typescript
connection.on("UnreadCountUpdated", (data) => {
  console.log("Unread count:", data.UnreadCount);
  // Update badge/counter in UI
});
```

**Payload:**
```json
{
  "unreadCount": 5
}
```

#### 5. **NotificationDeleted**
Sent when a notification is deleted.

```typescript
connection.on("NotificationDeleted", (data) => {
  console.log("Notification deleted:", data.NotificationId);
  // Remove from UI
});
```

---

## 🔌 Client Implementation Examples

### React/TypeScript Example

```typescript
import * as signalR from "@microsoft/signalr";

// 1. Create connection with JWT token
const token = localStorage.getItem('authToken');

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://your-api/hubs/notifications", {
    accessTokenFactory: () => token || ""
  })
  .withAutomaticReconnect()
  .build();

// 2. Set up event handlers
connection.on("ReceiveNotification", (notification) => {
  // Show notification
  toast.success(notification.message);
  
  // Update state
  setNotifications(prev => [notification, ...prev]);
  setUnreadCount(prev => prev + 1);
});

connection.on("UnreadCountUpdated", (data) => {
  setUnreadCount(data.unreadCount);
});

connection.on("NotificationRead", (data) => {
  setNotifications(prev =>
    prev.map(n =>
      n.id === data.notificationId
        ? { ...n, isRead: true, readAt: new Date() }
        : n
    )
  );
});

connection.on("AllNotificationsRead", () => {
  setNotifications(prev =>
    prev.map(n => ({ ...n, isRead: true, readAt: new Date() }))
  );
  setUnreadCount(0);
});

// 3. Start connection
async function startConnection() {
  try {
    await connection.start();
    console.log("SignalR Connected");
  } catch (err) {
    console.error("SignalR Connection Error:", err);
    setTimeout(startConnection, 5000); // Retry after 5s
  }
}

startConnection();

// 4. Clean up on unmount
return () => {
  connection.stop();
};
```

### Angular Example

```typescript
import * as signalR from '@microsoft/signalr';

export class NotificationService {
  private hubConnection!: signalR.HubConnection;
  
  public notifications$ = new BehaviorSubject<Notification[]>([]);
  public unreadCount$ = new BehaviorSubject<number>(0);

  constructor(private authService: AuthService) {}

  public startConnection(): void {
    const token = this.authService.getToken();
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://your-api/hubs/notifications', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));

    this.hubConnection.on('ReceiveNotification', (notification) => {
      const current = this.notifications$.value;
      this.notifications$.next([notification, ...current]);
    });

    this.hubConnection.on('UnreadCountUpdated', (data) => {
      this.unreadCount$.next(data.unreadCount);
    });
  }

  public stopConnection(): void {
    this.hubConnection.stop();
  }
}
```

### Vue.js Example

```javascript
import * as signalR from '@microsoft/signalr';

export default {
  data() {
    return {
      connection: null,
      notifications: [],
      unreadCount: 0
    }
  },
  
  async mounted() {
    const token = localStorage.getItem('authToken');
    
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("https://your-api/hubs/notifications", {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on("ReceiveNotification", (notification) => {
      this.notifications.unshift(notification);
      this.unreadCount++;
      this.$toast.info(notification.message);
    });

    this.connection.on("UnreadCountUpdated", (data) => {
      this.unreadCount = data.unreadCount;
    });

    try {
      await this.connection.start();
      console.log("SignalR Connected");
    } catch (err) {
      console.error(err);
    }
  },
  
  beforeUnmount() {
    if (this.connection) {
      this.connection.stop();
    }
  }
}
```

---

## 🔐 REST API Endpoints

### 1. Get User Notifications

```http
GET /api/notifications?unreadOnly=false&page=1&pageSize=20
Authorization: Bearer {token}
```

**Response:**
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 20,
  "totalCount": 45
}
```

### 2. Get Notification Stats

```http
GET /api/notifications/stats
Authorization: Bearer {token}
```

**Response:**
```json
{
  "totalCount": 45,
  "unreadCount": 12
}
```

### 3. Mark Single Notification as Read

```http
POST /api/notifications/{id}/read
Authorization: Bearer {token}
```

**Triggers:** `NotificationRead` and `UnreadCountUpdated` SignalR events

### 4. Mark All as Read

```http
POST /api/notifications/read-all
Authorization: Bearer {token}
```

**Triggers:** `AllNotificationsRead` and `UnreadCountUpdated` SignalR events

### 5. Delete Notification

```http
DELETE /api/notifications/{id}
Authorization: Bearer {token}
```

**Triggers:** `NotificationDeleted` and `UnreadCountUpdated` SignalR events

### 6. Send Test Notification

```http
POST /api/notifications/test
Authorization: Bearer {token}
```

Sends a test notification to yourself.

---

## 💡 Usage Examples

### Sending a Notification (Backend)

```csharp
// Inject INotificationService
private readonly INotificationService _notificationService;

// Send notification
await _notificationService.CreateAndSendNotificationAsync(
    new CreateNotificationDto
    {
        UserId = targetUserId,
        NotificationTypeCode = "incident_assigned",
        Title = "New Incident Assigned",
        Message = $"Incident '{incidentTitle}' has been assigned to you",
        RelatedEntityType = "incident",
        RelatedEntityId = incidentId
    }, 
    cancellationToken
);
```

**This automatically:**
1. Creates notification in database
2. Sends `ReceiveNotification` event via SignalR to the user
3. Sends updated `UnreadCountUpdated` to the user

---

## 🔄 Complete Flow Example

### Scenario: Incident Status Change

1. **User A** changes incident status to "In Review"
2. **Backend** (IncidentService):
   ```csharp
   await _notificationService.CreateAndSendNotificationAsync(new CreateNotificationDto
   {
       UserId = incidentCreatorId,
       NotificationTypeCode = "incident_status_changed",
       Title = "Incident Status Updated",
       Message = "Your incident status changed to In Review"
   });
   ```

3. **SignalR** sends event to **User B** (creator):
   - `ReceiveNotification` event → Browser shows notification
   - `UnreadCountUpdated` event → Badge shows "1"

4. **User B** clicks notification in UI
5. **Frontend** calls REST API:
   ```javascript
   await fetch('/api/notifications/{id}/read', { method: 'POST' });
   ```

6. **Backend** marks as read and sends SignalR events:
   - `NotificationRead` → UI marks notification as read
   - `UnreadCountUpdated` → Badge shows "0"

---

## 🛠️ Installation & Setup

### 1. Run SQL Migration

```bash
psql -U postgres -d your_database -f sql/301_create_notifications_table.sql
```

### 2. Install Client Package

```bash
npm install @microsoft/signalr
```

### 3. Update CORS (if needed)

In `appsettings.json`, ensure your frontend URL is allowed:

```json
"AllowedOrigins": ["http://localhost:3000", "https://your-frontend.com"]
```

---

## 📊 Performance Considerations

- **Connection Pooling**: SignalR maintains persistent connections
- **Scalability**: Use Azure SignalR Service or Redis backplane for multiple servers
- **Database Indexes**: Already optimized for user queries
- **Pagination**: All list endpoints support paging
- **Cleanup**: Consider archiving old notifications periodically

---

## 🐛 Troubleshooting

### SignalR Not Connecting

1. **Check CORS**: Ensure `AllowCredentials()` is set
2. **Check Token**: Verify JWT token is passed in query string
3. **Check URL**: Hub URL must be `/hubs/notifications`
4. **Check Console**: Look for WebSocket errors

### Notifications Not Received

1. **Verify user is connected**: Check SignalR logs
2. **Check user group**: User must be in `user_{userId}` group
3. **Test with test endpoint**: Use `/api/notifications/test`

### Database Issues

1. **Run migration**: Ensure tables are created
2. **Check foreign keys**: Verify user_id exists
3. **Check notification types**: Ensure types are seeded

---

## ✅ Complete!

Your SignalR notification system is now fully operational with:

- ✅ Real-time notifications
- ✅ Read/unread tracking
- ✅ User-specific delivery
- ✅ REST API + WebSocket
- ✅ JWT authentication
- ✅ Three-layer architecture
- ✅ Database persistence

Notifications will automatically be sent when incidents are created, assigned, or status changes!
