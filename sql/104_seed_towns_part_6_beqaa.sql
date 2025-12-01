-- Seed Towns for Beqaa Districts
-- Zahle: 22222222-2222-2222-2222-222222222215
-- West Beqaa: 22222222-2222-2222-2222-222222222216
-- Rashaya: 22222222-2222-2222-2222-222222222217

-- Zahle District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Zahle', 'زحلة', 33.8500, 35.9000),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Chtaura', 'شتورا', 33.8167, 35.8500),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Taalabaya', 'تعلبايا', 33.8333, 35.8667),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Saadnayel', 'سعدنايل', 33.8333, 35.8833),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Bar Elias', 'بر الياس', 33.7833, 35.9000),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Kab Elias', 'كب الياس', 33.8000, 35.8667),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Anjar', 'عنجر', 33.7333, 35.9333),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222215', 'Majdel Anjar', 'مجدل عنجر', 33.7167, 35.9000)
ON CONFLICT (district_id, name) DO NOTHING;

-- West Beqaa District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222216', 'Joub Jannine', 'جب جنين', 33.6333, 35.7833),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222216', 'Machghara', 'مشغرة', 33.5833, 35.7667),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222216', 'Saghbine', 'صغبين', 33.6167, 35.7500),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222216', 'Qaraoun', 'قرعون', 33.5667, 35.8000),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222216', 'Kefraya', 'كفريا', 33.6667, 35.8167),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222216', 'Mansoura', 'المنصورة', 33.6500, 35.8000)
ON CONFLICT (district_id, name) DO NOTHING;

-- Rashaya District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222217', 'Rashaya', 'راشيا', 33.5000, 35.8500),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222217', 'Kfar Qouq', 'كفرقوق', 33.4833, 35.8333),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222217', 'Ain Ata', 'عين عطا', 33.5167, 35.8667),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222217', 'Bakka', 'بكا', 33.4667, 35.8167),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222217', 'Dahr El Ahmar', 'ضهر الأحمر', 33.5333, 35.8833)
ON CONFLICT (district_id, name) DO NOTHING;

-- Baalbek-Hermel Districts
-- Baalbek: 22222222-2222-2222-2222-222222222223
-- Hermel: 22222222-2222-2222-2222-222222222224

-- Baalbek District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Baalbek', 'بعلبك', 34.0000, 36.2167),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Ras Baalbek', 'رأس بعلبك', 34.2667, 36.4167),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Deir El Ahmar', 'دير الأحمر', 34.1333, 36.1500),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Labweh', 'اللبوة', 34.1500, 36.3833),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Douris', 'دورس', 33.9667, 36.1833),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Chaat', 'شعث', 34.0333, 36.2333),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Iaat', 'إيعات', 34.0167, 36.1667),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222223', 'Nabi Chit', 'النبي شيت', 33.9500, 36.1333)
ON CONFLICT (district_id, name) DO NOTHING;

-- Hermel District
INSERT INTO incident.towns (id, district_id, name, name_ar, lat, lng) VALUES
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222224', 'Hermel', 'الهرمل', 34.3833, 36.3833),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222224', 'Qaa', 'القاع', 34.3500, 36.4833),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222224', 'Fakeha', 'فاكهة', 34.3167, 36.3500),
  (uuid_generate_v4(), '22222222-2222-2222-2222-222222222224', 'Chwaghir', 'شواغير', 34.4000, 36.4167)
ON CONFLICT (district_id, name) DO NOTHING;