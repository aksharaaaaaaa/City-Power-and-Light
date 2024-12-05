using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CogCaseOne.Models;

namespace CogCaseOne.Services
{

    public class AccountApiService
    {
        // Method for creating a new account in accounts table
        // Returns new account's account ID
        public static async Task<string> CreateAccount(HttpClient httpClient, string name, string email, string phone, string token) 
        {
            var url = $"{Program.Scope}accounts"; // constructs URL for accounts table

            // set details for new account
            var payload = new
            {
                name = name,
                emailaddress1 = email,
                telephone1 = phone
            };
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = Program.CreateJsonContent(payload) // serialise payload into JSON
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send POST request

            response.EnsureSuccessStatusCode();

            // Extract ID from response headers
            return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
        }

        // Method for getting account details using specific account ID
        public static async Task<string> GetAccountById(HttpClient httpClient, string accountId) 
        {
            var url = $"{Program.Scope}accounts({accountId})"; // constructs URL to get specific account

            var response = await httpClient.GetAsync(url); // send GET request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve account. Status code: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result; // read response content

            var account = JsonSerializer.Deserialize<Account>(responseBody); // deserialise JSON response into Account object

            // use helper method to format information of account
            return Program.FormatInfo(account);
        }

        // Method to display all accounts in accounts table
        public static async Task<List<Account>> GetAllAccounts(HttpClient httpClient)
        {
            var url = $"{Program.Scope}accounts"; // constructs URL for accounts table
            var response = await httpClient.GetAsync(url); // send GET request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve accounts. Status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync(); // read response content

            var accountResponse = JsonSerializer.Deserialize<AccountResponse>(responseBody); // deserialise JSON response

            // Check for errors during deserialisation & if accounts exist in table
            if (accountResponse == null)
            {
                Console.WriteLine("Deserialization returned null.");
                throw new Exception("The response could not be deserialized.");
            }
            if (accountResponse.Value == null)
            {
                Console.WriteLine("The 'Value' property is null.");
                throw new Exception("The response does not contain any accounts.");
            }

            return accountResponse.Value;
        }

        // Method for updating account using specific account ID
        public static async Task UpdateAccount(HttpClient httpClient, string accountId, string newEmail, string newPhone, string token)
        {
            var url = $"{Program.Scope}accounts({accountId})"; // constructs URL for specific account

            // set details to update
            var payload = new
            {
                emailaddress1 = newEmail,
                telephone1 = newPhone
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = Program.CreateJsonContent(payload) // serialises payload into JSON 
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send PATCH request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to update account. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Account with ID {accountId} updated successfully."); // confirm for user in console
        }

        // Method for deleting account using specific account ID
        public static async Task DeleteAccount(HttpClient httpClient, string accountId) 
        {
            var url = $"{Program.Scope}accounts({accountId})"; // constructs URL for specific account
            var response = await httpClient.DeleteAsync(url); // sends DELETE request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to delete account. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Account with ID {accountId} deleted successfully."); // confirm for user in console
        }
    }
}
