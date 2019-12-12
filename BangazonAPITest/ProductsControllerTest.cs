using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

namespace BangazonAPITest
{
	public class ProductsControllerTest
	{

//dummy product

		private Product dummyProduct { get; } = new Product
		{
			Id = 999,
			ProductTypeId = 1,
			CustomerId = 1,
			Price = 19.99M,
			Title = "Kitchen Utensil Set",
			Description = "Everything you need to get started cooking.",
			Quantity = 232
		};

		private string url { get; } = "/api/Products";


		public async Task<Product> CreateDummyProduct()
		{

			using (var client = new APIClientProvider().Client)
			{

				// Serialize the C# object into a JSON string
				string UtensilAsJSON = JsonConvert.SerializeObject(dummyProduct);


				// Use the client to send the request and store the response
				HttpResponseMessage response = await client.PostAsync(
					url,
					new StringContent(UtensilAsJSON, Encoding.UTF8, "application/json")
				);

				string responseBody = await response.Content.ReadAsStringAsync();

				Product newlyCreatedProduct = JsonConvert.DeserializeObject<Product>(responseBody);

				return newlyCreatedProduct;
			}
		}

		public async Task deleteDummyProduct(Product ProductToDelete)
		{
			using (HttpClient client = new APIClientProvider().Client)
			{
				HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}/{ProductToDelete.Id}");

			}

		}


		/* TESTS START HERE */


//test post

		[Fact]
		public async Task Create_Product()
		{
			using (var client = new APIClientProvider().Client)
			{
				// Create a new product in the db
				Product newUtensil = await CreateDummyProduct();

				// Try to get it again
				HttpResponseMessage response = await client.GetAsync($"{url}/{newUtensil.Id}");
				response.EnsureSuccessStatusCode();

				// Turn the response into JSON
				string responseBody = await response.Content.ReadAsStringAsync();

				// Turn the JSON into C#
				Product newProduct = JsonConvert.DeserializeObject<Product>(responseBody);

				// Make sure it's really there
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				Assert.Equal(dummyProduct.Title, newProduct.Title);
				Assert.Equal(dummyProduct.ProductTypeId, newProduct.ProductTypeId);
				Assert.Equal(dummyProduct.CustomerId, newProduct.CustomerId);
				Assert.Equal(dummyProduct.Price, newProduct.Price);
				Assert.Equal(dummyProduct.Title, newProduct.Title);
				Assert.Equal(dummyProduct.Description, newProduct.Description);
				Assert.Equal(dummyProduct.Quantity, newProduct.Quantity);

				// Clean up after ourselves
				await deleteDummyProduct(newProduct);

			}

		}


//test delete

		[Fact]

		public async Task Delete_Product()
		{
			 

			// Create a new product in the db
			Product newUtensil = await CreateDummyProduct();

			// Delete it
			await deleteDummyProduct(newUtensil);

			using (var client = new APIClientProvider().Client)
			{
				// Try to get it again
				HttpResponseMessage response = await client.GetAsync($"{url}{newUtensil.Id}");

				// Make sure it's really gone
				Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			}
		}

//test get all

		[Fact]
		public async Task Get_All_Product()
		{

			using (var client = new APIClientProvider().Client)
			{

				// Try to get all of the product from /api/Products
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();

				// Convert to JSON
				string responseBody = await response.Content.ReadAsStringAsync();

				// Convert from JSON to C#
				List<Product> Products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

				// Make sure we got back a 200 OK Status and that there are more than 0 products in our database
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				Assert.True(Products.Count > 0);

			}
		}

//test get single

		[Fact]
		public async Task Get_Single_Product()
		{
			using (HttpClient client = new APIClientProvider().Client)
			{
				// Create a dummy product
				Product newUtensil = await CreateDummyProduct();

				// Try to get it
				HttpResponseMessage response = await client.GetAsync($"{url}/{newUtensil.Id}");
				response.EnsureSuccessStatusCode();

				// Turn the response into JSON
				string responseBody = await response.Content.ReadAsStringAsync();

				// Turn the JSON into C#
				Product UtensilFromDB = JsonConvert.DeserializeObject<Product>(responseBody);

				// Did we get back what we expected to get back? 
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				Assert.Equal(dummyProduct.Title, UtensilFromDB.Title);
				Assert.Equal(dummyProduct.ProductTypeId, UtensilFromDB.ProductTypeId);
				Assert.Equal(dummyProduct.CustomerId, UtensilFromDB.CustomerId);
				Assert.Equal(dummyProduct.Price, UtensilFromDB.Price);
				Assert.Equal(dummyProduct.Title, UtensilFromDB.Title);
				Assert.Equal(dummyProduct.Description, UtensilFromDB.Description);
				Assert.Equal(dummyProduct.Quantity, UtensilFromDB.Quantity);

				// Clean up after ourselves-- delete the dummy product we just created
				await deleteDummyProduct(UtensilFromDB);

			}
		}


//test edit

		[Fact]
		public async Task Update_Product()
		{

			using (var client = new APIClientProvider().Client)
			{
				// Create a dummy product
				Product newUtensil = await CreateDummyProduct();

				// Make a new title and assign it to our dummy product
				string newTitle = "Kitchen Multitool";
				newUtensil.Title = newTitle;

				// Convert it to JSON
				string modifiedUtensilAsJSON = JsonConvert.SerializeObject(newUtensil);

				// Try to PUT the newly edited product
				var response = await client.PutAsync(
					$"{url}/{newUtensil.Id}",
					new StringContent(modifiedUtensilAsJSON, Encoding.UTF8, "application/json")
				);

				// See what comes back from the PUT. Is it a 204? 
				string responseBody = await response.Content.ReadAsStringAsync();
				Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

				// Get the edited product back from the database after the PUT
				var getModifiedProduct = await client.GetAsync($"{url}/{newUtensil.Id}");
				getModifiedProduct.EnsureSuccessStatusCode();

				// Convert it to JSON
				string getProductBody = await getModifiedProduct.Content.ReadAsStringAsync();

				// Convert it from JSON to C#
				Product newlyEditedProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

				// Make sure the title was modified correctly
				Assert.Equal(HttpStatusCode.OK, getModifiedProduct.StatusCode);
				Assert.Equal(newTitle, newlyEditedProduct.Title);

				// Clean up after yourself
				await deleteDummyProduct(newlyEditedProduct);
			}
		}

		[Fact]
		public async Task Test_Get_NonExitant_Product_Fails()
		{

			using (var client = new APIClientProvider().Client)
			{
				// Try to get a product with an Id that could never exist
				HttpResponseMessage response = await client.GetAsync($"{url}/00000000");

				// It should bring back a 204 no content error
				Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
			}
		}

		[Fact]
		public async Task Test_Delete_NonExistent_Product_Fails()
		{
			using (var client = new APIClientProvider().Client)
			{
				// Try to delete an Id that shouldn't exist 
				HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}0000000000");

				Assert.False(deleteResponse.IsSuccessStatusCode);
				Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
			}
		}
	}
}
