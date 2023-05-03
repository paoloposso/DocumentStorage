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

CREATE OR REPLACE PROCEDURE list_groups()
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT id, name, description, created_at
    FROM groups;
END;
$$;

CREATE OR REPLACE PROCEDURE get_group_by_id(
    IN p_id INTEGER,
    OUT p_name VARCHAR(50),
    OUT p_description TEXT,
    OUT p_created_at TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT name, description, created_at
    INTO p_name, p_description, p_created_at
    FROM groups
    WHERE id = p_id;
END;
$$;

CREATE OR REPLACE PROCEDURE update_group(
    IN p_id INTEGER,
    IN p_name VARCHAR(50)
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE groups
    SET name = p_name
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
