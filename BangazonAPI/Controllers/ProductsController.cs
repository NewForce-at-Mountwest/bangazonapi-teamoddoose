using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;

namespace BangazonAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IConfiguration _config;

		public ProductsController(IConfiguration config)
		{
			_config = config;
		}

		public SqlConnection Connection
		{
			get
			{
				return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			}
		}

		[HttpGet]
		public async Task<IActionResult> Get(int limit)
		{
			using (SqlConnection conn = Connection)
			{
				conn.Open();	
				using (SqlCommand cmd = conn.CreateCommand())
				{
					string query = "SELECT Id, Title, ProductTypeId, CustomerId, Price, Description, Quantity FROM Product ";

					if (limit != 0)
					{
						query = $"SELECT TOP {limit} Id, Title, ProductTypeId, CustomerId, Price, Description, Quantity FROM Product ";

					}

					cmd.CommandText = query;
					SqlDataReader reader = cmd.ExecuteReader();
					List<Product> Products = new List<Product>();

					while (reader.Read())
					{
						Product Product = new Product
						{
							Id = reader.GetInt32(reader.GetOrdinal("Id")),
							Title = reader.GetString(reader.GetOrdinal("Title")),
							ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
							CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
							Price = reader.GetDouble(reader.GetOrdinal("Price")),
							Description = reader.GetString(reader.GetOrdinal("Description")),
							Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
						};

						Products.Add(Product);
					}
					reader.Close();

					return Ok(Products);
				}
			}
		}

		[HttpGet("{id}", Name = "GetProduct")]
		public async Task<IActionResult> Get([FromRoute] int id)
		{
			using (SqlConnection conn = Connection)
			{
				conn.Open();
				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = @"
                        SELECT
                            Id, Title, ProductTypeId, CustomerId, Price, Description, Quantity
                        FROM Product
                        WHERE Id = @id";
					cmd.Parameters.Add(new SqlParameter("@id", id));
					SqlDataReader reader = cmd.ExecuteReader();

					Product Product = null;

					if (reader.Read())
					{
						Product = new Product
						{
							Id = reader.GetInt32(reader.GetOrdinal("Id")),
							Title = reader.GetString(reader.GetOrdinal("Title")),
							ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
							CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
							Price = reader.GetDouble(reader.GetOrdinal("Price")),
							Description = reader.GetString(reader.GetOrdinal("Description")),
							Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
						};
					}
					reader.Close();

					return Ok(Product);
				}
			}
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Product Product)
		{
			using (SqlConnection conn = Connection)
			{
				conn.Open();
				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = @"INSERT INTO Product (Title, ProductTypeId, CustomerId, Price, Description, Quantity)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Title, @ProductTypeId, @CustomerId, @Price, @Description, @Quantity)";
					cmd.Parameters.Add(new SqlParameter("@Title", Product.Title));
					cmd.Parameters.Add(new SqlParameter("@ProductTypeId", Product.ProductTypeId));
					cmd.Parameters.Add(new SqlParameter("@CustomerId", Product.CustomerId));
					cmd.Parameters.Add(new SqlParameter("@Price", Product.Price));
					cmd.Parameters.Add(new SqlParameter("@Description", Product.Description));
					cmd.Parameters.Add(new SqlParameter("@Quantity", Product.Quantity));

					int newId = (int)cmd.ExecuteScalar();
					Product.Id = newId;
					return CreatedAtRoute("GetProduct", new { id = newId }, Product);
				}
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product Product)
		{
			try
			{
				using (SqlConnection conn = Connection)
				{
					conn.Open();
					using (SqlCommand cmd = conn.CreateCommand())
					{
						cmd.CommandText = @"UPDATE Product
                                            SET Title = @Title,
											SET ProductTypeId = @ProductTypeId,
											SET CustomerId = @CustomerId,
											SET Price = @Price,
											SET Description = @Description,
											SET Quantity = @Quantity
                                            WHERE Id = @Id";
						cmd.Parameters.Add(new SqlParameter("@Title", Product.Title));
						cmd.Parameters.Add(new SqlParameter("@ProductTypeId", Product.ProductTypeId));
						cmd.Parameters.Add(new SqlParameter("@CustomerId", Product.CustomerId));
						cmd.Parameters.Add(new SqlParameter("@Price", Product.Price));
						cmd.Parameters.Add(new SqlParameter("@Description", Product.Description));
						cmd.Parameters.Add(new SqlParameter("@Quantity", Product.Quantity));

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							return new StatusCodeResult(StatusCodes.Status204NoContent);
						}
						throw new Exception("No rows affected");
					}
				}
			}
			catch (Exception)
			{
				if (!ProductExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			try
			{
				using (SqlConnection conn = Connection)
				{
					conn.Open();
					using (SqlCommand cmd = conn.CreateCommand())
					{
						cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
						cmd.Parameters.Add(new SqlParameter("@id", id));

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							return new StatusCodeResult(StatusCodes.Status204NoContent);
						}
						throw new Exception("No rows affected");
					}
				}
			}
			catch (Exception)
			{
				if (!ProductExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		private bool ProductExists(int id)
		{
			using (SqlConnection conn = Connection)
			{
				conn.Open();
				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = @"
                        SELECT Id, Title, ProductTypeId, CustomerId, Price, Description, Quantity
                        FROM Product
                        WHERE Id = @id";
					cmd.Parameters.Add(new SqlParameter("@id", id));

					SqlDataReader reader = cmd.ExecuteReader();
					return reader.Read();
				}
			}
		}
	}
}