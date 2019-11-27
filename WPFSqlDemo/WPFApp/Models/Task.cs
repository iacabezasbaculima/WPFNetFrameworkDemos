using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApp.Models
{
	public class Task
	{
		public int TaskId { get; set; }
		public string Description { get; set; }
		public string Done { get; set; }
		public string DateCompleted { get; set; }
	}
}
