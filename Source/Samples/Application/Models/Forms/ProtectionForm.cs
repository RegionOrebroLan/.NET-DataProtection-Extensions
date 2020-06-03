using System.ComponentModel.DataAnnotations;

namespace Application.Models.Forms
{
	public abstract class ProtectionForm
	{
		#region Fields

		public const int MaximumLengthForText = 255;

		#endregion

		#region Properties

		[MaxLength(MaximumLengthForText)]
		public virtual string Purpose { get; set; }

		#endregion
	}
}