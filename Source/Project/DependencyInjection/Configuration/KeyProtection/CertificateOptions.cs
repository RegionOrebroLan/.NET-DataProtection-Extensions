using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using RegionOrebroLan.Abstractions.Extensions;
using RegionOrebroLan.Configuration;
using RegionOrebroLan.Security.Cryptography.Configuration;
using RegionOrebroLan.Security.Cryptography.Extensions;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration.KeyProtection
{
	public class CertificateOptions : KeyProtectionOptions
	{
		#region Properties

		public virtual DynamicOptions CertificateResolver { get; set; } = new DynamicOptions
		{
			Type = typeof(StoreResolverOptions).AssemblyQualifiedName
		};

		#endregion

		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			var resolverOptions = (ResolverOptions)builder.InstanceFactory.Create(this.CertificateResolver.Type);
			this.CertificateResolver.Options?.Bind(resolverOptions);

			var certificate = builder.CertificateResolver.Resolve(resolverOptions);

			builder.ProtectKeysWithCertificate(certificate.Unwrap<X509Certificate2>());
		}

		#endregion
	}
}