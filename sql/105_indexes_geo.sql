-- Indexes for geography tables

CREATE INDEX IF NOT EXISTS idx_districts_governorate ON incident.districts(governorate_id);
CREATE INDEX IF NOT EXISTS idx_towns_district ON incident.towns(district_id);
CREATE INDEX IF NOT EXISTS idx_towns_lat_lng ON incident.towns(lat, lng);