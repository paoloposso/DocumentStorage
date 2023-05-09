
CREATE DATABASE [documents];

CREATE TABLE IF NOT EXISTS users (
  id SERIAL PRIMARY KEY,
  email VARCHAR(64) UNIQUE NOT NULL,
  name VARCHAR(50) NOT NULL,
  hash VARCHAR(100) NOT NULL,
  user_role INTEGER NOT NULL,
  active BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS groups (
  id SERIAL PRIMARY KEY,
  name VARCHAR(50) UNIQUE NOT NULL,
  description TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS documents (
  id SERIAL PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  file_path TEXT NOT NULL,
  created_by INTEGER REFERENCES users(id) NOT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  category VARCHAR(20) NULL
);


CREATE TABLE IF NOT EXISTS group_members (
  group_id INTEGER REFERENCES groups(id) NOT NULL,
  user_id INTEGER REFERENCES users(id) NOT NULL,
  PRIMARY KEY (group_id, user_id)
);

CREATE TABLE IF NOT EXISTS document_access (
  document_id INTEGER REFERENCES documents(id) NOT NULL,
  group_id INTEGER REFERENCES groups(id),
  user_id INTEGER REFERENCES users(id),
  PRIMARY KEY (document_id, group_id, user_id)
);

CREATE INDEX IF NOT EXISTS users_email_idx ON users (email);

CREATE INDEX IF NOT EXISTS group_members_group_id_idx ON group_members (group_id);
CREATE INDEX IF NOT EXISTS group_members_user_id_idx ON group_members (user_id);

CREATE INDEX IF NOT EXISTS group_members_group_id_user_id_idx ON group_members (group_id, user_id);
