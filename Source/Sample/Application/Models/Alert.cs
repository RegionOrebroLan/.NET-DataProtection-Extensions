using System.Collections.Generic;

namespace Application.Models
{
	public class Alert
	{
		#region Properties

		public virtual IList<string> Details { get; } = new List<string>();
		public virtual string Heading { get; set; }
		public virtual string Information { get; set; }
		public virtual AlertMode Mode { get; set; } = AlertMode.Info;

		#endregion
	}
}