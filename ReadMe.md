# .NET-DataProtection-Extensions

Additions and extensions for .NET data-protection (ASP.NET Core).

[![NuGet](https://img.shields.io/nuget/v/RegionOrebroLan.DataProtection.svg?label=NuGet)](https://www.nuget.org/packages/RegionOrebroLan.DataProtection)

## Features

- Configuring data-protection through AppSettings.json

## Development

### Data-protection with Azure

- Haven't got it yet how the url should be entered.

### Data-protection with database (EF)

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

### Data-protection with Redis

- [Quickstart: Use Azure Cache for Redis with a .NET Framework application](https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/cache-dotnet-how-to-use-azure-redis-cache/)