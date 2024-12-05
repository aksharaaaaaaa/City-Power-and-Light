namespace CogCaseOne;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Identity.Client; 
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.Text;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using CogCaseOne.Models;
using CogCaseOne.Services;
using dotenv.net;


class Program 
{
    static string Token; // Token for authentication
    private const string BaseUrl = "https://orgff19c007.crm11.dynamics.com/.default"; // Base URL for API
    public static string Scope = "https://orgff19c007.crm11.dynamics.com/api/data/v9.2/"; // Scope for API

    // Environment variables for tenant ID, secret ID, app ID
    static string TenantID = Environment.GetEnvironmentVariable("tenant_id");
    static string SecretID = Environment.GetEnvironmentVariable("secret_id");
    static string AppID = Environment.GetEnvironmentVariable("app_id");

    // Construct connection string for service client
    static string connectionString = $@"AuthType=ClientSecret;
                        SkipDiscovery=true;url={BaseUrl};
                        Secret={SecretID};
                        ClientId={AppID};
                        RequireNewInstance=true";


    static async Task Main(string[] args)
    {
        // Load environment variables from .env file
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "C:\\Users\\aksha\\source\\repos\\CogCaseOne\\CogCaseOne\\.env" }));
        
        string userID = GetUserId();
        string email = Environment.GetEnvironmentVariable("email");
        string password = Environment.GetEnvironmentVariable("password");

        // Get access token
        Token = await GetAccessToken(TenantID, AppID, SecretID, email, password, BaseUrl);

        var httpClient = new HttpClient();
        // set authorisation header with token
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        try
        {

            // ACCOUNTS
            // create account
            var accountId = await AccountApiService.CreateAccount(httpClient, "Test", "contact@test.com", "111111-1111", Token);
            Console.WriteLine($"Created new Account ID: {accountId}");
            // get account details
            string createdAccount = await AccountApiService.GetAccountById(httpClient, accountId);
            Console.WriteLine("\n" + createdAccount);
            // update account
            await AccountApiService.UpdateAccount(httpClient, accountId, "update@test.com", "999999-9999", Token);
            // get account details (updated)
            string updatedAccount = await AccountApiService.GetAccountById(httpClient, accountId);
            Console.WriteLine("\n" + updatedAccount);
            // delete account
            await AccountApiService.DeleteAccount(httpClient, accountId);
            // show all accounts
            var allAccounts = await AccountApiService.GetAllAccounts(httpClient);
            Console.WriteLine("\nAll accounts:");
            foreach (Account account in allAccounts)
            {
                Console.WriteLine(FormatInfo(account));
            }


            // CONTACTS
            // create contact
            var contactId = await ContactApiService.CreateContact(httpClient, "Test", "Contact", "contact@test.com", "Test111", Token);
            Console.WriteLine($"Created new contact ID: {contactId}");
            // get contact details
            string createdContact = await ContactApiService.GetContactById(httpClient, contactId);
            Console.WriteLine("\n" + createdContact);
            // update contact
            await ContactApiService.UpdateContact(httpClient, contactId, "update@test2.com", Token);
            // get contact details (updated)
            string updatedContact = await ContactApiService.GetContactById(httpClient, contactId);
            Console.WriteLine("\n" + updatedContact);
            // delete contact
            await ContactApiService.DeleteContact(httpClient, contactId);
            // show all contacts
            var allContacts = await ContactApiService.GetAllContacts(httpClient);
            Console.WriteLine("\nAll contacts:");
            foreach (Contact contact in allContacts)
            {
                Console.WriteLine(FormatInfo(contact));
            }


            // INCIDENTS
            // create account
            var caseAccountId = await AccountApiService.CreateAccount(httpClient, "Test", "contact@test.com", "111111-1111", Token);
            Console.WriteLine($"Created new Account ID: {caseAccountId}");
            // create incident linked to account
            var incidentId = await IncidentApiService.CreateIncident(httpClient, "Test Case", "Testing case creation", caseAccountId, 1, Token);
            Console.WriteLine($"Created incident ID: {incidentId}");
            // get incident details
            string createdIncident = await IncidentApiService.GetIncidentById(httpClient, incidentId);
            Console.WriteLine("\n" + createdIncident);
            // update incident
            await IncidentApiService.UpdateIncident(httpClient, incidentId, 4, "update@test.com", Token);
            // get incident details (updated)
            string updatedIncident = await IncidentApiService.GetIncidentById(httpClient, incidentId);
            Console.WriteLine("\n" + updatedIncident);
            // delete incident & account
            await IncidentApiService.DeleteIncident(httpClient, incidentId);
            await AccountApiService.DeleteAccount(httpClient, caseAccountId);
            // show all incidents
            var allIncidents = await IncidentApiService.GetAllIncidents(httpClient);
            Console.WriteLine("\nAll incidents:");
            foreach (Incident incident in allIncidents)
            {
                Console.WriteLine(FormatInfo(incident));
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
    
    // Method for getting user ID
    static string GetUserId()
    {
        ServiceClient service = new ServiceClient(connectionString);
        var response = (WhoAmIResponse)service.Execute(new WhoAmIRequest());

        return response.UserId.ToString();
    }

    // Method for getting access token
    public static async Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret, string username, string password, string scope)
    {
        var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"; // construct URL
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
        var response = await client.PostAsync(url, new FormUrlEncodedContent(payload)); // send POST request
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync(); // read response content
        var tokenResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse); // deserialise response into AuthResponse object
        return tokenResponse.access_token; // extract access_token from AuthResponse object
    }

    // Method for serialising objects into JSON content - used in update and create methods
    public static StringContent CreateJsonContent(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    // Method for formatting entity information for console user
    public static string FormatInfo(object entity)
    {
        var entityInfo = new StringBuilder();

        switch (entity)
        {
            case Account account:
                entityInfo.AppendLine($"Account ID: {account.AccountId}");
                entityInfo.AppendLine($"Name: {account.Name}");
                entityInfo.AppendLine($"Email: {account.Email}");
                entityInfo.AppendLine($"Phone: {account.Phone}");
                break;
            case Contact contact:
                entityInfo.AppendLine($"Contact ID: {contact.ContactId}");
                entityInfo.AppendLine($"Full Name: {contact.FullName}");
                entityInfo.AppendLine($"Email: {contact.Email}");
                break;
            case Incident incident:
                entityInfo.AppendLine($"Incident ID: {incident.IncidentId}");
                entityInfo.AppendLine($"Title: {incident.Title}");
                entityInfo.AppendLine($"Description: {incident.Description}");
                entityInfo.AppendLine($"Customer ID: {incident.CustomerId}");
                entityInfo.AppendLine($"Email: {incident.Email}");
                entityInfo.AppendLine($"Status Code: {incident.StatusCode}");
                break;
            default:
                throw new Exception("Invalid entity type");
        }
        return entityInfo.ToString();
    }
}
    