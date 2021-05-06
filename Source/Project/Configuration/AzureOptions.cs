using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using IDataProtectionBuilder = RegionOrebroLan.DataProtection.DependencyInjection.IDataProtectionBuilder;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public class AzureOptions : ExtendedDataProtectionOptions
	{
		#region Properties

		[SuppressMessage("Design", "CA1056:Uri properties should not be strings")]
		public virtual string Url { get; set; }

		#endregion

		#region Methods

		public override void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				builder.PersistKeysToAzureBlobStorage(new Uri(this.Url));

				base.Add(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add data-protection by Azure for options of type \"{this.GetType()}\".", exception);
			}
		}

		#endregion
	}
}