using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration
{
	public class AzureOptions : DataProtectionOptions
	{
		#region Properties

		[SuppressMessage("Design", "CA1056:Uri properties should not be strings")]
		public virtual string Url { get; set; }

		#endregion

		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.PersistKeysToAzureBlobStorage(new Uri(this.Url));
		}

		#endregion
	}
}