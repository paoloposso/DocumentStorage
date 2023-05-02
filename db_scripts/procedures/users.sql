CREATE OR REPLACE PROCEDURE user_update_role(
  p_email VARCHAR(64),
  p_role INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  UPDATE users
  SET user_role = p_role
  WHERE email = p_email;
END;
$$;

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

CREATE OR REPLACE PROCEDURE get_user_auth_info(IN email VARCHAR(64), OUT id INTEGER, OUT hash VARCHAR(100), OUT salt VARCHAR(100))
AS $$
BEGIN
  SELECT id, hash, salt INTO id, hash, salt FROM users WHERE email = get_user_auth_info.email;
END;
$$ LANGUAGE plpgsql;


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
