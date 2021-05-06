using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.DataProtection.Data;
using RegionOrebroLan.DataProtection.DependencyInjection;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public class SqliteOptions : DatabaseOptions
	{
		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.Services.AddDbContext<DataProtectionContext, SqliteDataProtectionContext>(optionsBuilder =>
				optionsBuilder.UseSqlite(
					builder.Configuration.GetConnectionString(this.ConnectionStringName),
					options =>
					{
						if(this.MigrationsAssembly != null)
							options.MigrationsAssembly(this.MigrationsAssembly);
					}
				));
		}

		#endregion
	}
}