using System;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RegionOrebroLan.Data;
using RegionOrebroLan.Data.Common;
using RegionOrebroLan.Data.Extensions;
using RegionOrebroLan.DataProtection.Data;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public class DatabaseOptions : ExtendedDataProtectionOptions
	{
		#region Properties

		public virtual string ConnectionStringName { get; set; } = "Data-Protection";

		/// <summary>
		/// If the database should be created if it does not exist.
		/// </summary>
		public virtual bool Create { get; set; }

		public virtual bool EntryAssemblyAsMigrationAssembly { get; set; }
		public virtual string ProviderName { get; set; } = "System.Data.SqlClient";

		#endregion

		#region Methods

		public override void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				builder.Services.AddDbContext<DataProtectionKeyDbContext>(optionsBuilder =>
					optionsBuilder.UseSqlServer(
						builder.Configuration.GetConnectionString(this.ConnectionStringName),
						options => { options.MigrationsAssembly(this.GetMigrationAssembly().GetName().Name); }
					));

				builder.PersistKeysToDbContext<DataProtectionKeyDbContext>();

				builder.Services.TryAddSingleton(AppDomain.CurrentDomain);
				builder.Services.TryAddSingleton<IApplicationDomain, ApplicationHost>();
				builder.Services.TryAddSingleton<IConnectionStringBuilderFactory, ConnectionStringBuilderFactory>();
				builder.Services.TryAddSingleton<IDatabaseManagerFactory, DatabaseManagerFactory>();
				builder.Services.TryAddSingleton<IFileSystem, FileSystem>();
				builder.Services.TryAddSingleton<IProviderFactories, DbProviderFactoriesWrapper>();

				base.Add(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add data-protection by database for options of type \"{this.GetType()}\".", exception);
			}
		}

		protected internal virtual Assembly GetMigrationAssembly()
		{
			return this.EntryAssemblyAsMigrationAssembly ? Assembly.GetEntryAssembly() : this.GetType().Assembly;
		}

		public override void Use(IApplicationBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				var serviceProvider = builder.ApplicationServices;

				if(this.Create)
				{
					var configuration = serviceProvider.GetRequiredService<IConfiguration>();
					var databaseManagerFactory = serviceProvider.GetRequiredService<IDatabaseManagerFactory>();
					var databaseManager = databaseManagerFactory.Create(this.ProviderName);
					databaseManager.CreateDatabaseIfItDoesNotExist(configuration.GetConnectionString(this.ConnectionStringName));
				}

				// ReSharper disable ConvertToUsingDeclaration
				using(var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<DataProtectionKeyDbContext>();
					configurationDbContext.Database.Migrate();
				}
				// ReSharper restore ConvertToUsingDeclaration
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not use data-protection by database for options of type \"{this.GetType()}\".", exception);
			}
		}

		#endregion
	}
}