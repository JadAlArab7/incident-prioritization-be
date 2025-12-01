-- Schema
CREATE SCHEMA IF NOT EXISTS incident;

-- Governorates (9 in Lebanon)
CREATE TABLE IF NOT EXISTS incident.governorates (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL UNIQUE,
  name_ar TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Districts (26 in Lebanon)
CREATE TABLE IF NOT EXISTS incident.districts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  governorate_id UUID NOT NULL REFERENCES incident.governorates(id) ON DELETE RESTRICT,
  name TEXT NOT NULL,
  name_ar TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  UNIQUE (governorate_id, name)
);

-- Towns (municipalities / localities)
CREATE TABLE IF NOT EXISTS incident.towns (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  district_id UUID NOT NULL REFERENCES incident.districts(id) ON DELETE RESTRICT,
  name TEXT NOT NULL,
  name_ar TEXT NOT NULL,
  lat DOUBLE PRECISION,
  lng DOUBLE PRECISION,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  UNIQUE (district_id, name)
);