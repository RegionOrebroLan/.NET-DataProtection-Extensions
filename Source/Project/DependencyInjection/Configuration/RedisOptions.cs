using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration
{
	public class RedisOptions : DataProtectionOptions
	{
		#region Properties

		public virtual string Configuration { get; set; }
		public virtual ConfigurationOptions ConfigurationOptions { get; set; }
		public virtual string Key { get; set; } = "DataProtectionKeys";

		#endregion

		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			ConnectionMultiplexer connectionMultiplexer;

			if(this.ConfigurationOptions != null)
			{
				var endPointsSection = builder.Configuration.GetSection($"{builder.ConfigurationKey}:{nameof(this.ConfigurationOptions)}:{nameof(this.ConfigurationOptions.EndPoints)}");
				var endPoints = new Dictionary<string, int>();
				endPointsSection?.Bind(endPoints);

				foreach(var endPoint in endPoints)
				{
					this.ConfigurationOptions.EndPoints.Add(endPoint.Key, endPoint.Value);
				}

				connectionMultiplexer = ConnectionMultiplexer.Connect(this.ConfigurationOptions);
			}
			else
			{
				connectionMultiplexer = ConnectionMultiplexer.Connect(this.Configuration);
			}

			//this.ConfigurationOptions.ConnectRetry

			builder.PersistKeysToStackExchangeRedis(connectionMultiplexer, this.Key);
		}

		#endregion
	}
}