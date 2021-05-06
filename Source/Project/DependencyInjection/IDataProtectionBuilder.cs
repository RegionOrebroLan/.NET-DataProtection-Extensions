using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RegionOrebroLan.DependencyInjection;
using RegionOrebroLan.Security.Cryptography;

namespace RegionOrebroLan.DataProtection.DependencyInjection
{
	public interface IDataProtectionBuilder : Microsoft.AspNetCore.DataProtection.IDataProtectionBuilder
	{
		#region Properties

		ICertificateResolver CertificateResolver { get; }
		IConfiguration Configuration { get; }
		string ConfigurationKey { get; }
		IHostEnvironment HostEnvironment { get; }
		IInstanceFactory InstanceFactory { get; }

		#endregion

		#region Methods

		void Configure();

		#endregion
	}
}