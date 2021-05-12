namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration
{
	public abstract class ConnectionStringOptions : DataProtectionOptions
	{
		#region Properties

		public virtual string ConnectionStringName { get; set; }

		#endregion
	}
}