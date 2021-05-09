using System;
using Microsoft.AspNetCore.DataProtection;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration.KeyProtection
{
	public class DpapiOptions : KeyProtectionOptions
	{
		#region Properties

		public virtual bool ProtectToLocalMachine { get; set; }

		#endregion

		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.ProtectKeysWithDpapi(this.ProtectToLocalMachine);
		}

		#endregion
	}
}