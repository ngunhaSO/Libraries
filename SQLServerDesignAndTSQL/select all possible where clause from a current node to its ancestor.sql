
ALTER PROCEDURE [dbo].[spSSPSTTreeHierarchyWhereClause]
	@PlanID int,
	@GroupID int,
	@whereClause varchar(max) out

	--select all where clause associated from a current node up to its ancestor

AS
 create table #tmp1
 (
	ID int identity,
	subcategoryid int,
	facilitytypeid int,
	subcategory varchar(100),
	sourcecolumn varchar(100),
	result int
 )

 declare @parentgroupid int

 select @parentgroupid = ParentGroupID
 from SSPSTTreeHierarchy where PlanID = @PlanID and GroupID = @GroupID

 create table #tmp2 (
	ID int identity,
	groupname varchar(100),
	groupid int,
	parentgroupid int,
	levelnumber int,
	subcategoryid int,
	whereclause varchar(max),
	result int
 )

 insert into #tmp2(groupid, subcategoryid)
 select groupID, SubCategoryID from SSPSTFacilityGroupSubCategory where GroupID = @GroupID

 update #tmp2
 set groupname = (select groupname from SSPSTFacilityGroup where GroupID = @GroupID)

 update #tmp2
 set parentgroupid = (select parentgroupid from SSPSTTreeHierarchy where PlanID = @PlanID and GroupID = @GroupID)

 update #tmp2
 set levelnumber = (select levelnumber from SSPSTTreeHierarchy where PlanID = @PlanID and GroupID = @GroupID)

 update #tmp2
	set whereclause = (select replace(t.SourceColumn, '{0}', s.SubCatValue)
						from SSPSTFacilityTypeSubCategory s join SSPSTFacilityType t on t.FacilityTypeID = s.FacilityTypeID
						where SubCategoryID = #tmp2.subcategoryid)	

while @parentgroupid is not null
	begin
		create table #tmp3(
			ID int identity,
			groupname varchar(100),
			groupid int,
			parentgroupid int,
			levelnumber int,
			subcategoryid int,
			whereclause varchar(max),
			result int
		)

		insert into #tmp3(groupid, subcategoryid)
		select groupID, SubCategoryID from SSPSTFacilityGroupSubCategory where GroupID = @parentgroupid

		update #tmp3
		set groupname = (select groupname from SSPSTFacilityGroup where GroupID = @parentgroupid)

		update #tmp3 
		set parentgroupid = (select ParentGroupID from SSPSTTreeHierarchy where PlanID = @PlanID and GroupID = @parentgroupid)

		update #tmp3
		set levelnumber = (Select LevelNumber from SSPSTTreeHierarchy where PlanID = @PlanID and GroupID = @parentgroupid)

		update #tmp3
		set whereclause = (select replace(t.SourceColumn, '{0}', s.SubCatValue)
							from SSPSTFacilityTypeSubCategory s join SSPSTFacilityType t on s.FacilityTypeID = t.FacilityTypeID
							where SubCategoryID = #tmp3.subcategoryid)

		insert into #tmp2 (groupname ,
		groupid ,
		parentgroupid ,
		levelnumber ,
		subcategoryid ,
		whereclause ,
		result)
		select groupname ,
			groupid ,
			parentgroupid ,
			levelnumber ,
			subcategoryid ,
			whereclause ,
			result from #tmp3	

		drop table #tmp3

		set @parentgroupid = (select ParentGroupID from SSPSTTreeHierarchy where PlanID = @PlanID and GroupID = @parentgroupid) --raise the counter of the loop
end

---===== PERFORM TRANFORMATION ==========
		create table #transform(
			groupid int,
			whereclause varchar(max)
		)
		
		insert into #transform
		select main.groupid, '(' + left(main.whereClause, len(main.whereClause) - 2) + ')' as whereClause
		from (
				select distinct t2.groupid, (
							select t1.whereclause + ' OR '  as [text()]
							from #tmp2 t1
							where t1.groupid = t2.groupid
							order by t1.groupid
							for xml path('')
						) whereClause
				from #tmp2 t2
			) main

select @whereClause = (
	select left(main.whereclause, len(main.whereclause) - 3) as whereClause
	from (
		select distinct (
			select t1.whereclause + ' OR '  as [text()]
			from #transform t1
			order by t1.groupid
			for xml path('')) whereclause
		from #transform t2
	) main
)

