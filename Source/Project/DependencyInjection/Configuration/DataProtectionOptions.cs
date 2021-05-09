using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RegionOrebroLan.Configuration;
using RegionOrebroLan.DataProtection.DependencyInjection.Configuration.KeyProtection;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration
{
	public abstract class DataProtectionOptions
	{
		#region Properties

		public virtual IConfigurationSection KeyProtection { get; set; }
		public virtual IConfigurationSection Options { get; set; }

		#endregion

		#region Methods

		public virtual void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				this.AddInternal(builder);

				var keyProtectionOptions = new DynamicOptions();
				this.KeyProtection?.Bind(keyProtectionOptions);

				if(keyProtectionOptions.Type == null)
					return;

				var keyProtection = (KeyProtectionOptions)builder.InstanceFactory.Create(keyProtectionOptions.Type);
				this.KeyProtection?.Bind(keyProtection);
				keyProtection.Add(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add data-protection with options of type \"{this.GetType()}\".", exception);
			}
		}

		protected internal abstract void AddInternal(IDataProtectionBuilder builder);

		public virtual void Use(IApplicationBuilder builder)
		{
			try
			{
				this.UseInternal(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not use data-protection with options of type \"{this.GetType()}\".", exception);
			}
		}

		protected internal virtual void UseInternal(IApplicationBuilder builder) { }

		#endregion
	}
}