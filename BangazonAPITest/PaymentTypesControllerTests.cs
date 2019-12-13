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
    public class PaymentTypesControllerTest
    {

        //dummy paymentType

        private PaymentType dummyPaymentType { get; } = new PaymentType
        {
            
            AccountNumber = 1,
            Name = "Bodacious",
            CustomerId = 1,
        };

        private string url { get; } = "/api/PaymentTypes";


        public async Task<PaymentType> CreateDummyPaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {

                // Serialize the C# object into a JSON string
                string PaymentAsJSON = JsonConvert.SerializeObject(dummyPaymentType);


                // Use the client to send the request and store the response
                HttpResponseMessage response = await client.PostAsync(
                    url,
                    new StringContent(PaymentAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                PaymentType newlyCreatedPaymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                return newlyCreatedPaymentType;
            }
        }

        public async Task deleteDummyPaymentType(PaymentType PaymentTypeToDelete)
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}/{PaymentTypeToDelete.Id}");

            }

        }


        /* TESTS START HERE */


        //test post

        [Fact]
        public async Task Create_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Create a new paymentType in the db
                PaymentType newPayment = await CreateDummyPaymentType();

                // Try to get it again
                HttpResponseMessage response = await client.GetAsync($"{url}/{newPayment.Id}");
                response.EnsureSuccessStatusCode();

                // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();

                // Turn the JSON into C#
                PaymentType newPaymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                // Make sure it's really there
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyPaymentType.AccountNumber, newPaymentType.AccountNumber);
                Assert.Equal(dummyPaymentType.Name, newPaymentType.Name);
                Assert.Equal(dummyPaymentType.CustomerId, newPaymentType.CustomerId);
          

                // Clean up after ourselves
                await deleteDummyPaymentType(newPaymentType);

            }

        }


        //test delete

        [Fact]

        public async Task Delete_PaymentType()
        {


            // Create a new paymentType in the db
            PaymentType newPayment = await CreateDummyPaymentType();

            // Delete it
            await deleteDummyPaymentType(newPayment);

            using (var client = new APIClientProvider().Client)
            {
                // Try to get it again
                HttpResponseMessage response = await client.GetAsync($"{url}{newPayment.Id}");

                // Make sure it's really gone
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }

        //test get all

        [Fact]
        public async Task Get_All_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {

                // Try to get all of the paymentType from /api/PaymentTypes
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Convert to JSON
                string responseBody = await response.Content.ReadAsStringAsync();

                // Convert from JSON to C#
                List<PaymentType> PaymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                // Make sure we got back a 200 OK Status and that there are more than 0 paymentType in our database
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(PaymentTypes.Count > 0);

            }
        }

        //test get single

        [Fact]
        public async Task Get_Single_PaymentType()
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                // Create a dummy paymentType
                PaymentType newPayment = await CreateDummyPaymentType();

                // Try to get it
                HttpResponseMessage response = await client.GetAsync($"{url}/{newPayment.Id}");
                response.EnsureSuccessStatusCode();

                // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();

                // Turn the JSON into C#
                PaymentType PaymentFromDB = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                // Did we get back what we expected to get back? 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyPaymentType.AccountNumber, newPayment.AccountNumber);
                Assert.Equal(dummyPaymentType.Name, newPayment.Name);
                Assert.Equal(dummyPaymentType.CustomerId, newPayment.CustomerId);


                // Clean up after ourselves-- delete the dummy paymentType we just created
                await deleteDummyPaymentType(PaymentFromDB);

            }
        }


        //test edit

        [Fact]
        public async Task Update_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                // Create a dummy paymentType
                PaymentType newPayment = await CreateDummyPaymentType();

                // Make a new acctNumber and assign it to our dummy paymentType
                String newName = "Bogdonny";
                newPayment.Name = newName;

                // Convert it to JSON
                string modifiedPaymentAsJSON = JsonConvert.SerializeObject(newPayment);

                // Try to PUT the newly edited paymentType
                var response = await client.PutAsync(
                    $"{url}/{newPayment.Id}",
                    new StringContent(modifiedPaymentAsJSON, Encoding.UTF8, "application/json")
                );

                // See what comes back from the PUT. Is it a 204? 
                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                // Get the edited paymentType back from the database after the PUT
                var getModifiedPaymentType = await client.GetAsync($"{url}/{newPayment.Id}");
                getModifiedPaymentType.EnsureSuccessStatusCode();

                // Convert it to JSON
                string getPaymentTypeBody = await getModifiedPaymentType.Content.ReadAsStringAsync();

                // Convert it from JSON to C#
                PaymentType newlyEditedPaymentType = JsonConvert.DeserializeObject<PaymentType>(getPaymentTypeBody);

                // Make sure the Name was modified correctly
                Assert.Equal(HttpStatusCode.OK, getModifiedPaymentType.StatusCode);
                Assert.Equal(newName, newlyEditedPaymentType.Name);

                // Clean up after yourself
                await deleteDummyPaymentType(newlyEditedPaymentType);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_PaymentType_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                // Try to get a paymentType with an Id that could never exist
                HttpResponseMessage response = await client.GetAsync($"{url}/00000000");

                // It should bring back a 204 no content error
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_PaymentType_Fails()
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
