-- Notification types lookup table
CREATE TABLE IF NOT EXISTS incident.notification_types (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  code TEXT NOT NULL UNIQUE,  -- 'incident_assigned', 'status_changed', 'comment_added', etc.
  name TEXT NOT NULL,
  name_ar TEXT NOT NULL,
  description TEXT
);

-- Main notifications table
CREATE TABLE IF NOT EXISTS incident.notifications (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID NOT NULL REFERENCES incident.users(id) ON DELETE CASCADE,
  notification_type_id UUID NOT NULL REFERENCES incident.notification_types(id) ON DELETE RESTRICT,
  title TEXT NOT NULL,
  message TEXT NOT NULL,
  -- Optional reference to related entity (e.g., incident_id)
  related_entity_type TEXT,  -- 'incident', 'user', etc.
  related_entity_id UUID,
  -- Read/Unread status
  is_read BOOLEAN NOT NULL DEFAULT FALSE,
  read_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  -- Optional metadata as JSON
  metadata JSONB
);

-- Index for efficient user queries
CREATE INDEX IF NOT EXISTS idx_notifications_user_id ON incident.notifications(user_id);
CREATE INDEX IF NOT EXISTS idx_notifications_user_unread ON incident.notifications(user_id, is_read) WHERE is_read = FALSE;
CREATE INDEX IF NOT EXISTS idx_notifications_created_at ON incident.notifications(created_at DESC);

-- Seed notification types
INSERT INTO incident.notification_types (id, code, name, name_ar, description) VALUES
  (gen_random_uuid(), 'incident_assigned', 'Incident Assigned', 'تم تعيين الحادث', 'Notification when an incident is assigned to a user'),
  (gen_random_uuid(), 'incident_status_changed', 'Incident Status Changed', 'تم تغيير حالة الحادث', 'Notification when incident status changes'),
  (gen_random_uuid(), 'incident_created', 'Incident Created', 'تم إنشاء حادث', 'Notification when a new incident is created'),
  (gen_random_uuid(), 'system_notification', 'System Notification', 'إشعار النظام', 'General system notification')
ON CONFLICT (code) DO NOTHING;
