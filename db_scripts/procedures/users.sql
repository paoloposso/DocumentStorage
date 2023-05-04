CREATE OR REPLACE PROCEDURE user_insert(
  IN p_email VARCHAR(64),
  IN p_name VARCHAR(50),
  IN p_hashed_password VARCHAR(100),
  IN p_role INTEGER,
  OUT p_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO users (email, name, hash, user_role)
  VALUES (p_email, p_name, p_hashed_password, p_role)
  RETURNING id INTO p_id;
END;
$$;

CREATE OR REPLACE PROCEDURE get_user_auth_info(
  IN p_email VARCHAR(64),
  OUT p_id INTEGER,
  OUT p_hash VARCHAR(100)
)
AS $$
BEGIN
  SELECT id, hash INTO p_id, p_hash FROM users WHERE email = p_email;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE update_user(
  IN p_user_id INTEGER,
  IN p_active BOOLEAN,
  IN p_user_role INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  UPDATE users
  SET active = p_active,
      user_role = p_user_role
  WHERE id = p_user_id;
  
  COMMIT;
END;
$$;

CREATE OR REPLACE PROCEDURE add_user_to_group(
    IN p_user_id INTEGER,
    IN p_group_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM group_members
        WHERE group_id = p_group_id AND user_id = p_user_id
    ) THEN
        RAISE NOTICE 'User % is already a member of group %', p_user_id, p_group_id;
    ELSE
        INSERT INTO group_members (group_id, user_id)
        VALUES (p_group_id, p_user_id);
    END IF;
END;
$$;

CREATE OR REPLACE PROCEDURE list_users()
LANGUAGE plpgsql
AS $$
BEGIN
  SELECT id, email, name, user_role, active, created_at FROM users;
END;
$$;

CREATE OR REPLACE PROCEDURE get_user_by_id(
    IN p_user_id INTEGER,
    OUT p_user_email TEXT,
    OUT p_user_name TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT email, name
    INTO p_user_email, p_user_name
    FROM users
    WHERE id = p_user_id;
END;
$$;
