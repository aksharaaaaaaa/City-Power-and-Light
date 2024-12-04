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
using dotenv.net.Utilities;

class Program 
{
    static string Token; // Token for authentication
    private const string BaseUrl = "https://orgff19c007.crm11.dynamics.com/.default"; // Base URL for API
    static string Scope = "https://orgff19c007.crm11.dynamics.com/api/data/v9.2/"; // Scope for API

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
        //Console.WriteLine("Token: " + Token);

        var httpClient = new HttpClient();
        // set authorisation header with token
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        try
        {
            // UNCOMMENT FOLLOWING LINES TO PERFORM VARIOUS CRUD OPERATIONS ON ACCOUNTS/CONTACTS/INCIDENTS TABLES

            //CREATE ACCOUNT
            //var accountId = await AccountApiService.CreateAccount(httpClient, "Looney Tunesss Show", "contact@tunesss.com", "123456-7890", Token);
            //Console.WriteLine($"Created Account ID: {accountId}");
            //var accountId = await AccountApiService.CreateAccount(httpClient, "Test456", "contact@test111.com", "111222-3330", Token);
            //Console.WriteLine($"Created Account ID: {accountId}");

            //UPDATE/DELETE ACCOUNTS
            //await AccountApiService.UpdateAccount(httpClient, "04618639-aab0-ef11-b8e8-6045bdcf868c", "test222@test.com", "444333-2221", Token);
            //await AccountApiService.DeleteAccount(httpClient, "8a14ffe2-a2b0-ef11-b8e8-6045bdcf868c");

            //GET ACCOUNT BY ID
            //string retrievedAccount = await AccountApiService.GetAccountById(httpClient, "04618639-aab0-ef11-b8e8-6045bdcf868c");
            ////--Task<string> retrievedAccount = AccountApiService.GetAccountById(httpClient, "a5a28bc9-6dae-ef11-b8e8-6045bdcf868c");
            //Console.WriteLine(retrievedAccount);

            //GET ALL ACCOUNTS
            //var allAccounts = await AccountApiService.GetAllAccounts(httpClient);
            //Console.WriteLine("All accounts:");
            //foreach (var account in allAccounts)
            //{
            //    Console.WriteLine($"Name: {account.Name}");
            //    Console.WriteLine($"ID: {account.AccountId}");
            //    Console.WriteLine();
            //}
            //

            //

            //CREATE & GET CONTACT
            //var contactId = await ContactApiService.CreateContact(httpClient, "Test", "Person", "contact@test.com", "Test111", Token);
            //Console.WriteLine($"Created contact ID: {contactId}");
            //string retrievedContact = await ContactApiService.GetContactById(httpClient, contactId);
            //Console.WriteLine(retrievedContact);
            //

            //UPDATE/DELETE CONTACT
            //await UpdateContact(httpClient, "55a0a54b-ceb0-ef11-b8e8-6045bdcf868c", "update@test2.com", Token);
            //await DeleteContact(httpClient, "55a0a54b-ceb0-ef11-b8e8-6045bdcf868c");


            //GET ALL CONTACTS
            var allContacts = await ContactApiService.GetAllContacts(httpClient); //55a0a54b - ceb0 - ef11 - b8e8 - 6045bdcf868c
            Console.WriteLine("All contacts:");
            foreach (var contact in allContacts)
            {
                Console.WriteLine($"Contact ID: {contact.ContactId}");
                Console.WriteLine($"Full Name: {contact.FullName}");
                Console.WriteLine($"Email: {contact.Email}");
                Console.WriteLine($"Company: {contact.Company}");

                Console.WriteLine();
            }
            //

            //

            //CREATE INCIDENT
            //var incidentId = await IncidentApiService.CreateIncident(httpClient, "Test", "Testing case creation", "04618639-aab0-ef11-b8e8-6045bdcf868c", 1, Token); 
            //Console.WriteLine($"Created incident ID: {incidentId}");

            //GET ONE INCIDENT 
            //string retrievedCase = await IncidentApiService.GetIncidentById(httpClient, "c4ecffbf-35b2-ef11-b8e8-6045bdcf868c");
            //Console.WriteLine(retrievedCase);

            //GET ALL INCIDENTS
            //var allIncidents = await IncidentApiService.GetAllIncidents(httpClient);
            //Console.WriteLine("All incidents:");
            //foreach (var incident in allIncidents)
            //{
            //    Console.WriteLine($"Incident ID: {incident.IncidentId}");
            //    Console.WriteLine($"Title: {incident.Title}");
            //    Console.WriteLine($"Description: {incident.Description}");
            //    Console.WriteLine($"Customer ID: {incident.CustomerId}");
            //    Console.WriteLine();
            //}

            //UPDATE INCIDENT
            //await IncidentApiService.UpdateIncident(httpClient, "c4ecffbf-35b2-ef11-b8e8-6045bdcf868c", 1, "update@test.com", Token);
            //string retrievedCase = await IncidentApiService.GetIncidentById(httpClient, "c4ecffbf-35b2-ef11-b8e8-6045bdcf868c");
            //Console.WriteLine(retrievedCase);

            //DELETE INCIDENT
            //var beforeDeletionIncidents = await IncidentApiService.GetAllIncidents(httpClient);
            //Console.WriteLine("All incidents:");
            //foreach (var incident in allIncidents)
            //{
            //    Console.WriteLine($"Title: {incident.Title}");
            //}
            //await IncidentApiService.DeleteIncident(httpClient, "c4ecffbf-35b2-ef11-b8e8-6045bdcf868c");
            //var afterDeletionIncidents = await IncidentApiService.GetAllIncidents(httpClient);
            //Console.WriteLine("All incidents:");
            //foreach (var incident in allIncidents)
            //{
            //    Console.WriteLine($"Title: {incident.Title}");
            //}

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

        //Console.WriteLine($"User ID is {response.UserId}.");//--{response.Results.Keys}.");

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

}
    