
-- Procedure to update the role of a user
CREATE OR REPLACE PROCEDURE user_update_role(
  p_email VARCHAR(255),
  p_role VARCHAR(10)
)
LANGUAGE plpgsql
AS $$
BEGIN
  UPDATE users
  SET role = p_role
  WHERE email = p_email;
END;
$$;

CREATE OR REPLACE PROCEDURE user_insert(
  p_email VARCHAR(255),
  p_name VARCHAR(50),
  p_hashed_password bytea,
  p_salt bytea,
  p_role VARCHAR(10)
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO users (email, name, password_salt, password_hash, role)
  VALUES (p_email, p_name, salt, hashed_password, p_role);
END;
$$;

CREATE OR REPLACE FUNCTION user_get_by_email(
  p_email VARCHAR(255)
)
RETURNS TABLE (
  id INTEGER,
  email VARCHAR(255),
  name VARCHAR(50),
  password_salt bytea,
  password_hash bytea,
  role VARCHAR(10),
  created_at TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
  RETURN QUERY SELECT id, email, name, password_salt, password_hash, role, created_at
    FROM users WHERE email = p_email;
END;
$$;
