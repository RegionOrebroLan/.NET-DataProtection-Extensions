using RegionOrebroLan.DataProtection.DependencyInjection;

namespace RegionOrebroLan.DataProtection.Configuration.KeyProtection
{
	public abstract class KeyProtectionOptions
	{
		#region Methods

		public abstract void Add(IDataProtectionBuilder builder);

		#endregion
	}
}