using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Product
	{
		int Id { get; set; }
		int ProductTypeId { get; set; }
		int CustomerId { get; set; }
		Double Price { get; set; }
		string Title { get; set; }
		String Description { get; set; }
		int Quantity { get; set; }
		Customer Customer { get; set; }
		ProductType ProductType { get; set; }
	}
}
