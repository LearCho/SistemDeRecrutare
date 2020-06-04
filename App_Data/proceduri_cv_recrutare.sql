/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [id_cv]
      ,[nume_fisier]
      ,[continut_fisier]
  FROM [DB_sistem_recrutare].[dbo].[cv]

    alter table dbo.cv 
  add id_cv int identity(1,1) NOT NULL
  primary key(id_cv);

  drop procedure dbo.adauga_detalii_cv;
  create procedure dbo.adauga_detalii_cv 
(  
@nume_fisier varchar(128),  
@continut_fisier varBinary(max)  )  
as begin 
set nocount on
Insert into dbo.cv values(@nume_fisier, @continut_fisier)   
End 

CREATE Procedure dbo.vezi_detalii_cv
(  @id_cv int=null    )  
as  begin    
select id_cv,nume_fisier, continut_fisier from dbo.cv  
where id_cv = isnull(@id_cv, id_cv)  
End