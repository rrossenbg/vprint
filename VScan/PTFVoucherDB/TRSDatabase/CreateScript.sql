 create procedure getPtfCountries
as
 begin
 SELECT iso_number, iso_2, iso_country
 FROM [ISO]
WHERE iso_ptf='Y';
end;

GO

alter procedure getSiteCodesByCountryId(@CountryID int)
as
begin
declare @txtCountryID as varchar(10);
set @txtCountryID = '%' + cast(@CountryID as varchar(10)) + '%';
select i.iso_shortcode, l.ll_code
from ISO i 
cross join LocationLookup l 
where iso_ptf='Y' and i.iso_number= @CountryID and l.ll_site like  @txtCountryID
order by i.iso_shortcode asc;
end;