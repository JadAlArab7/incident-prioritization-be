CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Audit trail of all status changes
CREATE TABLE IF NOT EXISTS incident_status_history (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  incident_id UUID NOT NULL REFERENCES incidents(id) ON DELETE CASCADE,
  from_status_id UUID NOT NULL REFERENCES incident_statuses(id),
  to_status_id   UUID NOT NULL REFERENCES incident_statuses(id),
  changed_by_user_id UUID NOT NULL REFERENCES users(id),
  comment TEXT,
  changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_inc_status_hist_incident ON incident_status_history(incident_id, changed_at DESC);