using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
	public class Computer
	{
		int Id { get; set; }
		DateTime PurchaseDate { get; set; }
		DateTime DecommisionedDate { get; set; }
		string Make { get; set; }
		string Manufacture { get; set; }

	}
}
