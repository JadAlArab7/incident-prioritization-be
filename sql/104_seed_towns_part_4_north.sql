-- Seed Towns for North Lebanon Districts
-- Tripoli: 22222222-2222-2222-2222-222222222206
-- Zgharta: 22222222-2222-2222-2222-222222222207
-- Koura: 22222222-2222-2222-2222-222222222208
-- Bsharri: 22222222-2222-2222-2222-222222222209
-- Batroun: 22222222-2222-2222-2222-222222222210
-- Miniyeh-Danniyeh: 22222222-2222-2222-2222-222222222211

-- Tripoli District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222206', 'Tripoli', 'طرابلس', 34.4333, 35.8500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222206', 'El Mina', 'الميناء', 34.4500, 35.8167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222206', 'Beddawi', 'البداوي', 34.4667, 35.8667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222206', 'Qalamoun', 'القلمون', 34.4667, 35.8000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222206', 'Ras Maska', 'رأس مسقا', 34.4000, 35.8333)
ON CONFLICT (district_id, name) DO NOTHING;

-- Zgharta District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222207', 'Zgharta', 'زغرتا', 34.4000, 35.8833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222207', 'Ehden', 'إهدن', 34.3000, 35.9833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222207', 'Miziara', 'مزيارة', 34.3167, 35.9333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222207', 'Ardeh', 'أردة', 34.3333, 35.9167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222207', 'Kfarsghab', 'كفرصغاب', 34.2833, 35.9667)
ON CONFLICT (district_id, name) DO NOTHING;

-- Koura District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222208', 'Amioun', 'أميون', 34.3000, 35.8167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222208', 'Kousba', 'كوسبا', 34.2833, 35.8500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222208', 'Enfeh', 'أنفه', 34.3500, 35.7333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222208', 'Chekka', 'شكا', 34.3167, 35.7333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222208', 'Kfar Hazir', 'كفر حزير', 34.3167, 35.7833)
ON CONFLICT (district_id, name) DO NOTHING;

-- Bsharri District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222209', 'Bsharri', 'بشري', 34.2500, 36.0167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222209', 'Hasroun', 'حصرون', 34.2333, 36.0333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222209', 'Hadath El Jebbeh', 'حدث الجبة', 34.2167, 36.0000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222209', 'Bqaa Kafra', 'بقاع كفرا', 34.2333, 36.0500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222209', 'The Cedars', 'الأرز', 34.2500, 36.0500)
ON CONFLICT (district_id, name) DO NOTHING;

-- Batroun District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222210', 'Batroun', 'البترون', 34.2500, 35.6667),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222210', 'Douma', 'دوما', 34.1833, 35.8500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222210', 'Tannourine', 'تنورين', 34.2000, 35.9167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222210', 'Hardine', 'حردين', 34.2167, 35.8833),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222210', 'Kfour El Arabi', 'كفور العربي', 34.2333, 35.7167)
ON CONFLICT (district_id, name) DO NOTHING;

-- Miniyeh-Danniyeh District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222211', 'Miniyeh', 'المنية', 34.4500, 35.9500),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222211', 'Sir Ed Danniyeh', 'سير الضنية', 34.4167, 36.0333),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222211', 'Bakhoun', 'بخعون', 34.3833, 36.0167),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222211', 'Kfar Habou', 'كفر حبو', 34.4000, 36.0000),
  (gen_random_uuid(), '22222222-2222-2222-2222-222222222211', 'Assoun', 'عاصون', 34.3667, 36.0333)
ON CONFLICT (district_id, name) DO NOTHING;