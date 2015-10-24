
select t.GroupID, t.LevelNumber, t.ParentGroupID, g.GroupName
from SSPSTTreeHierarchy t
join SSPSTFacilityGroup g
on t.GroupID = g.GroupID
where t.PlanID = 100135
order by t.LevelNumber, t.ParentGroupID

/*
	Step 1: identify the anchor query which basically select the top level of the tree hierarchy
	Step 2: identify the recursive query which select the rest of the tree hierarchy
	Step 3: add the anchor and recursive query to CTE

*/


;With CTETreeHierarchy as		--step 3: cte for adding anchor and recursive query
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

---============= Display full hierarchy tree ============================
---============= this is similar to the one above it but it reference back to the cte instead of the table itself

;with CTEParents as
(
select p.GroupID, p.LevelNumber, p.ParentGroupID
from SSPSTTreeHierarchy p
where p.PlanID = 100135 and p.ParentGroupID is null
UNION ALL

select c.GroupID, c.LevelNumber, c.ParentGroupID
from SSPSTTreeHierarchy c
inner join CTEParents cp
on c.ParentGroupID = cp.GroupID
where c.PlanID = 100135
)
select * from CTEParents
order by LevelNumber, ParentGroupID


--=============== SELECT ALL LEAVES =========================
select c.GroupID, c.LevelNumber, c.ParentGroupID
from SSPSTTreeHierarchy c
where c.GroupID not in (select p.ParentGroupID from SSPSTTreeHierarchy p where p.ParentGroupID is not null)
and c.PlanID = 100135
order by c.LevelNumber, c.ParentGroupID

--============== SELECT ALL LEAVES AND DISPLAY GROUP NAME ASSOCIATED WITH IT =================
select c.GroupID, c.LevelNumber, c.ParentGroupID, g.GroupName
from SSPSTTreeHierarchy c
inner join SSPSTFacilityGroup g
on c.GroupID = g.GroupID
where c.GroupID not in (select p.ParentGroupID from SSPSTTreeHierarchy p where p.ParentGroupID is not null)
and c.PlanID = 100135
order by c.LevelNumber, c.ParentGroupID

