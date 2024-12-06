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

    /// <summary>
    /// Main entry point of application.
    /// Performs CRUD operations on accounts, contacts, and incidents tables.
    /// </summary>
    /// <param name="args">Command-line arguments</param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
        // Load environment variables from .env file
        DotEnv.Load();

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
            await EntityManager.ManageEntities(httpClient, Token);
            Console.WriteLine("==============================");
            Console.WriteLine("Close to exit or press ENTER to view all entities in accounts, contacts, and incidents tables.");
            Console.ReadLine();
            await EntityManager.GetAllEntities(httpClient);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
    
     /// <summary>
     /// Retrieves user ID.
     /// </summary>
     /// <returns>User ID as string.</returns>
     static string GetUserId()
     {
         ServiceClient service = new ServiceClient(connectionString);
         var response = (WhoAmIResponse)service.Execute(new WhoAmIRequest());

         return response.UserId.ToString();
     }

     /// <summary>
     /// Retrieves access token.
     /// </summary>
     /// <param name="tenantId">Tenant ID</param>
     /// <param name="clientId">Client ID</param>
     /// <param name="clientSecret">Client secret</param>
     /// <param name="username">Username</param>
     /// <param name="password">Password</param>
     /// <param name="scope">Scope</param>
     /// <returns>Access token as string.</returns>
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

}

