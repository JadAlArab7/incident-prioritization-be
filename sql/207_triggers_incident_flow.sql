CREATE OR REPLACE FUNCTION enforce_incident_review_assignee()
RETURNS TRIGGER AS $$
DECLARE target_code TEXT;
BEGIN
  SELECT code INTO target_code FROM incident_statuses WHERE id = NEW.status_id;

  IF target_code = 'in_review' AND NEW.sent_to_user_id IS NULL THEN
    RAISE EXCEPTION 'sent_to_user_id is required when sending to review';
  END IF;

  RETURN NEW;
END; $$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_incident_review_assignee ON incidents;
CREATE TRIGGER trg_incident_review_assignee
BEFORE INSERT OR UPDATE ON incidents
FOR EACH ROW EXECUTE FUNCTION enforce_incident_review_assignee();