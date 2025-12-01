-- Indexes for incident tables

CREATE INDEX IF NOT EXISTS idx_incidents_sent_to ON incident.incidents(sent_to_user_id);
CREATE INDEX IF NOT EXISTS idx_incidents_created_by ON incident.incidents(created_by_user_id);
CREATE INDEX IF NOT EXISTS idx_incidents_status ON incident.incidents(status_id);
CREATE INDEX IF NOT EXISTS idx_incidents_created_at ON incident.incidents(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_locations_geo ON incident.locations(governorate_id, district_id, town_id);
CREATE INDEX IF NOT EXISTS idx_attachments_incident ON incident.attachments(incident_id);
CREATE INDEX IF NOT EXISTS idx_incident_types_mapping ON incident.incident_incident_types(incident_id);