# .NET-DataProtection-Extensions

Additions and extensions for .NET data-protection (ASP.NET Core).

[![NuGet](https://img.shields.io/nuget/v/RegionOrebroLan.DataProtection.svg?label=NuGet)](https://www.nuget.org/packages/RegionOrebroLan.DataProtection)

## 1 Features

- Configuring data-protection through AppSettings.json

### 1.1 Examples

- [Azure](/Source/Sample/Application/appsettings.Azure.json) - the url is just an example
- [FileSystem](/Source/Sample/Application/appsettings.FileSystem.json)
- [Redis](/Source/Sample/Application/appsettings.Redis.json) - you need to setup Redis, see below
- [Sqlite](/Source/Sample/Application/appsettings.Sqlite.json)
- [SqlServer](/Source/Sample/Application/appsettings.SqlServer.json)

#### 1.1.1 Redis

Setup Redis locally with Docker:

	docker run --rm -it -p 6379:6379 redis

## 2 Development

### 2.1 Data-protection with Azure

- Haven't got it yet how the url should be composed.

### 2.2 Data-protection with database (EF)

#### 2.2.1 Migrations (DataProtectionContext)

We might want to create/recreate migrations. If we can accept data-loss we can recreate the migrations otherwhise we will have to update them.

Copy all the commands below and run them in the Package Manager Console for the affected database-context.

If you want more migration-information you can add the -Verbose parameter:

	Add-Migration TheMigration -Context TheDatabaseContext -OutputDir Data/Migrations -Project Project -Verbose;

**Important!** Before running the commands below you need to ensure the "Project"-project is set as startup-project. 

##### 2.1.1.1 Create migrations

	Write-Host "Removing migrations...";
	Remove-Migration -Context SqliteDataProtectionContext -Force -Project Project;
	Remove-Migration -Context SqlServerDataProtectionContext -Force -Project Project;
	Write-Host "Removing current migrations-directory...";
	Remove-Item "Project\Data\Migrations" -ErrorAction Ignore -Force -Recurse;
	Write-Host "Creating migrations...";
	Add-Migration SqliteDataProtectionContextMigration -Context SqliteDataProtectionContext -OutputDir Data/Migrations/Sqlite -Project Project;
	Add-Migration SqlServerDataProtectionContextMigration -Context SqlServerDataProtectionContext -OutputDir Data/Migrations/SqlServer -Project Project;
	Write-Host "Finnished";

##### 2.1.1.2 Update migrations

	Write-Host "Updating migrations...";
	Add-Migration SqliteCacheContextMigrationUpdate -Context SqliteDataProtectionContext -OutputDir Data/Migrations/Sqlite -Project Project;
	Add-Migration SqlServerCacheContextMigrationUpdate -Context SqlServerDataProtectionContext -OutputDir Data/Migrations/SqlServer -Project Project;
	Write-Host "Finnished";

### 2.3 Data-protection with Redis

- [Quickstart: Use Azure Cache for Redis with a .NET Framework application](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/cache-dotnet-how-to-use-azure-redis-cache/)