using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RegionOrebroLan.DataProtection.Data
{
	public abstract class DataProtectionContext : DbContext, IDataProtectionKeyContext
	{
		#region Constructors

		protected DataProtectionContext(DbContextOptions options) : base(options) { }

		#endregion

		#region Properties

		public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

		#endregion
	}

	public abstract class DataProtectionContext<T> : DataProtectionContext where T : DataProtectionContext
	{
		#region Constructors

		protected DataProtectionContext(DbContextOptions<T> options) : base(options) { }

		#endregion
	}
}