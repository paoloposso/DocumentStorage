CREATE OR REPLACE PROCEDURE add_group(
    IN p_name VARCHAR(50),
    IN p_description TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO groups (name, description)
    VALUES (p_name, p_description);
END;
$$;

CREATE OR REPLACE FUNCTION list_groups()
RETURNS TABLE (
    id integer,
    name varchar(50),
    description text
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY 
	SELECT groups.id, groups.name, groups.description
    FROM groups;
END;
$$;

CREATE OR REPLACE PROCEDURE get_group_by_id(
    IN p_id INTEGER,
    OUT p_name VARCHAR(50),
    OUT p_description TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT name, description
    INTO p_name, p_description
    FROM groups
    WHERE id = p_id;
END;
$$;

CREATE OR REPLACE PROCEDURE update_group(
    IN p_id INTEGER,
    IN p_name VARCHAR(50),
    IN p_description TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE groups
    SET name = p_name,
    	description = p_description
    WHERE id = p_id;
END;
$$;

CREATE OR REPLACE PROCEDURE delete_group_by_id(
    IN p_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM group_members
        WHERE group_id = p_id
    ) THEN
        RAISE NOTICE 'Group % has users associated to it', p_id;
    ELSE
        DELETE FROM groups
        WHERE id = p_id;
    END IF;
END;
$$;

CREATE OR REPLACE PROCEDURE list_group_members(
    IN p_group_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT u.id, u.email, u.name
    FROM users u
    INNER JOIN group_members gm ON u.id = gm.user_id
    WHERE gm.group_id = p_group_id;
END;
$$;

CREATE OR REPLACE FUNCTION get_users_in_group(
  IN group_id INTEGER
)
RETURNS TABLE (
  id INTEGER,
  email VARCHAR(64),
  name VARCHAR(50),
  user_role INTEGER,
  active BOOLEAN,
  created_at TIMESTAMP
)
AS $$
BEGIN
  RETURN QUERY
    SELECT u.id, u.email, u.name, u.user_role, u.active, u.created_at
    FROM users u
    INNER JOIN group_members gm ON gm.user_id = u.id
    WHERE gm.group_id = get_users_in_group.group_id;
END;
$$ LANGUAGE plpgsql;
