##Ǩ��  
Add-Migration InitialCreate -Context CustomConfigurationDbContext -OutputDir Migrations\Configuration\Mysql  
Update-Database -context CustomConfigurationDbContext

Add-Migration InitialCreate -Context CustomPersistedGrantDbContext -OutputDir Migrations\PersistedGrant\Mysql  
Update-Database -context CustomPersistedGrantDbContext

Add-Migration InitialCreate -Context ApplicationDbContext -OutputDir Migrations\Application\Mysql  
Update-Database -context ApplicationDbContext

##�����������  
dotnet run seed