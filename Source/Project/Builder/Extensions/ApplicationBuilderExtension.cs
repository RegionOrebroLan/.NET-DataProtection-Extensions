using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.DataProtection.DependencyInjection.Configuration;

namespace RegionOrebroLan.DataProtection.Builder.Extensions
{
	public static class ApplicationBuilderExtension
	{
		#region Methods

		public static IApplicationBuilder UseDataProtection(this IApplicationBuilder applicationBuilder)
		{
			try
			{
				if(applicationBuilder == null)
					throw new ArgumentNullException(nameof(applicationBuilder));

				var dataProtectionOptions = applicationBuilder.ApplicationServices.GetRequiredService<DataProtectionOptions>();

				dataProtectionOptions.Use(applicationBuilder);

				return applicationBuilder;
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Could not use data-protection.", exception);
			}
		}

		#endregion
	}
}