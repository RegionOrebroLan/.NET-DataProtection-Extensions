namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration
{
	/// <summary>
	/// Options used when no type is configured.
	/// </summary>
	public class EmptyOptions : DataProtectionOptions
	{
		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			// Do nothing.
		}

		#endregion
	}
}