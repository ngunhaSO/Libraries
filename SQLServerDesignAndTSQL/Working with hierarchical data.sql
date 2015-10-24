select  GroupID, GroupName
from SSPSTFacilityGroup
where GroupID IN (
SELECT c.GroupID
FROM SSPSTTreeHierarchy as c
LEFT JOIN SSPSTTreeHierarchy as p
on c.ParentGroupID = p.GroupID
WHERE c.PlanID = 100135)

--- regular join
SELECT c.GroupID, c.LevelNumber, c.ParentGroupID
FROM SSPSTTreeHierarchy as c
LEFT JOIN SSPSTTreeHierarchy as p
on c.ParentGroupID = p.GroupID
WHERE c.PlanID = 100135
ORDER BY c.LevelNumber, c.GroupID

---============select all leaves. If a node is a leaf, its GroupID should not be in the ParentGroupID list=====================
--- Note: the ParentGroupID list must be not null
SELECT * FROM SSPSTTreeHierarchy WHERE GroupID NOT IN (
    SELECT DISTINCT ParentGroupID FROM SSPSTTreeHierarchy WHERE ParentGroupID IS NOT NULL)
and PlanID = 100135

----========== Similar to above but display group name as extra column=================
SELECT SSPSTTreeHierarchy.GroupID, SSPSTFacilityGroup.GroupName
FROM SSPSTTreeHierarchy JOIN SSPSTFacilityGroup
ON SSPSTTreeHierarchy.GroupID = SSPSTFacilityGroup.GroupID
WHERE SSPSTTreeHierarchy.GroupID NOT IN (
    SELECT DISTINCT ParentGroupID FROM SSPSTTreeHierarchy WHERE ParentGroupID IS NOT NULL)
and PlanID = 100135

--- ====================== find all possible parents given a node id=======================================
declare @id bigint
set @id = 100192;
with tblParent as 
(
	select *
	from SSPSTTreeHierarchy WHERE GROUPID = @id
	UNION ALL
	Select SSPSTTreeHierarchy.*
	from SSPSTTreeHierarchy JOIN tblParent ON SSPSTTreeHierarchy.GroupID = tblParent.ParentGroupID
)
select * from tblParent
--WHERE GroupID <> @id --uncomment this will get all possible parents, including current groupid
ORDER BY LevelNumber
OPTION (Maxrecursion 32767)

----==================== Display all hierarchy tree ==============================:
;
With CTETreeHierarchy as		--step 3: cte for adding anchor and recursive query
(
---step 1: identify the anchor
select p.GroupID, p.LevelNumber, p.ParentGroupID
from SSPSTTreeHierarchy p
where p.ParentGroupID is null and p.PlanID = 100135

UNION ALL	--step 3: add anchor and recursive to CTE

--step 2: identify the recursive query
select c.GroupID, c.LevelNumber, c.ParentGroupID
from SSPSTTreeHierarchy c
where c.ParentGroupID is not null and c.PlanID = 100135
)
select * from CTETreeHierarchy order by LevelNumber, ParentGroupID


----================ Find all possible children =============================
Declare @groupid Bigint
Set @groupid = 100163;
with tblChild as
(
	SELECT *
	FROM SSPSTTreeHierarchy Where ParentGroupID = @groupid
	UNION ALL
	SELECT SSPSTTreeHierarchy.*
	FROM SSPSTTreeHierarchy Join tblChild On SSPSTTreeHierarchy.ParentGroupID = tblChild.GroupID
)
SELECT * FROM tblChild
OPTION(MaxRecursion 32767);


----====================== Display all possible parents in a column========================================
;With Hierarchy(ChildId, ParentID, Parents)
as
(
	SELECT GroupID, ParentGroupID, CAST('' AS VARCHAR(MAX))
	FROM SSPSTTreeHierarchy AS FirstGeneration
	WHERE ParentGroupID IS NULL AND PLANID = 100135
	UNION ALL
	SELECT NextGeneration.GroupID,  Parent.ChildId,
		CAST(CASE WHEN Parent.Parents = '' THEN (CAST(NextGeneration.ParentGroupID AS varchar(MAX)))
					ELSE (Parent.Parents + ' >> ' + CAST(NextGeneration.ParentGroupID AS VARCHAR(MAX)))
			END AS varchar(max))
	FROM SSPSTTreeHierarchy AS NextGeneration
		INNER JOIN Hierarchy AS Parent
		ON NextGeneration.ParentGroupID = Parent.ChildId
	WHERE PLANID = 100135
)
SELECT * FROM Hierarchy
OPTION(MaxRecursion 32767)


----==========================WORKING WITH REAL APPLICATION Safety Project====================================================================
;With Hierarchy(ChildId, ParentID, Parents)
as
(
	SELECT GroupID, ParentGroupID, CAST('' AS VARCHAR(MAX))
	FROM SSPSTTreeHierarchy AS FirstGeneration
	WHERE ParentGroupID IS NULL AND PLANID = 100135
	UNION ALL
	SELECT NextGeneration.GroupID,  Parent.ChildId,
		CAST(CASE WHEN Parent.Parents = '' THEN (CAST(NextGeneration.ParentGroupID AS varchar(MAX)))
					ELSE (Parent.Parents + ' >> ' + CAST(NextGeneration.ParentGroupID AS VARCHAR(MAX)))
			END AS varchar(max))
	FROM SSPSTTreeHierarchy AS NextGeneration
		INNER JOIN Hierarchy AS Parent
		ON NextGeneration.ParentGroupID = Parent.ChildId
	WHERE PLANID = 100135
)
SELECT * FROM Hierarchy
OPTION(MaxRecursion 32767)



---===============================================================================================
With Hierarchy(GroupID, ParentGroupID, Parents)
as
(
	SELECT GroupID, ParentGroupID, CAST('' AS VARCHAR(MAX))
	FROM SSPSTTreeHierarchy AS FirstGeneration
	WHERE ParentGroupID IS NULL AND PLANID = 100135
	UNION ALL
	SELECT NextGeneration.GroupID,  Parent.GroupID,
		CAST(CASE WHEN Parent.Parents = '' THEN (select groupname from SSPSTFacilityGroup where groupid = nextGeneration.ParentGroupID)
					ELSE (Parent.Parents + ' >> ' + (select groupname from SSPSTFacilityGroup where groupid = nextGeneration.ParentGroupID))
			END AS varchar(max))
	FROM SSPSTTreeHierarchy AS NextGeneration
		INNER JOIN Hierarchy AS Parent
		ON NextGeneration.ParentGroupID = Parent.GroupID
	WHERE PLANID = 100135
)
SELECT * FROM Hierarchy WHERE GroupID NOT IN (
		SELECT DISTINCT ParentGroupID FROM Hierarchy WHERE ParentGroupID IS NOT NULL)
OPTION(MaxRecursion 32767)

---===============================================================================================
select  GroupID, GroupName
from SSPSTFacilityGroup
where GroupID IN (
SELECT c.GroupID
FROM SSPSTTreeHierarchy as c
LEFT JOIN SSPSTTreeHierarchy as p
on c.ParentGroupID = p.GroupID
WHERE c.PlanID = 100135)


SELECT * FROM SSPSTTreeHierarchy WHERE GroupID NOT IN (
    SELECT DISTINCT ParentGroupID FROM SSPSTTreeHierarchy WHERE ParentGroupID IS NOT NULL)
and PlanID = 100135
;
---select all leaves. If a node is a leaf, its GroupID should not be in the ParentGroupID list
--- Note: the ParentGroupID list must be not null
CREATE Table #LeavesTable(
	TreeID int,
	PlanID int,
	GroupID int,
	ParentGroupID int,
	LevelNumber int
)
INSERT INTO #LeavesTable
SELECT * FROM SSPSTTreeHierarchy WHERE GroupID NOT IN (
    SELECT DISTINCT ParentGroupID FROM SSPSTTreeHierarchy WHERE ParentGroupID IS NOT NULL)
and PlanID = 100135
;
---- Display all possible parents in a column along with group name
With Hierarchy(ChildId, ParentID, Parents)
as
(
	SELECT GroupID, ParentGroupID, CAST('' AS VARCHAR(MAX))
	FROM SSPSTTreeHierarchy AS FirstGeneration
	WHERE ParentGroupID IS NULL AND PLANID = 100135
	UNION ALL
	SELECT NextGeneration.GroupID,  Parent.ChildId,
		CAST(CASE WHEN Parent.Parents = '' THEN (CAST(NextGeneration.ParentGroupID AS varchar(MAX)))
					ELSE (Parent.Parents + ' >> ' + CAST(NextGeneration.ParentGroupID AS VARCHAR(MAX)))
			END AS varchar(max))
	FROM SSPSTTreeHierarchy AS NextGeneration
		INNER JOIN Hierarchy AS Parent
		ON NextGeneration.ParentGroupID = Parent.ChildId
	WHERE PLANID = 100135
)
SELECT Hierarchy.ChildId,Hierarchy.ParentID FROM Hierarchy
JOIN #LeavesTable On
#LeavesTable.GroupID = Hierarchy.ChildId
OPTION(MaxRecursion 32767)

drop table #LeavesTable