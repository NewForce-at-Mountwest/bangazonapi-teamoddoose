using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Department
	{
		string Name { get; set; }
		int Budget { get; set; }
		int Id { get; set; }
		public List<Employee> Employees { get; set; } = new List<Employee>();
	}
}
