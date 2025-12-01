-- ============================================
-- MAIN SEED FILE
-- Run this file to set up the entire database
-- All tables are created under the 'incident' schema
-- ============================================

-- ============================================
-- PART 0: CREATE SCHEMA
-- ============================================

CREATE SCHEMA IF NOT EXISTS incident;

-- ============================================
-- PART 1: CREATE TABLES
-- ============================================

-- 001_create_tables.sql (Users and Roles)
CREATE TABLE IF NOT EXISTS incident.roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS incident.users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    password_salt VARCHAR(255) NOT NULL,
    role_id INTEGER REFERENCES incident.roles(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 101_create_tables_geo.sql (Geographic tables)
CREATE TABLE IF NOT EXISTS incident.governorates (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    name_ar VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS incident.districts (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    name_ar VARCHAR(100),
    governorate_id INTEGER REFERENCES incident.governorates(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS incident.towns (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    name_ar VARCHAR(100),
    district_id INTEGER REFERENCES incident.districts(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 201_create_tables_incident.sql (Incident tables)
CREATE TABLE IF NOT EXISTS incident.incident_statuses (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    name_ar VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS incident.incident_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    name_ar VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS incident.locations (
    id SERIAL PRIMARY KEY,
    governorate_id INTEGER REFERENCES incident.governorates(id),
    district_id INTEGER REFERENCES incident.districts(id),
    town_id INTEGER REFERENCES incident.towns(id),
    address TEXT,
    latitude DECIMAL(10, 8),
    longitude DECIMAL(11, 8),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS incident.incidents (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    incident_type_id INTEGER REFERENCES incident.incident_types(id),
    status_id INTEGER REFERENCES incident.incident_statuses(id),
    location_id INTEGER REFERENCES incident.locations(id),
    reporter_name VARCHAR(100),
    reporter_phone VARCHAR(20),
    reporter_email VARCHAR(100),
    assigned_to INTEGER REFERENCES incident.users(id),
    created_by INTEGER REFERENCES incident.users(id),
    priority INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS incident.attachments (
    id SERIAL PRIMARY KEY,
    incident_id INTEGER REFERENCES incident.incidents(id) ON DELETE CASCADE,
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    file_type VARCHAR(100),
    file_size BIGINT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- PART 2: SEED ROLES
-- ============================================

-- 002_seed_roles.sql
INSERT INTO incident.roles (name) VALUES ('supervisor') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.roles (name) VALUES ('officer') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.roles (name) VALUES ('secretary') ON CONFLICT (name) DO NOTHING;

-- ============================================
-- PART 3: SEED INCIDENT STATUSES
-- ============================================

-- 202_seed_incident_statuses.sql
INSERT INTO incident.incident_statuses (name, name_ar) VALUES ('pending', 'قيد الانتظار') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_statuses (name, name_ar) VALUES ('in_progress', 'قيد التنفيذ') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_statuses (name, name_ar) VALUES ('resolved', 'تم الحل') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_statuses (name, name_ar) VALUES ('closed', 'مغلق') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_statuses (name, name_ar) VALUES ('rejected', 'مرفوض') ON CONFLICT (name) DO NOTHING;

-- ============================================
-- PART 4: SEED INCIDENT TYPES
-- ============================================

-- 203_seed_incident_types.sql
INSERT INTO incident.incident_types (name, name_ar) VALUES ('fire', 'حريق') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('flood', 'فيضان') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('earthquake', 'زلزال') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('traffic_accident', 'حادث سير') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('medical_emergency', 'طوارئ طبية') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('crime', 'جريمة') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('infrastructure_damage', 'ضرر في البنية التحتية') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('environmental_hazard', 'خطر بيئي') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('public_disturbance', 'إزعاج عام') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.incident_types (name, name_ar) VALUES ('other', 'أخرى') ON CONFLICT (name) DO NOTHING;

-- ============================================
-- PART 5: SEED GOVERNORATES
-- ============================================

-- 102_seed_governorates.sql
INSERT INTO incident.governorates (name, name_ar) VALUES ('Akkar', 'عكار') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('Baalbek-Hermel', 'بعلبك الهرمل') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('Beirut', 'بيروت') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('Beqaa', 'البقاع') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('Keserwan-Jbeil', 'كسروان جبيل') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('Mount Lebanon', 'جبل لبنان') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('Nabatieh', 'النبطية') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('North', 'الشمال') ON CONFLICT (name) DO NOTHING;
INSERT INTO incident.governorates (name, name_ar) VALUES ('South', 'الجنوب') ON CONFLICT (name) DO NOTHING;

-- ============================================
-- PART 6: SEED DISTRICTS
-- ============================================

-- 103_seed_districts.sql
-- Akkar Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Akkar', 'عكار', id FROM incident.governorates WHERE name = 'Akkar' 
ON CONFLICT DO NOTHING;

-- Baalbek-Hermel Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Baalbek', 'بعلبك', id FROM incident.governorates WHERE name = 'Baalbek-Hermel' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Hermel', 'الهرمل', id FROM incident.governorates WHERE name = 'Baalbek-Hermel' 
ON CONFLICT DO NOTHING;

-- Beirut Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Beirut', 'بيروت', id FROM incident.governorates WHERE name = 'Beirut' 
ON CONFLICT DO NOTHING;

-- Beqaa Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Rashaya', 'راشيا', id FROM incident.governorates WHERE name = 'Beqaa' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Western Beqaa', 'البقاع الغربي', id FROM incident.governorates WHERE name = 'Beqaa' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Zahle', 'زحلة', id FROM incident.governorates WHERE name = 'Beqaa' 
ON CONFLICT DO NOTHING;

-- Keserwan-Jbeil Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Jbeil', 'جبيل', id FROM incident.governorates WHERE name = 'Keserwan-Jbeil' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Keserwan', 'كسروان', id FROM incident.governorates WHERE name = 'Keserwan-Jbeil' 
ON CONFLICT DO NOTHING;

-- Mount Lebanon Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Aley', 'عاليه', id FROM incident.governorates WHERE name = 'Mount Lebanon' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Baabda', 'بعبدا', id FROM incident.governorates WHERE name = 'Mount Lebanon' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Chouf', 'الشوف', id FROM incident.governorates WHERE name = 'Mount Lebanon' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Matn', 'المتن', id FROM incident.governorates WHERE name = 'Mount Lebanon' 
ON CONFLICT DO NOTHING;

-- Nabatieh Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Bint Jbeil', 'بنت جبيل', id FROM incident.governorates WHERE name = 'Nabatieh' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Hasbaya', 'حاصبيا', id FROM incident.governorates WHERE name = 'Nabatieh' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Marjeyoun', 'مرجعيون', id FROM incident.governorates WHERE name = 'Nabatieh' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Nabatieh', 'النبطية', id FROM incident.governorates WHERE name = 'Nabatieh' 
ON CONFLICT DO NOTHING;

-- North Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Batroun', 'البترون', id FROM incident.governorates WHERE name = 'North' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Bsharri', 'بشري', id FROM incident.governorates WHERE name = 'North' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Koura', 'الكورة', id FROM incident.governorates WHERE name = 'North' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Minieh-Danniyeh', 'المنية الضنية', id FROM incident.governorates WHERE name = 'North' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Tripoli', 'طرابلس', id FROM incident.governorates WHERE name = 'North' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Zgharta', 'زغرتا', id FROM incident.governorates WHERE name = 'North' 
ON CONFLICT DO NOTHING;

-- South Districts
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Jezzine', 'جزين', id FROM incident.governorates WHERE name = 'South' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Sidon', 'صيدا', id FROM incident.governorates WHERE name = 'South' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.districts (name, name_ar, governorate_id) 
SELECT 'Tyre', 'صور', id FROM incident.governorates WHERE name = 'South' 
ON CONFLICT DO NOTHING;

-- ============================================
-- PART 7: SEED TOWNS (Sample towns for each district)
-- ============================================

-- Akkar Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Halba', 'حلبا', id FROM incident.districts WHERE name = 'Akkar' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Qoubaiyat', 'القبيات', id FROM incident.districts WHERE name = 'Akkar' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Bebnine', 'ببنين', id FROM incident.districts WHERE name = 'Akkar' 
ON CONFLICT DO NOTHING;

-- Beirut Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Achrafieh', 'الأشرفية', id FROM incident.districts WHERE name = 'Beirut' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Hamra', 'الحمرا', id FROM incident.districts WHERE name = 'Beirut' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Verdun', 'فردان', id FROM incident.districts WHERE name = 'Beirut' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Ras Beirut', 'رأس بيروت', id FROM incident.districts WHERE name = 'Beirut' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Mazraa', 'المزرعة', id FROM incident.districts WHERE name = 'Beirut' 
ON CONFLICT DO NOTHING;

-- Baalbek Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Baalbek', 'بعلبك', id FROM incident.districts WHERE name = 'Baalbek' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Ras Baalbek', 'رأس بعلبك', id FROM incident.districts WHERE name = 'Baalbek' 
ON CONFLICT DO NOTHING;

-- Hermel Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Hermel', 'الهرمل', id FROM incident.districts WHERE name = 'Hermel' 
ON CONFLICT DO NOTHING;

-- Zahle Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Zahle', 'زحلة', id FROM incident.districts WHERE name = 'Zahle' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Chtaura', 'شتورا', id FROM incident.districts WHERE name = 'Zahle' 
ON CONFLICT DO NOTHING;

-- Western Beqaa Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Saghbine', 'صغبين', id FROM incident.districts WHERE name = 'Western Beqaa' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Machghara', 'مشغرة', id FROM incident.districts WHERE name = 'Western Beqaa' 
ON CONFLICT DO NOTHING;

-- Rashaya Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Rashaya', 'راشيا', id FROM incident.districts WHERE name = 'Rashaya' 
ON CONFLICT DO NOTHING;

-- Jbeil Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Byblos', 'جبيل', id FROM incident.districts WHERE name = 'Jbeil' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Amchit', 'عمشيت', id FROM incident.districts WHERE name = 'Jbeil' 
ON CONFLICT DO NOTHING;

-- Keserwan Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Jounieh', 'جونية', id FROM incident.districts WHERE name = 'Keserwan' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Harissa', 'حريصا', id FROM incident.districts WHERE name = 'Keserwan' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Zouk Mosbeh', 'ذوق مصبح', id FROM incident.districts WHERE name = 'Keserwan' 
ON CONFLICT DO NOTHING;

-- Matn Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Jdeideh', 'الجديدة', id FROM incident.districts WHERE name = 'Matn' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Antelias', 'انطلياس', id FROM incident.districts WHERE name = 'Matn' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Bikfaya', 'بكفيا', id FROM incident.districts WHERE name = 'Matn' 
ON CONFLICT DO NOTHING;

-- Baabda Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Baabda', 'بعبدا', id FROM incident.districts WHERE name = 'Baabda' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Hazmieh', 'الحازمية', id FROM incident.districts WHERE name = 'Baabda' 
ON CONFLICT DO NOTHING;

-- Aley Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Aley', 'عاليه', id FROM incident.districts WHERE name = 'Aley' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Bhamdoun', 'بحمدون', id FROM incident.districts WHERE name = 'Aley' 
ON CONFLICT DO NOTHING;

-- Chouf Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Beiteddine', 'بيت الدين', id FROM incident.districts WHERE name = 'Chouf' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Deir el Qamar', 'دير القمر', id FROM incident.districts WHERE name = 'Chouf' 
ON CONFLICT DO NOTHING;

-- Tripoli Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Tripoli', 'طرابلس', id FROM incident.districts WHERE name = 'Tripoli' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Mina', 'الميناء', id FROM incident.districts WHERE name = 'Tripoli' 
ON CONFLICT DO NOTHING;

-- Koura Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Amioun', 'أميون', id FROM incident.districts WHERE name = 'Koura' 
ON CONFLICT DO NOTHING;

-- Zgharta Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Zgharta', 'زغرتا', id FROM incident.districts WHERE name = 'Zgharta' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Ehden', 'إهدن', id FROM incident.districts WHERE name = 'Zgharta' 
ON CONFLICT DO NOTHING;

-- Bsharri Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Bsharri', 'بشري', id FROM incident.districts WHERE name = 'Bsharri' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'The Cedars', 'الأرز', id FROM incident.districts WHERE name = 'Bsharri' 
ON CONFLICT DO NOTHING;

-- Batroun Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Batroun', 'البترون', id FROM incident.districts WHERE name = 'Batroun' 
ON CONFLICT DO NOTHING;

-- Minieh-Danniyeh Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Minieh', 'المنية', id FROM incident.districts WHERE name = 'Minieh-Danniyeh' 
ON CONFLICT DO NOTHING;
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Sir Danniyeh', 'سير الضنية', id FROM incident.districts WHERE name = 'Minieh-Danniyeh' 
ON CONFLICT DO NOTHING;

-- Sidon Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Sidon', 'صيدا', id FROM incident.districts WHERE name = 'Sidon' 
ON CONFLICT DO NOTHING;

-- Tyre Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Tyre', 'صور', id FROM incident.districts WHERE name = 'Tyre' 
ON CONFLICT DO NOTHING;

-- Jezzine Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Jezzine', 'جزين', id FROM incident.districts WHERE name = 'Jezzine' 
ON CONFLICT DO NOTHING;

-- Nabatieh Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Nabatieh', 'النبطية', id FROM incident.districts WHERE name = 'Nabatieh' 
ON CONFLICT DO NOTHING;

-- Bint Jbeil Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Bint Jbeil', 'بنت جبيل', id FROM incident.districts WHERE name = 'Bint Jbeil' 
ON CONFLICT DO NOTHING;

-- Marjeyoun Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Marjeyoun', 'مرجعيون', id FROM incident.districts WHERE name = 'Marjeyoun' 
ON CONFLICT DO NOTHING;

-- Hasbaya Towns
INSERT INTO incident.towns (name, name_ar, district_id) 
SELECT 'Hasbaya', 'حاصبيا', id FROM incident.districts WHERE name = 'Hasbaya' 
ON CONFLICT DO NOTHING;

-- ============================================
-- PART 8: CREATE INDEXES
-- ============================================

-- 004_indexes.sql (User indexes)
CREATE INDEX IF NOT EXISTS idx_users_username ON incident.users(username);
CREATE INDEX IF NOT EXISTS idx_users_role_id ON incident.users(role_id);

-- 105_indexes_geo.sql (Geographic indexes)
CREATE INDEX IF NOT EXISTS idx_districts_governorate_id ON incident.districts(governorate_id);
CREATE INDEX IF NOT EXISTS idx_towns_district_id ON incident.towns(district_id);

-- 204_indexes_incident.sql (Incident indexes)
CREATE INDEX IF NOT EXISTS idx_incidents_status_id ON incident.incidents(status_id);
CREATE INDEX IF NOT EXISTS idx_incidents_incident_type_id ON incident.incidents(incident_type_id);
CREATE INDEX IF NOT EXISTS idx_incidents_location_id ON incident.incidents(location_id);
CREATE INDEX IF NOT EXISTS idx_incidents_assigned_to ON incident.incidents(assigned_to);
CREATE INDEX IF NOT EXISTS idx_incidents_created_by ON incident.incidents(created_by);
CREATE INDEX IF NOT EXISTS idx_incidents_created_at ON incident.incidents(created_at);
CREATE INDEX IF NOT EXISTS idx_locations_governorate_id ON incident.locations(governorate_id);
CREATE INDEX IF NOT EXISTS idx_locations_district_id ON incident.locations(district_id);
CREATE INDEX IF NOT EXISTS idx_locations_town_id ON incident.locations(town_id);
CREATE INDEX IF NOT EXISTS idx_attachments_incident_id ON incident.attachments(incident_id);

-- ============================================
-- DONE!
-- ============================================
SELECT 'Database setup completed successfully!' AS status;