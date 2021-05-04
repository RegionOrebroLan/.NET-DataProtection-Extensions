using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RegionOrebroLan.DataProtection.Data
{
	public class SqlServerDataProtectionContextFactory : IDesignTimeDbContextFactory<SqlServerDataProtectionContext>
	{
		#region Methods

		public virtual SqlServerDataProtectionContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SqlServerDataProtectionContext>();
			optionsBuilder.UseSqlServer("A value that can not be empty just to be able to create/update migrations.");

			return new SqlServerDataProtectionContext(optionsBuilder.Options);
		}

		#endregion
	}
}