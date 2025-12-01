CREATE INDEX IF NOT EXISTS idx_users_role_id ON incident.users(role_id);
CREATE INDEX IF NOT EXISTS idx_users_username ON incident.users(username);