using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegionOrebroLan.Configuration;
using RegionOrebroLan.DataProtection.Configuration;
using RegionOrebroLan.DependencyInjection;
using RegionOrebroLan.Security.Cryptography;

namespace RegionOrebroLan.DataProtection.DependencyInjection
{
	public class ExtendedDataProtectionBuilder : IDataProtectionBuilder
	{
		#region Constructors

		public ExtendedDataProtectionBuilder(IServiceCollection services)
		{
			this.Services = services ?? throw new ArgumentNullException(nameof(services));
		}

		#endregion

		#region Properties

		public virtual ICertificateResolver CertificateResolver { get; set; }
		public virtual IConfiguration Configuration { get; set; }
		public virtual string ConfigurationKey { get; set; } = ConfigurationKeys.DataProtectionPath;
		public virtual IHostEnvironment HostEnvironment { get; set; }
		public virtual IInstanceFactory InstanceFactory { get; set; }
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

				ExtendedDataProtectionOptions dataProtectionOptions = new DefaultOptions();

				this.Services.AddDataProtection(options => { configurationSection.Bind(options); });

				if(dynamicOptions.Type != null)
				{
					dataProtectionOptions = (ExtendedDataProtectionOptions)this.InstanceFactory.Create(dynamicOptions.Type);

					configurationSection.Bind(dataProtectionOptions);
					dynamicOptions.Options.Bind(dataProtectionOptions);
				}

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