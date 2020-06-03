using System.Collections.Generic;
using Application.Models.Forms;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;

namespace Application.Models.ViewModels
{
	public class HomeViewModel
	{
		#region Fields

		private static readonly IList<Protection> _protections = new List<Protection>();

		#endregion

		#region Properties

		public virtual IDataProtectionProvider DataProtectionProvider { get; set; }
		public virtual IDataProtector DataProtector { get; set; }
		public virtual string DefaultPurpose { get; set; }
		public virtual IOptions<KeyManagementOptions> KeyManagementOptions { get; set; }
		public virtual Alert ProtectAlert { get; set; }
		public virtual ProtectForm ProtectForm { get; set; } = new ProtectForm();
		public virtual IList<Protection> Protections => _protections;
		public virtual Alert UnprotectAlert { get; set; }
		public virtual UnprotectForm UnprotectForm { get; set; } = new UnprotectForm();

		#endregion
	}
}