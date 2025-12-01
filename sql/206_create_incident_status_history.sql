-- Audit trail of all status changes
CREATE TABLE IF NOT EXISTS incident.incident_status_history (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  incident_id UUID NOT NULL REFERENCES incident.incidents(id) ON DELETE CASCADE,
  from_status_id UUID NOT NULL REFERENCES incident.incident_statuses(id),
  to_status_id   UUID NOT NULL REFERENCES incident.incident_statuses(id),
  changed_by_user_id UUID NOT NULL REFERENCES incident.users(id),
  comment TEXT,
  changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_inc_status_hist_incident ON incident.incident_status_history(incident_id, changed_at DESC);