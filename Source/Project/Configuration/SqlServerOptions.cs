using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.DataProtection.Data;
using RegionOrebroLan.IO.Extensions;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public class SqlServerOptions : DatabaseOptions
	{
		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.Services.AddDbContext<DataProtectionContext, SqlServerDataProtectionContext>(optionsBuilder =>
				optionsBuilder.UseSqlServer(
					this.GetConnectionString(builder.Configuration),
					options =>
					{
						if(this.MigrationsAssembly != null)
							options.MigrationsAssembly(this.MigrationsAssembly);
					}
				));
		}

		protected internal virtual string GetConnectionString(IConfiguration configuration)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			var connectionString = configuration.GetConnectionString(this.ConnectionStringName);

			var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

			// ReSharper disable InvertIf
			if(!string.IsNullOrEmpty(connectionStringBuilder.AttachDBFilename))
			{
				connectionStringBuilder.AttachDBFilename = connectionStringBuilder.AttachDBFilename.ResolveDataDirectorySubstitution();

				if(string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog))
					connectionStringBuilder.InitialCatalog = connectionStringBuilder.AttachDBFilename;

				connectionString = connectionStringBuilder.ToString();
			}
			// ReSharper restore InvertIf

			return connectionString;
		}

		#endregion
	}
}