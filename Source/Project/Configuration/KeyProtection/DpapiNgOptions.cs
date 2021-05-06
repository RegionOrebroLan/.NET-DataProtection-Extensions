using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using IDataProtectionBuilder = RegionOrebroLan.DataProtection.DependencyInjection.IDataProtectionBuilder;

namespace RegionOrebroLan.DataProtection.Configuration.KeyProtection
{
	public class DpapiNgOptions : KeyProtectionOptions
	{
		#region Properties

		public virtual DpapiNGProtectionDescriptorFlags Flags { get; set; }
		public virtual string ProtectionDescriptorRule { get; set; }

		#endregion

		#region Methods

		public override void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				if(this.ProtectionDescriptorRule == null)
					builder.ProtectKeysWithDpapiNG();
				else
					builder.ProtectKeysWithDpapiNG(this.ProtectionDescriptorRule, this.Flags);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add key-protection by DPAPI-NG for options of type \"{this.GetType()}\".", exception);
			}
		}

		#endregion
	}
}