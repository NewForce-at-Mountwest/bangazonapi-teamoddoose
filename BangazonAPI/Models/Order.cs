using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Order
	{
		int Id { get; set; }
		int PaymentTypeId { get; set; }
		int CustomerId { get; set; }
		PaymentType PaymentType { get; set; }
		Customer Customer { get; set; }
		List<Product> Products { get; set; } = new List<Product>();
	}
}
