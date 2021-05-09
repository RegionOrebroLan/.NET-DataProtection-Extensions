using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AzureStorage;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.DataProtection.Data;
using RegionOrebroLan.DataProtection.DependencyInjection.Extensions;
using RegionOrebroLan.DependencyInjection;
using RegionOrebroLan.Extensions;
using StackExchange.Redis;
using DataProtectionOptions = RegionOrebroLan.DataProtection.DependencyInjection.Configuration.DataProtectionOptions;

namespace IntegrationTests.DependencyInjection.Extensions
{
	[TestClass]
	public class ServiceCollectionExtensionTest
	{
		#region Fields

		private static readonly string _dataDirectoryPath = Path.Combine(Global.ProjectDirectoryPath, "Test-data");

		#endregion

		#region Properties

		protected internal virtual string DataDirectoryPath => _dataDirectoryPath;

		#endregion

		#region Methods

		[TestMethod]
		public async Task AddDataProtection_Azure_Certificate_Test()
		{
			await this.AddDataProtectionAzureTest(22, KeyProtectionKind.Certificate);
		}

		[TestMethod]
		public async Task AddDataProtection_Azure_Dpapi_Test()
		{
			await this.AddDataProtectionAzureTest(21, KeyProtectionKind.Dpapi);
		}

		[TestMethod]
		public async Task AddDataProtection_Azure_DpapiNg_Test()
		{
			await this.AddDataProtectionAzureTest(21, KeyProtectionKind.DpapiNg);
		}

		[TestMethod]
		public async Task AddDataProtection_Azure_Test()
		{
			await this.AddDataProtectionAzureTest(20, null);
		}

		[TestMethod]
		public async Task AddDataProtection_EmptyOptions_ApplicationDiscriminator_Test()
		{
			await this.AddDataProtectionTest(null, 19, null, true);
		}

		[TestMethod]
		public async Task AddDataProtection_EmptyOptions_Test()
		{
			await this.AddDataProtectionTest(null, 19, null);
		}

		[TestMethod]
		public async Task AddDataProtection_FileSystem_ApplicationDiscriminator_Test()
		{
			await this.AddDataProtectionTest(DataProtectionKind.FileSystem, 20, null, true);
		}

		[TestMethod]
		public async Task AddDataProtection_FileSystem_Certificate_Test()
		{
			await this.AddDataProtectionTest(DataProtectionKind.FileSystem, 22, KeyProtectionKind.Certificate);
		}

		[TestMethod]
		public async Task AddDataProtection_FileSystem_Dpapi_Test()
		{
			await this.AddDataProtectionTest(DataProtectionKind.FileSystem, 21, KeyProtectionKind.Dpapi);
		}

		[TestMethod]
		public async Task AddDataProtection_FileSystem_DpapiNg_Test()
		{
			await this.AddDataProtectionTest(DataProtectionKind.FileSystem, 21, KeyProtectionKind.DpapiNg);
		}

		[TestMethod]
		public async Task AddDataProtection_FileSystem_Test()
		{
			await this.AddDataProtectionTest(DataProtectionKind.FileSystem, 20, null);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_Configuration_Certificate_Test()
		{
			await this.AddDataProtectionRedisTest(false, 22, KeyProtectionKind.Certificate);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_Configuration_Dpapi_Test()
		{
			await this.AddDataProtectionRedisTest(false, 21, KeyProtectionKind.Dpapi);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_Configuration_DpapiNg_Test()
		{
			await this.AddDataProtectionRedisTest(false, 21, KeyProtectionKind.DpapiNg);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_Configuration_Test()
		{
			await this.AddDataProtectionRedisTest(false, 20, null);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_ConfigurationOptions_Certificate_Test()
		{
			await this.AddDataProtectionRedisTest(true, 22, KeyProtectionKind.Certificate);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_ConfigurationOptions_Dpapi_Test()
		{
			await this.AddDataProtectionRedisTest(true, 21, KeyProtectionKind.Dpapi);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_ConfigurationOptions_DpapiNg_Test()
		{
			await this.AddDataProtectionRedisTest(true, 21, KeyProtectionKind.DpapiNg);
		}

		[TestMethod]
		public async Task AddDataProtection_Redis_ConfigurationOptions_Test()
		{
			await this.AddDataProtectionRedisTest(true, 20, null);
		}

		[TestMethod]
		public async Task AddDataProtection_Sqlite_Certificate_Test()
		{
			await this.AddDataProtectionDatabaseTest(25, KeyProtectionKind.Certificate, true);
		}

		[TestMethod]
		public async Task AddDataProtection_Sqlite_Dpapi_Test()
		{
			await this.AddDataProtectionDatabaseTest(24, KeyProtectionKind.Dpapi, true);
		}

		[TestMethod]
		public async Task AddDataProtection_Sqlite_DpapiNg_Test()
		{
			await this.AddDataProtectionDatabaseTest(24, KeyProtectionKind.DpapiNg, true);
		}

		[TestMethod]
		public async Task AddDataProtection_Sqlite_Test()
		{
			await this.AddDataProtectionDatabaseTest(23, null, true);
		}

		[TestMethod]
		public async Task AddDataProtection_SqlServer_Certificate_Test()
		{
			await this.AddDataProtectionDatabaseTest(25, KeyProtectionKind.Certificate, false);
		}

		[TestMethod]
		public async Task AddDataProtection_SqlServer_Dpapi_Test()
		{
			await this.AddDataProtectionDatabaseTest(24, KeyProtectionKind.Dpapi, false);
		}

		[TestMethod]
		public async Task AddDataProtection_SqlServer_DpapiNg_Test()
		{
			await this.AddDataProtectionDatabaseTest(24, KeyProtectionKind.DpapiNg, false);
		}

		[TestMethod]
		public async Task AddDataProtection_SqlServer_Test()
		{
			await this.AddDataProtectionDatabaseTest(23, null, false);
		}

		protected internal virtual async Task AddDataProtectionAzureTest(int expectedNumberOfServices, KeyProtectionKind? keyProtectionKind)
		{
			try
			{
				//await this.AddDataProtectionTest(DataProtectionKind.Azure, expectedNumberOfServices, keyProtectionKind);
				await this.AddDataProtectionTest(DataProtectionKind.Azure, expectedNumberOfServices, keyProtectionKind, false, true);
			}
			catch(CryptographicException cryptographicException)
			{
				if(cryptographicException.InnerException is StorageException)
					Assert.Inconclusive($"You need to setup an Azure Storage account. Exception: {cryptographicException.InnerException}");
			}
		}

		protected internal virtual async Task AddDataProtectionDatabaseTest(int expectedNumberOfServices, KeyProtectionKind? keyProtectionKind, bool sqlite)
		{
			await this.AddDataProtectionTest(sqlite ? DataProtectionKind.Sqlite : DataProtectionKind.SqlServer, expectedNumberOfServices, keyProtectionKind);
		}

		protected internal virtual async Task AddDataProtectionRedisTest(bool configurationOptions, int expectedNumberOfServices, KeyProtectionKind? keyProtectionKind)
		{
			try
			{
				await this.AddDataProtectionTest(configurationOptions ? DataProtectionKind.RedisConfigurationOptions : DataProtectionKind.RedisConfiguration, expectedNumberOfServices, keyProtectionKind);
			}
			catch(InvalidOperationException invalidOperationException)
			{
				if(invalidOperationException.InnerException?.InnerException is RedisConnectionException)
					Assert.Inconclusive($"You need to setup a Redis. You can do it with docker: \"docker run --rm -it -p 6379:6379 redis\". Exception: {invalidOperationException.InnerException.InnerException}");
			}
		}

		protected internal virtual async Task AddDataProtectionTest(DataProtectionKind? dataProtectionKind, int expectedNumberOfServices, KeyProtectionKind? keyProtectionKind, bool applicationDiscriminator = false, bool skipProtectionTest = false)
		{
			var jsonFilePaths = new List<string>
			{
				"appsettings.json"
			};

			if(applicationDiscriminator)
				jsonFilePaths.Add($"appsettings.ApplicationDiscriminator.json");

			if(dataProtectionKind != null)
				jsonFilePaths.Add($"appsettings.{dataProtectionKind}.json");

			if(keyProtectionKind != null)
				jsonFilePaths.Add($"appsettings.Key-Protection.{keyProtectionKind}.json");

			var configuration = Global.CreateConfiguration(jsonFilePaths.ToArray());

			var services = Global.CreateServices(configuration);

			var numberOfServicesBefore = services.Count;

			services.AddDataProtection(Global.CreateCertificateResolver(), configuration, Global.HostEnvironment, new InstanceFactory());

			Assert.AreEqual(expectedNumberOfServices, services.Count - numberOfServicesBefore);

			await using(var serviceProvider = services.BuildServiceProvider())
			{
				var microsoftDataProtectionOptions = serviceProvider.GetRequiredService<IOptions<Microsoft.AspNetCore.DataProtection.DataProtectionOptions>>();
				var expectedApplicationDiscriminator = applicationDiscriminator ? "Test" : Global.ProjectDirectoryPath;
				Assert.AreEqual(expectedApplicationDiscriminator, microsoftDataProtectionOptions.Value.ApplicationDiscriminator);

				var dataProtectionOptions = serviceProvider.GetRequiredService<DataProtectionOptions>();

				dataProtectionOptions.Use(new ApplicationBuilder(serviceProvider));

				var keyManagementOptions = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>().Value;

				var expectedXmlEncryptorType = await this.GetXmlEncryptorTypeAsync(keyProtectionKind);
				Assert.AreEqual(expectedXmlEncryptorType, keyManagementOptions.XmlEncryptor?.GetType());

				var expectedXmlRepositoryType = await this.GetXmlRepositoryTypeAsync(dataProtectionKind);
				Assert.AreEqual(expectedXmlRepositoryType, keyManagementOptions.XmlRepository?.GetType());

				if(skipProtectionTest)
					return;

				const string value = "Test-value";
				var dataProtector = serviceProvider.GetRequiredService<IDataProtectionProvider>().CreateProtector("Test-purpose");
				var protectedValue = dataProtector.Protect(value);
				var unprotectedValue = dataProtector.Unprotect(protectedValue);
				Assert.AreEqual(value, unprotectedValue);
			}
		}

		protected internal virtual async Task<Type> GetXmlEncryptorTypeAsync(KeyProtectionKind? keyProtectionKind)
		{
			await Task.CompletedTask;

			return keyProtectionKind switch
			{
				KeyProtectionKind.Certificate => typeof(CertificateXmlEncryptor),
				KeyProtectionKind.Dpapi => typeof(DpapiXmlEncryptor),
				KeyProtectionKind.DpapiNg => typeof(DpapiNGXmlEncryptor),
				_ => null
			};
		}

		protected internal virtual async Task<Type> GetXmlRepositoryTypeAsync(DataProtectionKind? dataProtectionKind)
		{
			await Task.CompletedTask;

			return dataProtectionKind switch
			{
				DataProtectionKind.Azure => typeof(AzureBlobXmlRepository),
				DataProtectionKind.FileSystem => typeof(FileSystemXmlRepository),
				DataProtectionKind.RedisConfiguration => typeof(RedisXmlRepository),
				DataProtectionKind.RedisConfigurationOptions => typeof(RedisXmlRepository),
				DataProtectionKind.Sqlite => typeof(EntityFrameworkCoreXmlRepository<DataProtectionContext>),
				DataProtectionKind.SqlServer => typeof(EntityFrameworkCoreXmlRepository<DataProtectionContext>),
				_ => null
			};
		}

		[TestCleanup]
		public async Task TestCleanup()
		{
			var directoryPath = Path.Combine(this.DataDirectoryPath, "Keys");

			if(Directory.Exists(directoryPath))
				Directory.Delete(directoryPath, true);

			foreach(var environment in new[] {"Sqlite", "SqlServer"})
			{
				var configuration = Global.CreateConfiguration("appsettings.json", $"appsettings.{environment}.json");
				var services = Global.CreateServices(configuration);
				services.AddDataProtection(Global.CreateCertificateResolver(), configuration, Global.HostEnvironment, new InstanceFactory());

				// ReSharper disable UseAwaitUsing
				using(var serviceProvider = services.BuildServiceProvider())
				{
					using(var scope = serviceProvider.CreateScope())
					{
						var dataProtectionContext = scope.ServiceProvider.GetService<DataProtectionContext>();

						if(dataProtectionContext != null)
							await dataProtectionContext.Database.EnsureDeletedAsync();
					}
				}
				// ReSharper restore UseAwaitUsing
			}

			AppDomain.CurrentDomain.SetDataDirectory(null);
		}

		[TestInitialize]
		public async Task TestInitialize()
		{
			await Task.CompletedTask;

			AppDomain.CurrentDomain.SetDataDirectory(this.DataDirectoryPath);
		}

		#endregion
	}
}