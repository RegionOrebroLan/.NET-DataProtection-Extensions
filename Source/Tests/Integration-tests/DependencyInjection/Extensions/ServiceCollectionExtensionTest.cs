using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.DataProtection.Configuration;
using RegionOrebroLan.DataProtection.Data;
using RegionOrebroLan.DataProtection.DependencyInjection.Extensions;
using RegionOrebroLan.DependencyInjection;
using RegionOrebroLan.Extensions;

namespace IntegrationTests.DependencyInjection.Extensions
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

		protected internal virtual void AddDataProtection_Database_Test(string environment)
		{
			var configuration = Global.CreateConfiguration("appsettings.json", $"appsettings.{environment}.json");

			var services = Global.CreateServices(configuration);

			var numberOfServicesBefore = services.Count;

			services.AddDataProtection(Global.CreateCertificateResolver(), configuration, Global.HostEnvironment, new InstanceFactory());

			Assert.AreEqual(25, services.Count - numberOfServicesBefore);

			using(var serviceProvider = services.BuildServiceProvider())
			{
				var dataProtectionOptions = serviceProvider.GetRequiredService<ExtendedDataProtectionOptions>();

				dataProtectionOptions.Use(new ApplicationBuilder(serviceProvider));

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
			}
		}

		[TestMethod]
		public void AddDataProtection_DefaultOptions_Test()
		{
			var configuration = Global.CreateConfiguration("appsettings.json");

			var services = Global.CreateServices(configuration);

			var numberOfServicesBefore = services.Count;

			services.AddDataProtection(Global.CreateCertificateResolver(), configuration, Global.HostEnvironment, new InstanceFactory());

			Assert.AreEqual(19, services.Count - numberOfServicesBefore);

			using(var serviceProvider = services.BuildServiceProvider())
			{
				var dataProtectionOptions = serviceProvider.GetRequiredService<ExtendedDataProtectionOptions>();

				dataProtectionOptions.Use(new ApplicationBuilder(serviceProvider));

				var keyManagementOptions = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>();

				Assert.IsNull(keyManagementOptions.Value.XmlEncryptor);
				Assert.IsNull(keyManagementOptions.Value.XmlRepository);

				const string value = "Test";
				var dataProtector = serviceProvider.GetRequiredService<IDataProtectionProvider>().CreateProtector("Test");
				var protectedValue = dataProtector.Protect(value);
				var unprotectedValue = dataProtector.Unprotect(protectedValue);
				Assert.AreEqual(value, unprotectedValue);

				var defaultOptions = (DefaultOptions)dataProtectionOptions;
				Assert.IsNull(defaultOptions.KeyProtection);
			}
		}

		[TestMethod]
		public void AddDataProtection_FileSystem_Test()
		{
			var configuration = Global.CreateConfiguration("appsettings.json", "appsettings.FileSystem.json");

			var services = Global.CreateServices(configuration);

			var numberOfServicesBefore = services.Count;

			services.AddDataProtection(Global.CreateCertificateResolver(), configuration, Global.HostEnvironment, new InstanceFactory());

			Assert.AreEqual(21, services.Count - numberOfServicesBefore);

			using(var serviceProvider = services.BuildServiceProvider())
			{
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
			}
		}

		[TestMethod]
		public void AddDataProtection_Sqlite_Test()
		{
			this.AddDataProtection_Database_Test("Sqlite");
		}

		[TestMethod]
		public void AddDataProtection_SqlServer_Test()
		{
			this.AddDataProtection_Database_Test("SqlServer");
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