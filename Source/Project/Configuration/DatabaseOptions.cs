using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.DataProtection.Data;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public abstract class DatabaseOptions : ExtendedDataProtectionOptions
	{
		#region Properties

		public virtual string ConnectionStringName { get; set; } = "Data-Protection";
		public virtual string MigrationsAssembly { get; set; }

		#endregion

		#region Methods

		public override void Use(IApplicationBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				// ReSharper disable ConvertToUsingDeclaration
				using(var serviceScope = builder.ApplicationServices.CreateScope())
				{
					var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<DataProtectionContext>();
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