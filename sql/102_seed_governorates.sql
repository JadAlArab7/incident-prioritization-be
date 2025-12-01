-- Seed 9 Lebanese Governorates (as of 2017, including Keserwan-Jbeil)
-- Source: Wikipedia - Governorates of Lebanon

INSERT INTO incident.governorates (id, name, name_ar) VALUES
  ('11111111-1111-1111-1111-111111111101', 'Beirut', 'بيروت'),
  ('11111111-1111-1111-1111-111111111102', 'Mount Lebanon', 'جبل لبنان'),
  ('11111111-1111-1111-1111-111111111103', 'North Lebanon', 'الشمال'),
  ('11111111-1111-1111-1111-111111111104', 'South Lebanon', 'الجنوب'),
  ('11111111-1111-1111-1111-111111111105', 'Beqaa', 'البقاع'),
  ('11111111-1111-1111-1111-111111111106', 'Nabatieh', 'النبطية'),
  ('11111111-1111-1111-1111-111111111107', 'Akkar', 'عكار'),
  ('11111111-1111-1111-1111-111111111108', 'Baalbek-Hermel', 'بعلبك الهرمل'),
  ('11111111-1111-1111-1111-111111111109', 'Keserwan-Jbeil', 'كسروان جبيل')
ON CONFLICT (name) DO NOTHING;