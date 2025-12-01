-- Create the incident schema
CREATE SCHEMA IF NOT EXISTS incident;

-- Create roles table in incident schema
CREATE TABLE IF NOT EXISTS incident.roles (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        TEXT NOT NULL UNIQUE CHECK (name IN ('secretary', 'officer', 'supervisor'))
);

-- Create users table in incident schema
CREATE TABLE IF NOT EXISTS incident.users (
    id               UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username         TEXT NOT NULL UNIQUE,
    password_hash    BYTEA NOT NULL,
    password_salt    BYTEA NOT NULL,
    role_id          UUID NOT NULL REFERENCES incident.roles(id) ON DELETE RESTRICT,
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Create updated_at trigger function
CREATE OR REPLACE FUNCTION incident.set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create trigger for users table
DROP TRIGGER IF EXISTS users_set_updated_at ON incident.users;
CREATE TRIGGER users_set_updated_at
  BEFORE UPDATE ON incident.users
  FOR EACH ROW EXECUTE FUNCTION incident.set_updated_at();