using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Product
	{
		public int Id { get; set; }
		public int ProductTypeId { get; set; }
		public int CustomerId { get; set; }
		public decimal Price { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Quantity { get; set; }
	}
}
