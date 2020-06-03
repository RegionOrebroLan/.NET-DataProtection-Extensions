using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RegionOrebroLan.DataProtection.Data
{
	public class DataProtectionKeyDbContext : DbContext, IDataProtectionKeyContext
	{
		#region Constructors

		public DataProtectionKeyDbContext(DbContextOptions<DataProtectionKeyDbContext> options) : base(options) { }

		#endregion

		#region Properties

		public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

		#endregion
	}
}