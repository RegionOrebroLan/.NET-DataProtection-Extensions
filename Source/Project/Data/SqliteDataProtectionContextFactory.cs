using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RegionOrebroLan.DataProtection.Data
{
	public class SqliteDataProtectionContextFactory : IDesignTimeDbContextFactory<SqliteDataProtectionContext>
	{
		#region Methods

		public virtual SqliteDataProtectionContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SqliteDataProtectionContext>();
			optionsBuilder.UseSqlite("A value that can not be empty just to be able to create/update migrations.");

			return new SqliteDataProtectionContext(optionsBuilder.Options);
		}

		#endregion
	}
}