using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class PaymentType
	{
		int Id { get; set; }
		int AccountNumber { get; set; }
		string Name { get; set; }
		int CustomerId { get; set; }
	}
}
