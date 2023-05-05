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

CREATE OR REPLACE FUNCTION get_document_by_id_for_user(
  document_id INTEGER,
  user_id INTEGER
)
RETURNS TABLE (
  id INTEGER,
  name VARCHAR(255),
  description TEXT,
  file_path TEXT,
  created_by INTEGER,
  created_at TIMESTAMP
)
AS $$
BEGIN
  RETURN QUERY
  SELECT documents.*
  FROM documents
  WHERE documents.id = document_id
    AND (
      documents.created_by = user_id -- User created the document
      OR EXISTS ( -- User has been granted access
        SELECT 1 FROM document_access
        WHERE document_access.document_id = document_id
          AND (
            document_access.user_id = user_id
            OR document_access.group_id IN (
              SELECT group_id FROM user_groups WHERE user_id = user_id
            )
          )
      )
    );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE grant_document_access_to_user(
  document_id INTEGER,
  user_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO document_access(document_id, user_id)
  VALUES(document_id, user_id);
END;
$$;

-- Procedure to grant access to a group
CREATE OR REPLACE PROCEDURE grant_document_access_to_group(
  document_id INTEGER,
  group_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO document_access(document_id, group_id)
  VALUES(document_id, group_id);
END;
$$;
