using System.ComponentModel.DataAnnotations;

namespace Application.Models.Forms
{
	public class UnprotectForm : ProtectionForm
	{
		#region Properties

		[MaxLength(MaximumLengthForText)]
		[Required]
		public virtual string TextToUnprotect { get; set; }

		#endregion
	}
}