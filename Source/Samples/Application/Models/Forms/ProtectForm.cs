using System.ComponentModel.DataAnnotations;

namespace Application.Models.Forms
{
	public class ProtectForm : ProtectionForm
	{
		#region Properties

		[MaxLength(MaximumLengthForText)]
		[Required]
		public virtual string TextToProtect { get; set; }

		#endregion
	}
}