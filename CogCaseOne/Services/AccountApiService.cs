using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CogCaseOne.Models;
using Newtonsoft.Json.Linq;

namespace CogCaseOne.Services
{

    public class AccountApiService
    {
        /// <summary>
        /// Creates new account in accounts table.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="name">Name of account</param>
        /// <param name="email">Email of account</param>
        /// <param name="phone">Phone number of account</param>
        /// <param name="token">Authorisation token</param>
        /// <returns>ID of newly created account</returns>
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
                Content = UtilityHelper.CreateJsonContent(payload) // serialise payload into JSON
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send POST request

            response.EnsureSuccessStatusCode();

            // Extract ID from response headers
            return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
        }

        /// <summary>
        /// Retrieves account details using specific account ID
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="accountId">ID of account to retrieve</param>
        /// <returns>Formatted account details as string.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<string> GetAccountById(HttpClient httpClient, string accountId) 
        {
            var url = $"{Program.Scope}accounts({accountId})?$expand=primarycontactid($select=fullname)"; // constructs URL to get specific account

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
            return UtilityHelper.FormatInfo(account);
        }

        /// <summary>
        /// Retrieves all accounts in accounts table.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <returns>List of all accounts.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public static async Task<List<Account>> GetAllAccounts(HttpClient httpClient)
        {
            var url = $"{Program.Scope}accounts?$expand=primarycontactid($select=fullname)"; // constructs URL for accounts table
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

        /// <summary>
        /// Updates account using specific account ID.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="accountId">ID of account to update</param>
        /// <param name="newEmail">New email address for account</param>
        /// <param name="newPhone">New phone number for account</param>
        /// <param name="token">Authorisation token</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
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
                Content = UtilityHelper.CreateJsonContent(payload) // serialises payload into JSON 
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
        
        /// <summary>
        /// Updates a specified account to link a specific contact as primary contact.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="accountId">ID of account to update</param>
        /// <param name="contactId">ID of contact to set as primary contact for account</param>
        /// <param name="token">Authorisation token</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task UpdateAccountContact(HttpClient httpClient, string accountId, string contactId, string token)
        {
            var url = $"{Program.Scope}accounts({accountId})"; // constructs URL for specific account

            // set details to update
            var payload = new Dictionary<string, object>
            {
                {"primarycontactid@odata.bind", $"/contacts({contactId})" }
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = UtilityHelper.CreateJsonContent(payload) // serialises payload into JSON 
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

        /// <summary>
        /// Deletes account using specific account ID.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="accountId">ID of account to delete</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
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
