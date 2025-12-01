-- Seed 26 Lebanese Districts
-- Source: Wikipedia - Districts of Lebanon
-- Note: Beirut has no districts (it is both governorate and district)
-- Akkar is a single-district governorate

-- Beirut (no sub-districts, but we create one entry for consistency)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222201', '11111111-1111-1111-1111-111111111101', 'Beirut', 'بيروت')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- Mount Lebanon (6 districts, minus Keserwan and Jbeil which moved to new governorate)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222202', '11111111-1111-1111-1111-111111111102', 'Baabda', 'بعبدا'),
  ('22222222-2222-2222-2222-222222222203', '11111111-1111-1111-1111-111111111102', 'Matn', 'المتن'),
  ('22222222-2222-2222-2222-222222222204', '11111111-1111-1111-1111-111111111102', 'Chouf', 'الشوف'),
  ('22222222-2222-2222-2222-222222222205', '11111111-1111-1111-1111-111111111102', 'Aley', 'عاليه')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- North Lebanon (7 districts, minus Akkar which became separate governorate)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222206', '11111111-1111-1111-1111-111111111103', 'Tripoli', 'طرابلس'),
  ('22222222-2222-2222-2222-222222222207', '11111111-1111-1111-1111-111111111103', 'Zgharta', 'زغرتا'),
  ('22222222-2222-2222-2222-222222222208', '11111111-1111-1111-1111-111111111103', 'Koura', 'الكورة'),
  ('22222222-2222-2222-2222-222222222209', '11111111-1111-1111-1111-111111111103', 'Bsharri', 'بشري'),
  ('22222222-2222-2222-2222-222222222210', '11111111-1111-1111-1111-111111111103', 'Batroun', 'البترون'),
  ('22222222-2222-2222-2222-222222222211', '11111111-1111-1111-1111-111111111103', 'Miniyeh-Danniyeh', 'المنية الضنية')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- South Lebanon (3 districts)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222212', '11111111-1111-1111-1111-111111111104', 'Sidon', 'صيدا'),
  ('22222222-2222-2222-2222-222222222213', '11111111-1111-1111-1111-111111111104', 'Tyre', 'صور'),
  ('22222222-2222-2222-2222-222222222214', '11111111-1111-1111-1111-111111111104', 'Jezzine', 'جزين')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- Beqaa (3 districts, minus Baalbek and Hermel which moved to new governorate)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222215', '11111111-1111-1111-1111-111111111105', 'Zahle', 'زحلة'),
  ('22222222-2222-2222-2222-222222222216', '11111111-1111-1111-1111-111111111105', 'West Beqaa', 'البقاع الغربي'),
  ('22222222-2222-2222-2222-222222222217', '11111111-1111-1111-1111-111111111105', 'Rashaya', 'راشيا')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- Nabatieh (4 districts)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222218', '11111111-1111-1111-1111-111111111106', 'Nabatieh', 'النبطية'),
  ('22222222-2222-2222-2222-222222222219', '11111111-1111-1111-1111-111111111106', 'Marjeyoun', 'مرجعيون'),
  ('22222222-2222-2222-2222-222222222220', '11111111-1111-1111-1111-111111111106', 'Hasbaya', 'حاصبيا'),
  ('22222222-2222-2222-2222-222222222221', '11111111-1111-1111-1111-111111111106', 'Bint Jbeil', 'بنت جبيل')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- Akkar (single district governorate)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111107', 'Akkar', 'عكار')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- Baalbek-Hermel (2 districts)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222223', '11111111-1111-1111-1111-111111111108', 'Baalbek', 'بعلبك'),
  ('22222222-2222-2222-2222-222222222224', '11111111-1111-1111-1111-111111111108', 'Hermel', 'الهرمل')
ON CONFLICT (governorate_id, name) DO NOTHING;

-- Keserwan-Jbeil (2 districts)
INSERT INTO incident.districts (id, governorate_id, name, name_ar) VALUES
  ('22222222-2222-2222-2222-222222222225', '11111111-1111-1111-1111-111111111109', 'Keserwan', 'كسروان'),
  ('22222222-2222-2222-2222-222222222226', '11111111-1111-1111-1111-111111111109', 'Jbeil', 'جبيل')
ON CONFLICT (governorate_id, name) DO NOTHING;