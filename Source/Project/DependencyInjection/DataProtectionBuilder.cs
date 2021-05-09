using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegionOrebroLan.Configuration;
using RegionOrebroLan.DataProtection.Configuration;
using RegionOrebroLan.DataProtection.DependencyInjection.Configuration;
using RegionOrebroLan.DependencyInjection;
using RegionOrebroLan.Security.Cryptography;

namespace RegionOrebroLan.DataProtection.DependencyInjection
{
	public class DataProtectionBuilder : IDataProtectionBuilder
	{
		#region Constructors

		public DataProtectionBuilder(ICertificateResolver certificateResolver, IConfiguration configuration, IHostEnvironment hostEnvironment, IInstanceFactory instanceFactory, IServiceCollection services)
		{
			this.CertificateResolver = certificateResolver ?? throw new ArgumentNullException(nameof(certificateResolver));
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
			this.InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
			this.Services = services ?? throw new ArgumentNullException(nameof(services));
		}

		#endregion

		#region Properties

		public virtual ICertificateResolver CertificateResolver { get; }
		public virtual IConfiguration Configuration { get; }
		public virtual string ConfigurationKey { get; set; } = ConfigurationKeys.DataProtectionPath;
		public virtual IHostEnvironment HostEnvironment { get; }
		public virtual IInstanceFactory InstanceFactory { get; }
		public virtual IServiceCollection Services { get; }

		#endregion

		#region Methods

		public virtual void Configure()
		{
			try
			{
				var configurationSection = this.Configuration.GetSection(this.ConfigurationKey);

				var dynamicOptions = new DynamicOptions();
				configurationSection.Bind(dynamicOptions);

				var dataProtectionOptions = dynamicOptions.Type != null ? (DataProtectionOptions)this.InstanceFactory.Create(dynamicOptions.Type) : new EmptyOptions();

				configurationSection.Bind(dataProtectionOptions);

				this.Services.AddDataProtection(options =>
				{
					dataProtectionOptions.Options?.Bind(options);
				});

				dataProtectionOptions.Add(this);

				this.Services.AddSingleton(dataProtectionOptions);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Could not configure data-protection.", exception);
			}
		}

		#endregion
	}
}