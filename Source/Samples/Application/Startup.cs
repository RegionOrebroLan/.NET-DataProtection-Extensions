using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegionOrebroLan;
using RegionOrebroLan.DataProtection.Builder.Extensions;
using RegionOrebroLan.DataProtection.DependencyInjection.Extensions;
using RegionOrebroLan.DependencyInjection;
using RegionOrebroLan.Security.Cryptography;

namespace Application
{
	public class Startup
	{
		#region Constructors

		public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
		}

		#endregion

		#region Properties

		public virtual IConfiguration Configuration { get; }
		public virtual IHostEnvironment HostEnvironment { get; }
		protected internal virtual bool IsDefaultEnvironment => string.Equals("Default", this.HostEnvironment.EnvironmentName, StringComparison.OrdinalIgnoreCase);

		#endregion

		#region Methods

		public virtual void Configure(IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			applicationBuilder.UseDeveloperExceptionPage();

			AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(this.HostEnvironment.ContentRootPath, "Data"));

			if(!this.IsDefaultEnvironment)
				applicationBuilder.UseDataProtection();

			applicationBuilder
				.UseStaticFiles()
				.UseRouting()
				.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(!this.IsDefaultEnvironment)
				services.AddDataProtection(this.CreateCertificateResolver(), this.Configuration, this.HostEnvironment, new InstanceFactory());

			services.AddControllersWithViews();
		}

		protected internal virtual ICertificateResolver CreateCertificateResolver()
		{
			var services = new ServiceCollection();

			services.AddSingleton(AppDomain.CurrentDomain);
			services.AddSingleton<FileCertificateResolver>();
			services.AddSingleton(this.HostEnvironment);
			services.AddSingleton<IApplicationDomain, ApplicationHost>();
			services.AddSingleton<ICertificateResolver, CertificateResolver>();
			services.AddSingleton<StoreCertificateResolver>();

			return services.BuildServiceProvider().GetRequiredService<ICertificateResolver>();
		}

		#endregion
	}
}