using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.DataProtection.Data;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration
{
	public abstract class DatabaseOptions : DataProtectionOptions
	{
		#region Properties

		public virtual string ConnectionStringName { get; set; } = "Data-Protection";
		public virtual string MigrationsAssembly { get; set; }

		#endregion

		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.PersistKeysToDbContext<DataProtectionContext>();
		}

		protected internal override void UseInternal(IApplicationBuilder builder)
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

		#endregion
	}
}