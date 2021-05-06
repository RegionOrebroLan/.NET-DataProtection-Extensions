using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using IDataProtectionBuilder = RegionOrebroLan.DataProtection.DependencyInjection.IDataProtectionBuilder;

namespace RegionOrebroLan.DataProtection.Configuration
{
	public class FileSystemOptions : ExtendedDataProtectionOptions
	{
		#region Properties

		public virtual string Path { get; set; }

		#endregion

		#region Methods

		public override void Add(IDataProtectionBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				var path = this.Path;

				if(string.IsNullOrWhiteSpace(path))
					throw new InvalidOperationException("The path is not set.");

				if(!System.IO.Path.IsPathRooted(path))
					path = System.IO.Path.Combine(builder.HostEnvironment.ContentRootPath, path);

				builder.PersistKeysToFileSystem(new DirectoryInfo(path));

				base.Add(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add data-protection by file-system for options of type \"{this.GetType()}\".", exception);
			}
		}

		#endregion
	}
}