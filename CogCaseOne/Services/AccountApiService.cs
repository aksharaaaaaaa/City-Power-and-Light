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

    public class AccountApiService
    {
        static string Scope = "https://orgff19c007.crm11.dynamics.com/api/data/v9.2/";
        public static async Task<string> GetAccountById(HttpClient httpClient, string accountId)
        {
            // Ensure the base URL ends with a slash for proper concatenation
            var url = $"{Scope}accounts({accountId})"; // Use BaseUrl instead of Scope if it holds the correct API base

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Log details if there's an error
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve account. Status code: {response.StatusCode}");
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

        public static async Task<string> CreateAccount(HttpClient httpClient, string name, string email, string phone, string token)
        {
            var url = $"{Scope}accounts";
            var payload = new
            {
                name = name,
                emailaddress1 = email,
                telephone1 = phone
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

        public static async Task DeleteAccount(HttpClient httpClient, string accountId)
        {
            var url = $"{Scope}accounts({accountId})";
            var response = await httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to delete account. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Account with ID {accountId} deleted successfully.");
        }
        public static async Task UpdateAccount(HttpClient httpClient, string accountId, string newEmail, string newPhone)
        {
            var url = $"{Scope}accounts({accountId})";
            var payload = new
            {
                emailaddress1 = newEmail,
                telephone1 = newPhone
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
            Console.WriteLine($"Account with ID {accountId} updated successfully.");
        }

        public static async Task<List<Account>> GetAllAccounts(HttpClient httpClient)
        {
            var url = $"{Scope}accounts";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve accounts. Status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseBody);

            var accountResponse = JsonSerializer.Deserialize<AccountResponse>(responseBody);
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
        public static StringContent CreateJsonContent(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static async Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret, string username, string password, string scope)
        {
            var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            var client = new HttpClient();
            var payload = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "username", username },
            { "password", password },
            { "scope", scope }
        };
            var response = await client.PostAsync(url, new FormUrlEncodedContent(payload));
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);
            return tokenResponse.access_token;
        }
    }
}
