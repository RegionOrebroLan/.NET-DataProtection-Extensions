using Microsoft.EntityFrameworkCore;

namespace RegionOrebroLan.DataProtection.Data
{
	public class SqliteDataProtectionContext : DataProtectionContext<SqliteDataProtectionContext>
	{
		#region Constructors

		public SqliteDataProtectionContext(DbContextOptions<SqliteDataProtectionContext> options) : base(options) { }

		#endregion
	}
}