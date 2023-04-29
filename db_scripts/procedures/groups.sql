CREATE OR REPLACE PROCEDURE group_create(
  p_name VARCHAR(50),
  p_description TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO groups (name, description)
  VALUES (p_name, p_description);
END;
$$;
