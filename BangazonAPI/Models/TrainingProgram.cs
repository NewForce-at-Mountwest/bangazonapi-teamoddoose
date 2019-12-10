using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class TrainingProgram
	{
		int Id { get; set; }
		string Name { get; set; }
		DateTime StartDate { get; set; }
		DateTime EndDate { get; set; }
		int MaxAttendees { get; set; }
		List<Employee> Employees { get; set; } = new List<Employee>();
	}
}
