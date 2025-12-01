CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Actions are data-driven and tied to from→to status transitions
CREATE TABLE IF NOT EXISTS incident_status_transitions (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  from_status_id UUID NOT NULL REFERENCES incident_statuses(id) ON DELETE CASCADE,
  to_status_id   UUID NOT NULL REFERENCES incident_statuses(id) ON DELETE RESTRICT,
  action_code    TEXT NOT NULL,     -- e.g., 'send_to_review', 'accept', 'reject'
  initiator      TEXT NOT NULL,     -- 'creator' or 'officer'
  is_active      BOOLEAN NOT NULL DEFAULT TRUE,
  UNIQUE (from_status_id, to_status_id, action_code)
);

-- Seed transitions (use the same UUIDs you seeded for statuses)
-- DRAFT:     '00000000-0000-0000-0000-00000000d001'
-- IN_REVIEW: '00000000-0000-0000-0000-00000000r002'
-- ACCEPTED:  '00000000-0000-0000-0000-00000000a003'
-- REJECTED:  '00000000-0000-0000-0000-00000000j004'
INSERT INTO incident_status_transitions (from_status_id, to_status_id, action_code, initiator) VALUES
  ('00000000-0000-0000-0000-00000000d001','00000000-0000-0000-0000-00000000r002','send_to_review','creator'),
  ('00000000-0000-0000-0000-00000000r002','00000000-0000-0000-0000-00000000a003','accept','officer'),
  ('00000000-0000-0000-0000-00000000r002','00000000-0000-0000-0000-00000000j004','reject','officer'),
  ('00000000-0000-0000-0000-00000000j004','00000000-0000-0000-0000-00000000r002','send_to_review','creator')
ON CONFLICT DO NOTHING;

CREATE INDEX IF NOT EXISTS idx_inc_st_trans_from ON incident_status_transitions(from_status_id);
CREATE INDEX IF NOT EXISTS idx_inc_st_trans_action ON incident_status_transitions(action_code);