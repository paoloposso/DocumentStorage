
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
  IN p_email VARCHAR(255),
  IN p_name VARCHAR(50),
  IN p_hashed_password bytea,
  IN p_salt bytea,
  IN p_role VARCHAR(10),
  OUT p_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO users (email, name, password_salt, password_hash, role)
  VALUES (p_email, p_name, p_salt, p_hashed_password, p_role)
  RETURNING id INTO p_id;
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

CREATE OR REPLACE PROCEDURE add_user_to_group(
    IN user_id INTEGER,
    IN group_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM group_members
        WHERE group_id = $2 AND user_id = $1
    ) THEN
        RAISE NOTICE 'User % is already a member of group %', $1, $2;
    ELSE
        INSERT INTO group_members (group_id, user_id)
        VALUES ($2, $1);
    END IF;
END;
$$;
