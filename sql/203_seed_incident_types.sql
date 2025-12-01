-- Seed incident types with bilingual names
INSERT INTO incident.incident_types (id, name, name_en, name_ar) VALUES
  ('33333333-3333-3333-3333-333333333301', 'fire', 'Fire', 'حريق'),
  ('33333333-3333-3333-3333-333333333302', 'traffic_accident', 'Traffic Accident', 'حادث سير'),
  ('33333333-3333-3333-3333-333333333303', 'medical_emergency', 'Medical Emergency', 'حالة طبية طارئة'),
  ('33333333-3333-3333-3333-333333333304', 'flood', 'Flood', 'فيضان'),
  ('33333333-3333-3333-3333-333333333305', 'landslide', 'Landslide', 'انهيار أرضي'),
  ('33333333-3333-3333-3333-333333333306', 'power_outage', 'Power Outage', 'انقطاع التيار الكهربائي'),
  ('33333333-3333-3333-3333-333333333307', 'road_closure', 'Road Closure', 'إغلاق طريق'),
  ('33333333-3333-3333-3333-333333333308', 'structural_collapse', 'Structural Collapse', 'انهيار هيكلي'),
  ('33333333-3333-3333-3333-333333333309', 'missing_person', 'Missing Person', 'شخص مفقود'),
  ('33333333-3333-3333-3333-333333333310', 'environmental_pollution', 'Environmental Pollution', 'تلوث بيئي'),
  ('33333333-3333-3333-3333-333333333311', 'earthquake', 'Earthquake', 'زلزال'),
  ('33333333-3333-3333-3333-333333333312', 'explosion', 'Explosion', 'انفجار'),
  ('33333333-3333-3333-3333-333333333313', 'water_shortage', 'Water Shortage', 'نقص المياه'),
  ('33333333-3333-3333-3333-333333333314', 'civil_unrest', 'Civil Unrest', 'اضطرابات مدنية'),
  ('33333333-3333-3333-3333-333333333315', 'other', 'Other', 'أخرى')
ON CONFLICT (name) DO NOTHING;