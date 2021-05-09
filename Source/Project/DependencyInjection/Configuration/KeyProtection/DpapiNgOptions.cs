using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration.KeyProtection
{
	public class DpapiNgOptions : KeyProtectionOptions
	{
		#region Properties

		public virtual DpapiNGProtectionDescriptorFlags Flags { get; set; }
		public virtual string ProtectionDescriptorRule { get; set; }

		#endregion

		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			if(this.ProtectionDescriptorRule == null)
				builder.ProtectKeysWithDpapiNG();
			else
				builder.ProtectKeysWithDpapiNG(this.ProtectionDescriptorRule, this.Flags);
		}

		#endregion
	}
}