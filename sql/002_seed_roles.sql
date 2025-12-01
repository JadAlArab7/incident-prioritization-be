INSERT INTO roles (name) VALUES
  ('secretary'),
  ('officer'),
  ('supervisor')
ON CONFLICT (name) DO NOTHING;