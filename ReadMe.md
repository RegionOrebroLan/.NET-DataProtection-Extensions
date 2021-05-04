# .NET-DataProtection-Extensions

Additions and extensions for .NET data-protection (ASP.NET Core).

[![NuGet](https://img.shields.io/nuget/v/RegionOrebroLan.DataProtection.svg?label=NuGet)](https://www.nuget.org/packages/RegionOrebroLan.DataProtection)

## 1 Features

- Configuring data-protection through AppSettings.json

## 2 Development

### 2.1 Data-protection with Azure

- Haven't got it yet how the url should be entered.

### 2.2 Data-protection with database (EF)










#### 2.2.1 Migrations (DataProtectionContext)

We might want to create/recreate migrations. If we can accept data-loss we can recreate the migrations otherwhise we will have to update them.

Copy all the commands below and run them in the Package Manager Console for the affected database-context.

If you want more migration-information you can add the -Verbose parameter:

	Add-Migration TheMigration -Context TheDatabaseContext -OutputDir Data/Migrations -Project Project -Verbose;

**Important!** Before running the commands below you need to ensure the "Project"-project is set as startup-project. 

##### 2.1.1.1 Create migrations

	Write-Host "Removing migrations...";
	Remove-Migration -Context SqlServerDataProtectionContext -Force -Project Project;
	Write-Host "Removing current migrations-directory...";
	Remove-Item "Project\Data\Migrations" -ErrorAction Ignore -Force -Recurse;
	Write-Host "Creating migrations...";
	Add-Migration SqlServerDataProtectionContextMigration -Context SqlServerDataProtectionContext -OutputDir Data/Migrations/SqlServer -Project Project;
	Write-Host "Finnished";

##### 2.1.1.2 Update migrations

	Write-Host "Updating migrations...";
	Add-Migration SqlServerCacheContextMigrationUpdate -Context SqlServerDataProtectionContext -OutputDir Data/Migrations/SqlServer -Project Project;
	Write-Host "Finnished";




















#### Create migrations

We might want to create/recreate migrations. If we can accept data-loss we can recreate the migrations otherwhise we will have to update them.

If we want to create/recreate migrations we should first delete all exising classes under **RegionOrebroLan.DataProtection.Data.Migrations**. Delete the whole directory [Project/Data/Migrations](/Source/Project/Data/Migrations).

Copy all the commands below and run them in the Package Manager Console.

	Remove-Migration -Context DataProtectionKeyDbContext -Force;
	Write-Host "Removing current migrations-directory...";
	Remove-Item "Project\Data\Migrations" -ErrorAction Ignore -Force -Recurse;
	Write-Host "Creating migrations...";
	Add-Migration Create -Context DataProtectionKeyDbContext -OutputDir Data\Migrations;
	Write-Host "Finnished";

If you want more migration-information you can add the -Verbose parameter:

	Add-Migration Create -Context DataProtectionKeyDbContext -OutputDir Data\Migrations -Verbose

#### Update migrations

Copy all the commands below and run them in the Package Manager Console.

	Write-Host "Updating migrations...";
	Add-Migration Update -Context DataProtectionKeyDbContext -OutputDir Data\Migrations;
	Write-Host "Finnished";

### 2.3 Data-protection with Redis

- [Quickstart: Use Azure Cache for Redis with a .NET Framework application](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/cache-dotnet-how-to-use-azure-redis-cache/)