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
        // Method for creating new contact in contacts table
        // Returns new contact's contact ID
        public static async Task<string> CreateContact(HttpClient httpClient, string firstName, string lastName, string email, string companyName, string token)
        {
            var url = $"{Program.Scope}contacts"; // construct URL for contacts table

            // set details for new contact 
            var payload = new
            {
                firstname = firstName,
                lastname = lastName,
                emailaddress1 = email
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


        // Method for getting contact details using specific contact ID
        public static async Task<string> GetContactById(HttpClient httpClient, string contactId)
        {
            var url = $"{Program.Scope}contacts({contactId})"; // construct URL for specific contact

            var response = await httpClient.GetAsync(url); // send GET request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve contact. Status code: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result; // read response content

            var contact = JsonSerializer.Deserialize<Contact>(responseBody); // deserialise JSON response into Contact object

            // use helper method to format information of contact
            return Program.FormatInfo(contact);
        }

        // Method for getting all contacts in contacts table
        // Returns List of Contact objects
        public static async Task<List<Contact>> GetAllContacts(HttpClient httpClient)
        {
            var url = $"{Program.Scope}contacts"; // construct URL for contacts table
            var response = await httpClient.GetAsync(url); // send GET request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to retrieve contacts. Status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync(); // read response content

            // Check for errors during deserialisation & if contacts exist in table
            var contactResponse = JsonSerializer.Deserialize<ContactResponse>(responseBody);
            if (contactResponse == null)
            {
                Console.WriteLine("Deserialization returned null.");
                throw new Exception("The response could not be deserialized.");
            }
            if (contactResponse.Value == null)
            {
                Console.WriteLine("The 'Value' property is null.");
                throw new Exception("The response does not contain any contacts.");
            }

            return contactResponse.Value;
        }

        // Method for updating contact using specific contact ID
        public static async Task UpdateContact(HttpClient httpClient, string contactId, string newEmail, string token)
        {
            var url = $"{Program.Scope}contacts({contactId})"; // construct URL for specific contact

            // set details to update
            var payload = new
            {
                emailaddress1 = newEmail
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, url) 
            {
                Content = Program.CreateJsonContent(payload) // serialise payload into JSON
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // add authorisation headers
            var response = await httpClient.SendAsync(request); // send PATCH request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to update contact. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Contact with ID {contactId} updated successfully."); // confirm for user in console
        }

        // Method for deleting contact with specific contact ID
        public static async Task DeleteContact(HttpClient httpClient, string contactId)
        {
            var url = $"{Program.Scope}contacts({contactId})"; // construct URL for specific contact
            var response = await httpClient.DeleteAsync(url); // send DELETE request

            // Log error details if request is unsuccessful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"Failed to delete contact. Status code: {response.StatusCode}");
            }
            Console.WriteLine($"Contact with ID {contactId} deleted successfully."); // confirm for user in console
        }
    }
}
