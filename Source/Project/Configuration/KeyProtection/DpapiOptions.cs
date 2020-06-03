using System;
using Microsoft.AspNetCore.DataProtection;

namespace RegionOrebroLan.DataProtection.Configuration.KeyProtection
{
	public class DpapiOptions : KeyProtectionOptions
	{
		#region Properties

		public virtual bool ProtectToLocalMachine { get; set; }

		#endregion

		#region Methods

		public override void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				builder.ProtectKeysWithDpapi(this.ProtectToLocalMachine);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add key-protection by DPAPI for options of type \"{this.GetType()}\".", exception);
			}
		}

		#endregion
	}
}