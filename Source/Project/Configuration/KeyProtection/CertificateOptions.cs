using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using RegionOrebroLan.Abstractions.Extensions;
using RegionOrebroLan.Configuration;
using RegionOrebroLan.Security.Cryptography.Configuration;
using RegionOrebroLan.Security.Cryptography.Extensions;

namespace RegionOrebroLan.DataProtection.Configuration.KeyProtection
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

		public override void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				var resolverOptions = (ResolverOptions)builder.InstanceFactory.Create(this.CertificateResolver.Type);
				this.CertificateResolver.Options?.Bind(resolverOptions);

				var certificate = builder.CertificateResolver.Resolve(resolverOptions);

				builder.ProtectKeysWithCertificate(certificate.Unwrap<X509Certificate2>());
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add key-protection by certificate for options of type \"{this.GetType()}\".", exception);
			}
		}

		#endregion
	}
}