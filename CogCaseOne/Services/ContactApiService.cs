using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CogCaseOne.Models;
using Newtonsoft.Json.Linq;

namespace CogCaseOne.Services
{
    public class ContactApiService
    {
        static string Scope = "https://orgff19c007.crm11.dynamics.com/api/data/v9.2/";

        public static async Task<string> CreateContact(HttpClient httpClient, string firstName, string lastName, string email, string companyName, string token)
        {
            var url = $"{Scope}contacts";
            var payload = new
            {
                firstname = firstName,
                lastname = lastName,
                emailaddress1 = email
            };
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = CreateJsonContent(payload)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // Ensure the token is added
            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            // Extract ID from response headers
            return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
        }

        public static async Task<string> GetContactById(HttpClient httpClient, string contactId)
        {
            // Ensure the base URL ends with a slash for proper concatenation
            var url = $"{Scope}contacts({contactId})"; // Use BaseUrl instead of Scope if it holds the correct API base

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Log details if there's an error
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve contact. Status code: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            //added to get non-null only
            var accountData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            var nonNullValues = new Dictionary<string, object>();
            foreach (var pair in accountData)
            {
                if (pair.Value != null)
                {
                    nonNullValues[pair.Key] = pair.Value;
                }
            }
            return JsonSerializer.Serialize(nonNullValues, new JsonSerializerOptions { WriteIndented = true });
            //--return responseBody;
        }
        public static async Task<List<Contact>> GetAllContacts(HttpClient httpClient)
        {
            var url = $"{Scope}contacts";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve accounts. Status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseBody);

            var contactResponse = JsonSerializer.Deserialize<ContactResponse>(responseBody);
            if (contactResponse == null)
            {
                Console.WriteLine("Deserialization returned null.");
                throw new Exception("The response could not be deserialized.");
            }
            if (contactResponse.Value == null)
            {
                Console.WriteLine("The 'Value' property is null.");
                throw new Exception("The response does not contain any accounts.");
            }

            return contactResponse.Value;
        }
        public static async Task UpdateContact(HttpClient httpClient, string contactId, string newEmail)
        {
            var url = $"{Scope}contacts({contactId})";
            var payload = new
            {
                emailaddress1 = newEmail
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = CreateJsonContent(payload)
            };

           // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to update account. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Contact with ID {contactId} updated successfully.");
        }
        public static async Task DeleteContact(HttpClient httpClient, string contactId)
        {
            var url = $"{Scope}contacts({contactId})";
            var response = await httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to delete account. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Contact with ID {contactId} deleted successfully.");
        }
        public static StringContent CreateJsonContent(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
