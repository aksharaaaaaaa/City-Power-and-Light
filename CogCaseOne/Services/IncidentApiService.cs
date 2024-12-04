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
            
            // Log details if error
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve cases. Status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync(); //read response content
            //Console.WriteLine(responseBody);

            // Check for errors during deserialisation & if contacts exist in table
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

        // Method for creating new incident in cases table
        // Returns incident ID of newly created incident
        public static async Task<string> CreateIncident(HttpClient httpClient, string Title, string Description, string customerId, int statusCode, string token)
        {
            var url = $"{Scope}incidents"; // construct URL for cases table

            // set details for new incident
            // uses dictionary to allow for customer id - must use @odata.bind
            var payload = new Dictionary<string, object>
            {
                {"title", Title },
                {"description", Description },
                //emailaddress = emailAddress,
                {"customerid_account@odata.bind", $"/accounts({customerId})" },
                {"statuscode", statusCode }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = Program.CreateJsonContent(payload) // serialise payload into JSON
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send POST request

            // Log details if error
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to create incident. Status code: {response.StatusCode}");
            }

            // Extract ID from response headers
            return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
        }

        // Method for getting incident details using specific incident ID
        public static async Task<string> GetIncidentById(HttpClient httpClient, string incidentId)
        {
            var url = $"{Scope}incidents({incidentId})"; // construct URL for specific incident

            var response = await httpClient.GetAsync(url); // sends GET request

            // Log details if error
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve account. Status code: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result; // read response content
            
            // Deserialise response to get non-null values only
            var caseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            var nonNullValues = new Dictionary<string, object>();
            foreach (var pair in caseData)
            {
                if (pair.Value != null)
                {
                    nonNullValues[pair.Key] = pair.Value;
                }
            }
            return JsonSerializer.Serialize(nonNullValues, new JsonSerializerOptions { WriteIndented = true }); // serialises non-null values into indented JSON for user in console
        }

        // Method for updating incident using specific incident ID
        public static async Task UpdateIncident(HttpClient httpClient, string incidentId, int statusCode, string newEmail, string token)
        {
            var url = $"{Scope}incidents({incidentId})"; // constructs URL for specific incident

            // set new details for incident
            var payload = new
            {
                emailaddress = newEmail,
                statuscode = statusCode
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = Program.CreateJsonContent(payload) // serialise payload into JSON
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send PATCH request

            //Log details if error
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to update incident. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Account with ID {incidentId} updated successfully."); // confirm for user in console
        }

        // Method for deleting incident using specific incident ID
        public static async Task DeleteIncident(HttpClient httpClient, string incidentId)
        {
            var url = $"{Scope}incidents({incidentId})"; // construct URL for specific incident
            var response = await httpClient.DeleteAsync(url); // send DELETE request

            // Log details if error
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to delete incident. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Account with ID {incidentId} deleted successfully."); // confirm for user in console
        }

    }
}
