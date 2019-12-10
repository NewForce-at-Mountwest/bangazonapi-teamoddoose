using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Customer
	{
		int Id { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		DateTime AccountCreated { get; set; }
		DateTime LastActive { get; set; }
		List<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();
		List<Product> Products { get; set; } = new List<Product>();
	}
}
