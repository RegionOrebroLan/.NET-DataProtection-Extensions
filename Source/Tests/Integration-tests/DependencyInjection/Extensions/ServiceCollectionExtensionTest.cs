using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Data;
using RegionOrebroLan.Data.Common;
using RegionOrebroLan.DataProtection.Configuration;
using RegionOrebroLan.DataProtection.DependencyInjection.Extensions;
using RegionOrebroLan.DependencyInjection;

namespace RegionOrebroLan.DataProtection.IntegrationTests.DependencyInjection.Extensions
{
	[TestClass]
	public class ServiceCollectionExtensionTest
	{
		#region Fields

		private static readonly string _dataDirectoryPath = Path.Combine(Global.ProjectDirectoryPath, "Data");

		#endregion

		#region Properties

		protected internal virtual string DataDirectoryPath => _dataDirectoryPath;

		#endregion

		#region Methods

		[TestMethod]
		public void AddDataProtection_FileSystem_Test()
		{
			this.FileSystemCleanup();

			var configuration = Global.CreateConfiguration("appsettings.json", "appsettings.FileSystem.json");

			var services = Global.CreateServices(configuration);

			var numberOfServicesBefore = services.Count;

			services.AddDataProtection(Global.CreateCertificateResolver(), configuration, Global.HostEnvironment, new InstanceFactory());

			Assert.AreEqual(21, services.Count - numberOfServicesBefore);

			var serviceProvider = services.BuildServiceProvider();

			var dataProtectionOptions = serviceProvider.GetRequiredService<ExtendedDataProtectionOptions>();

			dataProtectionOptions.Use(new ApplicationBuilder(serviceProvider));

			var keyManagementOptions = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>();

			var xmlRepositoryType = keyManagementOptions.Value.XmlRepository.GetType();
			Assert.AreEqual(typeof(FileSystemXmlRepository), xmlRepositoryType);

			var xmlEncryptorType = keyManagementOptions.Value.XmlEncryptor.GetType();
			Assert.AreEqual(typeof(DpapiNGXmlEncryptor), xmlEncryptorType);

			const string value = "Test";
			var dataProtector = serviceProvider.GetRequiredService<IDataProtectionProvider>().CreateProtector("Test");
			var protectedValue = dataProtector.Protect(value);
			var unprotectedValue = dataProtector.Unprotect(protectedValue);
			Assert.AreEqual(value, unprotectedValue);

			var fileSystemOptions = (FileSystemOptions)dataProtectionOptions;
			var directoryPath = Path.Combine(Global.ProjectDirectoryPath, fileSystemOptions.Path);
			Assert.IsTrue(Directory.Exists(directoryPath));

			this.FileSystemCleanup();

			Assert.IsFalse(Directory.Exists(directoryPath));
		}

		[TestMethod]
		public void AddDataProtection_SqlServer_Test()
		{
			this.DatabaseCleanup();

			AppDomain.CurrentDomain.SetData("DataDirectory", this.DataDirectoryPath);

			var configuration = Global.CreateConfiguration("appsettings.json", "appsettings.SqlServer.json");

			var services = Global.CreateServices(configuration);

			var numberOfServicesBefore = services.Count;

			services.AddDataProtection(Global.CreateCertificateResolver(), configuration, Global.HostEnvironment, new InstanceFactory());

			Assert.AreEqual(31, services.Count - numberOfServicesBefore);

			var serviceProvider = services.BuildServiceProvider();

			var dataProtectionOptions = serviceProvider.GetRequiredService<ExtendedDataProtectionOptions>();

			dataProtectionOptions.Use(new ApplicationBuilder(serviceProvider));

			Thread.Sleep(1000);

			var databaseOptions = (DatabaseOptions)dataProtectionOptions;

			var databaseManager = serviceProvider.GetRequiredService<IDatabaseManagerFactory>().Create(databaseOptions.ProviderName);

			Assert.IsTrue(databaseManager.DatabaseExists(configuration.GetConnectionString(databaseOptions.ConnectionStringName)));

			var keyManagementOptions = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>();

			var genericXmlRepositoryType = keyManagementOptions.Value.XmlRepository.GetType().GetGenericTypeDefinition();
			Assert.AreEqual(typeof(EntityFrameworkCoreXmlRepository<>), genericXmlRepositoryType);

			var xmlEncryptorType = keyManagementOptions.Value.XmlEncryptor.GetType();
			Assert.AreEqual(typeof(CertificateXmlEncryptor), xmlEncryptorType);

			const string value = "Test";
			var dataProtector = serviceProvider.GetRequiredService<IDataProtectionProvider>().CreateProtector("Test");
			var protectedValue = dataProtector.Protect(value);
			var unprotectedValue = dataProtector.Unprotect(protectedValue);
			Assert.AreEqual(value, unprotectedValue);

			AppDomain.CurrentDomain.SetData("DataDirectory", null);

			this.DatabaseCleanup();
		}

		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		protected internal virtual void DatabaseCleanup()
		{
			var applicationDomain = new ApplicationHost(AppDomain.CurrentDomain, Global.HostEnvironment);
			var databaseNames = new List<string>();
			const string deleteDatabaseConnectionString = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;MultipleActiveResultSets=True";
			const string deleteDatabaseProviderName = "System.Data.SqlClient";
			var fileSystem = new FileSystem();
			var providerFactories = new DbProviderFactoriesWrapper();

			var connectionStringBuilderFactory = new ConnectionStringBuilderFactory(providerFactories);
			var connectionStringBuilder = connectionStringBuilderFactory.Create(deleteDatabaseConnectionString, deleteDatabaseProviderName);
			var databaseManagerFactory = new DatabaseManagerFactory(applicationDomain, connectionStringBuilderFactory, fileSystem, providerFactories);
			var databaseManager = databaseManagerFactory.Create(deleteDatabaseProviderName);
			var dbProviderFactory = providerFactories.Get(deleteDatabaseProviderName);

			using(var connection = dbProviderFactory.CreateConnection())
			{
				// ReSharper disable PossibleNullReferenceException
				connection.ConnectionString = connectionStringBuilder.ConnectionString;
				// ReSharper restore PossibleNullReferenceException
				connection.Open();

				using(var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT name FROM master.sys.databases;";
					command.CommandType = CommandType.Text;

					using(var reader = command.ExecuteReader())
					{
						while(reader.Read())
						{
							databaseNames.Add(reader.GetString(0));
						}
					}
				}
			}

			foreach(var databaseName in databaseNames.Where(name => name.StartsWith(this.DataDirectoryPath, StringComparison.OrdinalIgnoreCase)))
			{
				connectionStringBuilder.DatabaseFilePath = databaseName;

				// ReSharper disable EmptyGeneralCatchClause
				try
				{
					databaseManager.DropDatabase(connectionStringBuilder.ConnectionString);
				}
				catch { }
				// ReSharper restore EmptyGeneralCatchClause
			}
		}

		protected internal virtual void FileSystemCleanup()
		{
			var directoryPath = Path.Combine(this.DataDirectoryPath, @"Keys\_");

			if(Directory.Exists(directoryPath))
				Directory.Delete(directoryPath, true);

			Thread.Sleep(500);
		}

		#endregion
	}
}