using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using RegionOrebroLan.Configuration;
using RegionOrebroLan.DataProtection.Configuration.KeyProtection;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public abstract class ExtendedDataProtectionOptions : DataProtectionOptions
	{
		#region Properties

		public virtual DynamicOptions KeyProtection { get; set; }

		#endregion

		#region Methods

		public virtual void Add(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			if(this.KeyProtection?.Type == null)
				return;

			try
			{
				var keyProtectionOptions = (KeyProtectionOptions)builder.InstanceFactory.Create(this.KeyProtection.Type);
				this.KeyProtection.Options.Bind(keyProtectionOptions);
				keyProtectionOptions.Add(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Could not add key-protection.", exception);
			}
		}

		public virtual void Use(IApplicationBuilder builder) { }

		#endregion
	}
}