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
    public class IncidentApiService
    {
        // Base URL for API
        static string Scope = "https://orgff19c007.crm11.dynamics.com/api/data/v9.2/";

        // Method for getting all incidents in cases table
        public static async Task<List<Incident>> GetAllIncidents(HttpClient httpClient)
        {
            var url = $"{Scope}incidents"; // construct URL for cases table
            var response = await httpClient.GetAsync(url); // send GET request

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve cases. Status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseBody);

            var incidentResponse = JsonSerializer.Deserialize<IncidentResponse>(responseBody);
            if (incidentResponse == null)
            {
                Console.WriteLine("Deserialization returned null.");
                throw new Exception("The response could not be deserialized.");
            }
            if (incidentResponse.Value == null)
            {
                Console.WriteLine("The 'Value' property is null.");
                throw new Exception("The response does not contain any accounts.");
            }

            return incidentResponse.Value;
        }
        public static async Task<string> CreateIncident(HttpClient httpClient, string Title, string Description, string customerId, int statusCode, string token)
        {
            var url = $"{Scope}incidents";
            var payload = new Dictionary<string, object>
        {
            {"title", Title },
            {"description", Description },
            //emailaddress = emailAddress,
            {"customerid_account@odata.bind", $"/accounts({customerId})" },
            //_customerid_value = customerId, //account id
            {"statuscode", statusCode }
        };
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = Program.CreateJsonContent(payload)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // Ensure the token is added
            var response = await httpClient.SendAsync(request);

            //response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to create incident. Status code: {response.StatusCode}");
            }

            // Extract ID from response headers
            return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
        }
        //public static StringContent CreateJsonContent(object payload)
        //{
        //    var json = JsonSerializer.Serialize(payload);
        //    return new StringContent(json, Encoding.UTF8, "application/json");
        //}

        public static async Task<string> GetIncidentById(HttpClient httpClient, string incidentId)
        {
            // Ensure the base URL ends with a slash for proper concatenation
            var url = $"{Scope}incidents({incidentId})"; // Use BaseUrl instead of Scope if it holds the correct API base

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
            var caseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            var nonNullValues = new Dictionary<string, object>();
            foreach (var pair in caseData)
            {
                if (pair.Value != null)
                {
                    nonNullValues[pair.Key] = pair.Value;
                }
            }
            return JsonSerializer.Serialize(nonNullValues, new JsonSerializerOptions { WriteIndented = true });
            //--return responseBody;
        }

        public static async Task UpdateIncident(HttpClient httpClient, string incidentId, int statusCode, string newEmail, string token)
        {
            var url = $"{Scope}incidents({incidentId})";
            var payload = new
            {
                emailaddress = newEmail,
                statuscode = statusCode
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = Program.CreateJsonContent(payload)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to update incident. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Account with ID {incidentId} updated successfully.");
        }
        public static async Task DeleteIncident(HttpClient httpClient, string incidentId)
        {
            var url = $"{Scope}incidents({incidentId})";
            var response = await httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to delete incident. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Account with ID {incidentId} deleted successfully.");
        }

    }
}
