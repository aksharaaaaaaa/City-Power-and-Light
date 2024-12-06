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
        /// <summary>
        /// Creates new incident in cases table.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="Title">Title of incident</param>
        /// <param name="Description">Description of incident</param>
        /// <param name="customerId">Customer ID (account ID) to link incident to</param>
        /// <param name="statusCode">Status code of incident</param>
        /// <param name="token">Authorisation token</param>
        /// <returns>ID of newly created incident.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<string> CreateIncident(HttpClient httpClient, string Title, string Description, string customerId, int statusCode, string token)
        {
            var url = $"{Program.Scope}incidents"; // construct URL for cases table

            // set details for new incident
            // uses dictionary to allow setting customer ID with account ID - must use @odata.bind
            var payload = new Dictionary<string, object>
            {
                {"title", Title },
                {"description", Description },
                {"customerid_account@odata.bind", $"/accounts({customerId})" },
                {"statuscode", statusCode }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = UtilityHelper.CreateJsonContent(payload) // serialise payload into JSON
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send POST request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to create incident. Status code: {response.StatusCode}");
            }

            // Extract ID from response headers
            return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
        }

        /// <summary>
        /// Retrieves incident details using specific incident ID.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="incidentId">ID of incident to retrieve</param>
        /// <returns>Formatted incident details as string.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<string> GetIncidentById(HttpClient httpClient, string incidentId)
        {
            var url = $"{Program.Scope}incidents({incidentId})?$expand=customerid_account($select=name)"; // construct URL for specific incident

            var response = await httpClient.GetAsync(url); // sends GET request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve incident. Status code: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result; // read response content
            var incident = JsonSerializer.Deserialize<Incident>(responseBody); // deserialise JSON response into Incident object

            // use helper method to format information of incident
            return UtilityHelper.FormatInfo(incident);
        }

        /// <summary>
        /// Retrieves all incidents in incidents table.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <returns>List of incidents.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception"></exception>
        public static async Task<List<Incident>> GetAllIncidents(HttpClient httpClient)
        {
            var url = $"{Program.Scope}incidents?$expand=customerid_account($select=name)"; // construct URL for cases table
            var response = await httpClient.GetAsync(url); // send GET request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve cases. Status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync(); // read response content

            // Check for errors during deserialisation & if cases exist in table
            var incidentResponse = JsonSerializer.Deserialize<IncidentResponse>(responseBody);
            if (incidentResponse == null)
            {
                Console.WriteLine("Deserialization returned null.");
                throw new Exception("The response could not be deserialized.");
            }
            if (incidentResponse.Value == null)
            {
                Console.WriteLine("The 'Value' property is null.");
                throw new Exception("The response does not contain any cases.");
            }

            return incidentResponse.Value;
        }

        /// <summary>
        /// Updates incident using specific incident ID.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="incidentId">ID of incident to update</param>
        /// <param name="statusCode">New status code for incident</param>
        /// <param name="newEmail">New email address for incident</param>
        /// <param name="token">Authentication token</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task UpdateIncident(HttpClient httpClient, string incidentId, int statusCode, string newEmail, string token)
        {
            var url = $"{Program.Scope}incidents({incidentId})"; // constructs URL for specific incident

            // set new details for incident
            var payload = new
            {
                emailaddress = newEmail,
                statuscode = statusCode
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = UtilityHelper.CreateJsonContent(payload) // serialise payload into JSON
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send PATCH request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to update incident. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Incident with ID {incidentId} updated successfully."); // confirm for user in console
        }

        /// <summary>
        /// Deletes incident using specific incident ID.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="incidentId">ID of incident to delete</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task DeleteIncident(HttpClient httpClient, string incidentId)
        {
            var url = $"{Program.Scope}incidents({incidentId})"; // construct URL for specific incident
            var response = await httpClient.DeleteAsync(url); // send DELETE request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to delete incident. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Incident with ID {incidentId} deleted successfully."); // confirm for user in console
        }

    }
}
