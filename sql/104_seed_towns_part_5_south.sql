-- Seed Towns for South Lebanon Districts
-- Sidon: 22222222-2222-2222-2222-222222222212
-- Tyre: 22222222-2222-2222-2222-222222222213
-- Jezzine: 22222222-2222-2222-2222-222222222214

-- Sidon District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222212', 'Sidon', 'صيدا', 33.5667, 35.3833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222212', 'Ghazieh', 'الغازية', 33.5167, 35.3667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222212', 'Abra', 'عبرا', 33.5500, 35.4000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222212', 'Hlaliyeh', 'الهلالية', 33.5333, 35.3833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222212', 'Sarafand', 'صرفند', 33.4500, 35.3000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222212', 'Maghdouche', 'مغدوشة', 33.5333, 35.4167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222212', 'Darb El Sim', 'درب السيم', 33.5500, 35.3833)
ON CONFLICT (district_id, name) DO NOTHING;

-- Tyre District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222213', 'Tyre', 'صور', 33.2667, 35.2000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222213', 'Qana', 'قانا', 33.2167, 35.3000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222213', 'Abbassiyeh', 'العباسية', 33.2833, 35.2500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222213', 'Deir Qanoun En Nahr', 'دير قانون النهر', 33.2500, 35.2667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222213', 'Jouaiya', 'جويا', 33.2333, 35.2833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222213', 'Ras El Ain', 'رأس العين', 33.2833, 35.2167)
ON CONFLICT (district_id, name) DO NOTHING;

-- Jezzine District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222214', 'Jezzine', 'جزين', 33.5500, 35.5833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222214', 'Bkassine', 'بكاسين', 33.5333, 35.5667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222214', 'Roum', 'روم', 33.5167, 35.5500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222214', 'Kfar Falous', 'كفرفالوس', 33.5000, 35.5333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222214', 'Ain Majdalain', 'عين مجدلين', 33.5333, 35.6000)
ON CONFLICT (district_id, name) DO NOTHING;

-- Nabatieh Districts
-- Nabatieh: 22222222-2222-2222-2222-222222222218
-- Marjeyoun: 22222222-2222-2222-2222-222222222219
-- Hasbaya: 22222222-2222-2222-2222-222222222220
-- Bint Jbeil: 22222222-2222-2222-2222-222222222221

-- Nabatieh District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222218', 'Nabatieh', 'النبطية', 33.3833, 35.4833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222218', 'Kfar Roummane', 'كفررمان', 33.3667, 35.4667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222218', 'Habbouch', 'حبوش', 33.3500, 35.4833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222218', 'Arnoun', 'أرنون', 33.3667, 35.5167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222218', 'Kfar Tebnit', 'كفرتبنيت', 33.3500, 35.5000)
ON CONFLICT (district_id, name) DO NOTHING;

-- Marjeyoun District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222219', 'Marjeyoun', 'مرجعيون', 33.3667, 35.5833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222219', 'Khiam', 'الخيام', 33.3500, 35.6333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222219', 'Qlaiaa', 'القليعة', 33.3333, 35.5667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222219', 'Deir Mimas', 'دير ميماس', 33.3167, 35.5500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222219', 'Ibl El Saqi', 'إبل السقي', 33.3333, 35.6167)
ON CONFLICT (district_id, name) DO NOTHING;

-- Hasbaya District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222220', 'Hasbaya', 'حاصبيا', 33.4000, 35.6833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222220', 'Rashaya El Foukhar', 'راشيا الفخار', 33.3833, 35.7000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222220', 'Kfar Chouba', 'كفرشوبا', 33.4167, 35.7333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222220', 'Chebaa', 'شبعا', 33.4333, 35.7667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222220', 'Ain Ata', 'عين عطا', 33.4000, 35.7167)
ON CONFLICT (district_id, name) DO NOTHING;

-- Bint Jbeil District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222221', 'Bint Jbeil', 'بنت جبيل', 33.1167, 35.4333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222221', 'Aita El Chaab', 'عيتا الشعب', 33.0833, 35.4000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222221', 'Rmeich', 'رميش', 33.0833, 35.3667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222221', 'Ainata', 'عيناتا', 33.1333, 35.4500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222221', 'Maroun El Ras', 'مارون الراس', 33.1000, 35.4500)
ON CONFLICT (district_id, name) DO NOTHING;