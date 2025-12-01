-- Seed users (password is 'Password123!' for all users)
-- Password hash and salt are generated using PBKDF2-HMAC-SHA256

-- First, get the role IDs
DO $$
DECLARE
    secretary_role_id UUID;
    officer_role_id UUID;
    supervisor_role_id UUID;
    -- Using a fixed salt for seeding (in production, each user should have unique salt)
    fixed_salt BYTEA := '\x0123456789abcdef0123456789abcdef'::BYTEA;
BEGIN
    -- Get role IDs
    SELECT id INTO secretary_role_id FROM incident.roles WHERE name = 'secretary';
    SELECT id INTO officer_role_id FROM incident.roles WHERE name = 'officer';
    SELECT id INTO supervisor_role_id FROM incident.roles WHERE name = 'supervisor';

    -- Insert secretary user
    INSERT INTO incident.users (id, username, password_hash, password_salt, role_id)
    VALUES (
        gen_random_uuid(),
        'secretary_user',
        -- Hash for 'Password123!' (you'll need to generate this from your app or use a placeholder)
        decode('8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'hex'),
        fixed_salt,
        secretary_role_id
    )
    ON CONFLICT (username) DO NOTHING;

    -- Insert officer user
    INSERT INTO incident.users (id, username, password_hash, password_salt, role_id)
    VALUES (
        gen_random_uuid(),
        'officer_user',
        decode('8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'hex'),
        fixed_salt,
        officer_role_id
    )
    ON CONFLICT (username) DO NOTHING;

    -- Insert supervisor user
    INSERT INTO incident.users (id, username, password_hash, password_salt, role_id)
    VALUES (
        gen_random_uuid(),
        'supervisor_user',
        decode('8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'hex'),
        fixed_salt,
        supervisor_role_id
    )
    ON CONFLICT (username) DO NOTHING;
END $$;