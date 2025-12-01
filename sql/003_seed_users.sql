-- Example: seed one demo user (password will be hashed in code, not here)
-- The actual seeding will be done via the application's seeding service
-- Password: Admin@123
-- Role: supervisor

-- This is a placeholder - run the application's seed endpoint or use the DbSeeder service
-- INSERT INTO users (username, password_hash, password_salt, role_id)
-- VALUES ('admin', decode('HASH_HERE', 'hex'), decode('SALT_HERE', 'hex'),
--         (SELECT id FROM roles WHERE name = 'supervisor'));