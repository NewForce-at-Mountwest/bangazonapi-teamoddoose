using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Employee
	{
		int Id { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		int DepartmentId { get; set; }
		bool IsSupervisor { get; set; }
		Department Department { get; set; }
		Computer Computer { get; set; }
		TrainingProgram TrainingProgram { get; set; }
	}
}
