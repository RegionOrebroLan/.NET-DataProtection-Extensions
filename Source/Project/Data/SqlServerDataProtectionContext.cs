using Microsoft.EntityFrameworkCore;

namespace RegionOrebroLan.DataProtection.Data
{
	public class SqlServerDataProtectionContext : DataProtectionContext<SqlServerDataProtectionContext>
	{
		#region Constructors

		public SqlServerDataProtectionContext(DbContextOptions<SqlServerDataProtectionContext> options) : base(options) { }

		#endregion
	}
}