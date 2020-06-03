using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public class RedisOptions : ExtendedDataProtectionOptions
	{
		#region Properties

		public virtual string Key { get; set; } = "DataProtectionKeys";

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

				builder.PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(this.Url), this.Key);

				base.Add(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add data-protection by Redis for options of type \"{this.GetType()}\".", exception);
			}
		}

		#endregion
	}
}