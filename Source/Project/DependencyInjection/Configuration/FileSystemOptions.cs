using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace RegionOrebroLan.DataProtection.DependencyInjection.Configuration
{
	public class FileSystemOptions : DataProtectionOptions
	{
		#region Properties

		public virtual string Path { get; set; }

		#endregion

		#region Methods

		protected internal override void AddInternal(IDataProtectionBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			var path = this.Path;

			if(string.IsNullOrWhiteSpace(path))
				throw new InvalidOperationException("The path is not set.");

			if(!System.IO.Path.IsPathRooted(path))
				path = System.IO.Path.Combine(builder.HostEnvironment.ContentRootPath, path);

			builder.PersistKeysToFileSystem(new DirectoryInfo(path));
		}

		#endregion
	}
}