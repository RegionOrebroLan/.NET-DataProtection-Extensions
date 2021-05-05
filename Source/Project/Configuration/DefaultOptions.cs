using Microsoft.AspNetCore.Builder;

namespace RegionOrebroLan.DataProtection.Configuration
{
	/// <summary>
	/// Options doing nothing. The default data-protection set up by the framework is used.
	/// </summary>
	public class DefaultOptions : ExtendedDataProtectionOptions
	{
		#region Methods

		public override void Add(IDataProtectionBuilder builder) { }
		public override void Use(IApplicationBuilder builder) { }

		#endregion
	}
}