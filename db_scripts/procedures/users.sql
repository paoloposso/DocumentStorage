-- Procedure to insert a new user
CREATE OR REPLACE PROCEDURE user_insert(
  p_email VARCHAR(255),
  p_name VARCHAR(50),
  p_password VARCHAR(255),
  p_role VARCHAR(10) DEFAULT 'user'
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO users (email, name, password, role)
  VALUES (p_email, p_name, p_password, p_role);
END;
$$;

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

-- Procedure to retrieve a user by email
CREATE OR REPLACE FUNCTION user_get_by_email(
  p_email VARCHAR(255)
)
RETURNS TABLE (
  id INTEGER,
  email VARCHAR(255),
  name VARCHAR(50),
  password VARCHAR(255),
  role VARCHAR(10),
  created_at TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
  RETURN QUERY SELECT * FROM users WHERE email = p_email;
END;
$$;

CREATE OR REPLACE PROCEDURE group_add_user(
  p_group_id INTEGER,
  p_user_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  IF NOT EXISTS (
    SELECT 1
    FROM group_members
    WHERE group_id = p_group_id
      AND user_id = p_user_id
  ) THEN
    INSERT INTO group_members (group_id, user_id)
    VALUES (p_group_id, p_user_id);
  END IF;
END;
$$;

CREATE OR REPLACE PROCEDURE group_remove_user(
  p_group_id INTEGER,
  p_user_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  DELETE FROM group_members
  WHERE group_id = p_group_id
    AND user_id = p_user_id;
END;
$$;
