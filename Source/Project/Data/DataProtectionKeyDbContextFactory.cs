using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RegionOrebroLan.DataProtection.Data
{
	public class DataProtectionKeyDbContextFactory : IDesignTimeDbContextFactory<DataProtectionKeyDbContext>
	{
		#region Methods

		public virtual DataProtectionKeyDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<DataProtectionKeyDbContext>();
			optionsBuilder.UseSqlServer("A value that can not be empty just to be able to create/update migrations.");

			return new DataProtectionKeyDbContext(optionsBuilder.Options);
		}

		#endregion
	}
}