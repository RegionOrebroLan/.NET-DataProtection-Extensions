using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegionOrebroLan.DataProtection.Configuration;
using RegionOrebroLan.DependencyInjection;
using RegionOrebroLan.Security.Cryptography;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Extensions
{
	public static class ServiceCollectionExtension
	{
		#region Methods

		public static IDataProtectionBuilder AddDataProtection(this IServiceCollection services, ICertificateResolver certificateResolver, IConfiguration configuration, IHostEnvironment hostEnvironment, IInstanceFactory instanceFactory)
		{
			return services.AddDataProtection(certificateResolver, configuration, ConfigurationKeys.DataProtectionPath, hostEnvironment, instanceFactory);
		}

		public static IDataProtectionBuilder AddDataProtection(this IServiceCollection services, ICertificateResolver certificateResolver, IConfiguration configuration, string configurationKey, IHostEnvironment hostEnvironment, IInstanceFactory instanceFactory)
		{
			return services.AddDataProtection(certificateResolver, configuration, configurationKey, hostEnvironment, instanceFactory, _ => { });
		}

		public static IDataProtectionBuilder AddDataProtection(this IServiceCollection services, ICertificateResolver certificateResolver, IConfiguration configuration, IHostEnvironment hostEnvironment, IInstanceFactory instanceFactory, Action<DataProtectionOptions> postConfigureOptions)
		{
			return services.AddDataProtection(certificateResolver, configuration, ConfigurationKeys.DataProtectionPath, hostEnvironment, instanceFactory, postConfigureOptions);
		}

		public static IDataProtectionBuilder AddDataProtection(this IServiceCollection services, ICertificateResolver certificateResolver, IConfiguration configuration, string configurationKey, IHostEnvironment hostEnvironment, IInstanceFactory instanceFactory, Action<DataProtectionOptions> postConfigureOptions)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(certificateResolver == null)
				throw new ArgumentNullException(nameof(certificateResolver));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if(configurationKey == null)
				throw new ArgumentNullException(nameof(configurationKey));

			if(hostEnvironment == null)
				throw new ArgumentNullException(nameof(hostEnvironment));

			if(instanceFactory == null)
				throw new ArgumentNullException(nameof(instanceFactory));

			if(postConfigureOptions == null)
				throw new ArgumentNullException(nameof(postConfigureOptions));

			var dataProtectionBuilder = new ExtendedDataProtectionBuilder(services)
			{
				CertificateResolver = certificateResolver,
				Configuration = configuration,
				ConfigurationKey = configurationKey,
				HostEnvironment = hostEnvironment,
				InstanceFactory = instanceFactory,
			};

			dataProtectionBuilder.Configure();

			services.PostConfigure(postConfigureOptions);

			return dataProtectionBuilder;
		}

		#endregion
	}
}