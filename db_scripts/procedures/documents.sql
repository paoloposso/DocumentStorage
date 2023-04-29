CREATE OR REPLACE PROCEDURE document_insert(
  p_name VARCHAR(255),
  p_description TEXT,
  p_file_path TEXT,
  p_created_by INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO documents (name, description, file_path, created_by)
  VALUES (p_name, p_description, p_file_path, p_created_by);
END;
$$;

CREATE OR REPLACE PROCEDURE document_get_by_id_with_access(
  p_document_id INTEGER,
  p_user_id INTEGER,
  OUT p_id INTEGER,
  OUT p_name VARCHAR(255),
  OUT p_description TEXT,
  OUT p_file_path TEXT,
  OUT p_created_by INTEGER,
  OUT p_created_at TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
  SELECT d.id, d.name, d.description, d.file_path, d.created_by, d.created_at
  INTO p_id, p_name, p_description, p_file_path, p_created_by, p_created_at
  FROM documents d
  WHERE d.id = p_document_id
    AND (
      d.created_by = p_user_id
      OR EXISTS (
        SELECT 1
        FROM document_access da
        WHERE da.document_id = d.id
          AND (
            da.user_id = p_user_id
            OR EXISTS (
              SELECT 1
              FROM group_members gm
              WHERE gm.group_id = da.group_id
                AND gm.user_id = p_user_id
            )
          )
      )
    );
END;
$$;
