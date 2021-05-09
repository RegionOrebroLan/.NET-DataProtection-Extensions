using System;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration.KeyProtection
{
	public abstract class KeyProtectionOptions
	{
		#region Methods

		public virtual void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				this.AddInternal(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add key-protection with options of type \"{this.GetType()}\".", exception);
			}
		}

		protected internal abstract void AddInternal(IDataProtectionBuilder builder);

		#endregion
	}
}