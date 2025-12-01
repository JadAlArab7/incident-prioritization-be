-- Simple seed users SQL
-- NOTE: Run this only after roles are seeded (002_seed_roles.sql)
-- These users will need password reset via the application

INSERT INTO incident.users (id, username, password_hash, password_salt, role_id)
SELECT 
    gen_random_uuid(),
    'secretary_user',
    gen_random_bytes(32),  -- temporary hash
    gen_random_bytes(16),  -- temporary salt
    r.id
FROM incident.roles r WHERE r.name = 'secretary'
ON CONFLICT (username) DO NOTHING;

INSERT INTO incident.users (id, username, password_hash, password_salt, role_id)
SELECT 
    gen_random_uuid(),
    'officer_user',
    gen_random_bytes(32),
    gen_random_bytes(16),
    r.id
FROM incident.roles r WHERE r.name = 'officer'
ON CONFLICT (username) DO NOTHING;

INSERT INTO incident.users (id, username, password_hash, password_salt, role_id)
SELECT 
    gen_random_uuid(),
    'supervisor_user',
    gen_random_bytes(32),
    gen_random_bytes(16),
    r.id
FROM incident.roles r WHERE r.name = 'supervisor'
ON CONFLICT (username) DO NOTHING;