CREATE OR REPLACE PROCEDURE document_insert(
  p_name VARCHAR(255),
  p_description TEXT,
  p_file_path TEXT,
  p_created_by INTEGER,
  p_category VARCHAR(20)
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO documents (name, description, file_path, created_by, category)
  VALUES (p_name, p_description, p_file_path, p_created_by, p_category);
END;
$$;

CREATE OR REPLACE PROCEDURE get_document_by_id_for_user(
  IN p_document_id INTEGER,
  IN p_user_id INTEGER,
  OUT p_id INTEGER,
  OUT p_name VARCHAR(255),
  OUT p_description TEXT,
  OUT p_file_path TEXT,
  OUT p_created_by INTEGER,
  OUT p_created_at TIMESTAMP
)
AS $$
BEGIN
  SELECT documents.*
  INTO p_id, p_name, p_description, p_file_path, p_created_by, p_created_at
  FROM documents
  WHERE documents.id = p_document_id
    AND (
      documents.created_by = p_user_id
      OR EXISTS (
        SELECT 1 FROM document_access
        WHERE document_access.document_id = p_document_id
          AND (
            document_access.user_id = p_user_id
            OR document_access.group_id IN (
              SELECT group_id FROM user_groups WHERE user_id = p_user_id
            )
          )
      )
    )
    LIMIT 1;
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


CREATE OR REPLACE FUNCTION public.get_documents_by_user_id(
	p_user_id integer)
    RETURNS TABLE(id integer, name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000
LANGUAGE plpgsql
AS $$
BEGIN
  RETURN QUERY SELECT documents.id, documents.name
  FROM documents
  LEFT JOIN document_access ON documents.id = document_access.document_id
  LEFT JOIN group_members ON group_members.group_id = document_access.group_id
  WHERE documents.created_by = p_user_id
  OR document_access.user_id = p_user_id
  OR group_members.user_id = p_user_id;
END;
$$;
