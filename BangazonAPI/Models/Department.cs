﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Department
	{
		public string Name { get; set; }
		public int Budget { get; set; }
		public int Id { get; set; }
		public List<Employee> Employees { get; set; } = new List<Employee>();
	}
}
