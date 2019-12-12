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
    public class ProductTypeControllerTest
    {

     
        private ProductType dummyProductType { get; } = new ProductType
        {
            Name = "stinky feta"
        };

    ///         // Base URL
        private string url { get; } = "/api/ProductTypes";


    ///         // Method: Create a new productType in the database
        public async Task<ProductType> CreateDummyProductType()
        {

            using (var client = new APIClientProvider().Client)
            {

    ///         // Serialize the C# object into a JSON string
                string productTypeAsJSON = JsonConvert.SerializeObject(dummyProductType);


    ///         // Use the client to send the request and store the response
                HttpResponseMessage response = await client.PostAsync(
                    url,
                    new StringContent(productTypeAsJSON, Encoding.UTF8, "application/json")
                );

    ///         // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

    ///         // Deserialize the JSON into an instance of ProductType
                ProductType newProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                return newProductType;
            }
        }

    ///         Method: Delete ProductType
        public async Task deleteDummyProductType(ProductType productTypeToDelete)
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}/{productTypeToDelete.Id}");

            }

        }


        /* TESTS START HERE */


        [Fact]
        public async Task Create_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
    ///         // Create a new ProductType in the DB
                ProductType newProductType = await CreateDummyProductType();

    ///         // Retrieve ProductType
                HttpResponseMessage response = await client.GetAsync($"{url}/{newProductType.Id}");
                response.EnsureSuccessStatusCode();

    ///         // JSON Converstion
                string responseBody = await response.Content.ReadAsStringAsync();

    ///         // Turn the JSON into C#
                ProductType newConvertedProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);

    ///         // Make sure it's really there
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyProductType.Name, newProductType.Name);
                

    ///         // Clean up after ourselves
                await deleteDummyProductType(newProductType);

            }

        }


        [Fact]

        public async Task Delete_ProductType()
        {

    ///     // Create a new ProductType in the DB
            ProductType newProductType= await CreateDummyProductType();

    ///     // Delete ProductType
            await deleteDummyProductType(newProductType);
    
            using (var client = new APIClientProvider().Client)
            {
    ///         // Retrieve
                HttpResponseMessage response = await client.GetAsync($"{url}{newProductType.Id}");

    ///         // Confirm Deletion
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }

        [Fact]
        public async Task Get_All_ProductType()
        {

            using (var client = new APIClientProvider().Client)
            {

    ///         // Retrieve from /api/ProductTypes
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

    ///        // Convert to JSON
                string responseBody = await response.Content.ReadAsStringAsync();

    ///         // Convert from JSON to C#
                List<ProductType> productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

    ///         // 200 OK Status - Confirm count of productTypes are more than zero. 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);

            }
        }

        [Fact]
        public async Task Get_Single_ProductType()
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
    ///         // Create a dummy ProductType
                ProductType newProductType = await CreateDummyProductType();

    ///         // Retrieve
                HttpResponseMessage response = await client.GetAsync($"{url}/{newProductType.Id}");
                response.EnsureSuccessStatusCode();

    ///         // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();

    ///         // Turn the JSON into C#
                ProductType productTypeFromDB = JsonConvert.DeserializeObject<ProductType>(responseBody);

    ///         // 200 OK Status - Confirm Name 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyProductType.Name, productTypeFromDB.Name);
               

    ///            // Cleanup 
                await deleteDummyProductType(productTypeFromDB);

            }
        }




        [Fact]
        public async Task Update_ProductType()
        {

            using (var client = new APIClientProvider().Client)
            {
    ///         // Create dummy ProductType
                ProductType newProductType = await CreateDummyProductType();

    ///         // Create a new name and assign it to dummy ProductType
                string newName = "stinky gouda ";
                newProductType.Name = newName;

    ///         // Convert to JSON
                string modifiedProductTypeAsJSON = JsonConvert.SerializeObject(newProductType);

    ///         // PUT 
                var response = await client.PutAsync(
                    $"{url}/{newProductType.Id}",
                    new StringContent(modifiedProductTypeAsJSON, Encoding.UTF8, "application/json")
                );

    ///         // Confirm PUT
                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    ///         // Get the edited ProductType back from the database after the PUT
                var getModifiedProductType = await client.GetAsync($"{url}/{newProductType.Id}");
                getModifiedProductType.EnsureSuccessStatusCode();

    ///         // Convert it to JSON
                string getProductTypeBody = await getModifiedProductType.Content.ReadAsStringAsync();

    ///         // Convert it from JSON to C#
                ProductType newEditProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);

    ///         // Confirms that the productType name has been edited
                Assert.Equal(HttpStatusCode.OK, getModifiedProductType.StatusCode);
                Assert.Equal(newName, newEditProductType.Name);

    ///         // Clean up
                await deleteDummyProductType(newEditProductType);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitent_ProductType_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
    ///         // Non-Existent ProductType check
                HttpResponseMessage response = await client.GetAsync($"{url}/00000000");

    ///         // 204 Error Code 
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_ProductType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Delete and ID that should not exist
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}0000000000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
    }
}



