-- A reusable location row referencing geo lookups
CREATE TABLE IF NOT EXISTS incident.locations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  lat DOUBLE PRECISION NOT NULL,
  lng DOUBLE PRECISION NOT NULL,
  governorate_id UUID REFERENCES incident.governorates(id) ON DELETE SET NULL,
  district_id UUID REFERENCES incident.districts(id) ON DELETE SET NULL,
  town_id UUID REFERENCES incident.towns(id) ON DELETE SET NULL,
  address_text TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Status lookup with stable UUIDs (pre-generated) so we can default 'draft'
CREATE TABLE IF NOT EXISTS incident.incident_statuses (
  id UUID PRIMARY KEY,
  code TEXT NOT NULL UNIQUE,      -- 'draft','in_review','accepted','rejected'
  name TEXT NOT NULL,             -- English label
  name_ar TEXT NOT NULL
);

-- Incident types (many-to-many)
CREATE TABLE IF NOT EXISTS incident.incident_types (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL UNIQUE,
  name_en TEXT NOT NULL,
  name_ar TEXT NOT NULL
);

-- Main incident table
CREATE TABLE IF NOT EXISTS incident.incidents (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  title TEXT NOT NULL,
  description TEXT,
  -- 'sentTo' links to users table
  sent_to_user_id UUID REFERENCES incident.users(id) ON DELETE SET NULL,
  -- creator for ownership filtering
  created_by_user_id UUID NOT NULL REFERENCES incident.users(id) ON DELETE RESTRICT,
  -- single location now; multi-location can be via incident_locations
  location_id UUID REFERENCES incident.locations(id) ON DELETE SET NULL,
  -- priority as free text for now
  priority TEXT,
  suggested_actions_taken TEXT,
  status_id UUID NOT NULL REFERENCES incident.incident_statuses(id) ON DELETE RESTRICT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Trigger to update updated_at
CREATE OR REPLACE FUNCTION incident.set_incident_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END; $$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS incidents_set_updated_at ON incident.incidents;
CREATE TRIGGER incidents_set_updated_at
BEFORE UPDATE ON incident.incidents
FOR EACH ROW EXECUTE FUNCTION incident.set_incident_updated_at();

-- Many-to-many mapping: incident ↔ types
CREATE TABLE IF NOT EXISTS incident.incident_incident_types (
  incident_id UUID NOT NULL REFERENCES incident.incidents(id) ON DELETE CASCADE,
  incident_type_id UUID NOT NULL REFERENCES incident.incident_types(id) ON DELETE RESTRICT,
  PRIMARY KEY (incident_id, incident_type_id)
);

-- Optional attachments table (for future uploadFile)
CREATE TABLE IF NOT EXISTS incident.attachments (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  incident_id UUID NOT NULL REFERENCES incident.incidents(id) ON DELETE CASCADE,
  file_name TEXT NOT NULL,
  content_type TEXT,
  storage_path TEXT,
  uploaded_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- incident_locations for multi-location support later
CREATE TABLE IF NOT EXISTS incident.incident_locations (
  incident_id UUID NOT NULL REFERENCES incident.incidents(id) ON DELETE CASCADE,
  location_id UUID NOT NULL REFERENCES incident.locations(id) ON DELETE CASCADE,
  PRIMARY KEY (incident_id, location_id)
);