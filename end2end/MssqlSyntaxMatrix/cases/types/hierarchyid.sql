-- @description `HIERARCHYID` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_hierarchyid (v HIERARCHYID NULL, nn HIERARCHYID NOT NULL);
-- @queries
-- name: SelectHierarchyid :many
SELECT v, nn FROM t_hierarchyid;

-- name: InsertHierarchyid :exec
INSERT INTO t_hierarchyid (v, nn) VALUES (@v, @nn);

-- name: FilterHierarchyid :many
SELECT v FROM t_hierarchyid WHERE nn = @nn;
