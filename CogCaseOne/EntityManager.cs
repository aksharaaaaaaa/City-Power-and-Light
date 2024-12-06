using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CogCaseOne.Models;
using CogCaseOne.Services;
using Newtonsoft.Json.Linq;

namespace CogCaseOne
{
    /// <summary>
    /// Manages creation, reading, updating, and deletion of entities in Dataverse.
    /// </summary>
    internal class EntityManager
    {
        /// <summary>
        /// Manages process of creating, updating, and deleting entities.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="token">Authentication token</param>
        /// <returns></returns>
        public static async Task ManageEntities(HttpClient httpClient, string token)
        {
            // create and get account & contact
            Console.WriteLine("======= Creating account and contact =======");
            var accountId = await CreateAndGetAccount(httpClient, token);
            var contactId = await CreateAndGetContact(httpClient, token);

            // update and get contact details
            Console.WriteLine("======= Updating contact details =======");
            await UpdateAndGetContact(httpClient, contactId, token);

            // update account to link contact & get account
            Console.WriteLine("======= Linking contact as primary contact to account =======");
            await UpdateAccountContactAndGet(httpClient, accountId, contactId, token);

            // create case linked to account & get
            Console.WriteLine("======= Creating case linked to account =======");
            var incidentId = await CreateAndGetIncident(httpClient, accountId, token);

            // update and get case info
            Console.WriteLine("======= Updating case details =======");
            await UpdateAndGetIncident(httpClient, incidentId, token);

            // wait for user to confirm in web
            Console.WriteLine("Please check Dataverse for created entities & press ENTER to confirm deletion");
            Console.ReadLine();

            // delete all
            Console.WriteLine("======= Deleting created entities =======");
            await DeleteEntities(httpClient, accountId, contactId, incidentId);

        }
        /// <summary>
        /// Creates account and retrieves its details.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="token">Authentication token</param>
        /// <returns>ID of created account</returns>
        public static async Task<string> CreateAndGetAccount(HttpClient httpClient, string token)
        {
            // create account
            var accountId = await AccountApiService.CreateAccount(httpClient, "NewAccount", "new@account.com", "111111-1111", token);
            Console.WriteLine($"Created new Account ID: {accountId}");

            // get account details
            string createdAccount = await AccountApiService.GetAccountById(httpClient, accountId);
            Console.WriteLine("\n" + createdAccount);

            return accountId;
        }

        /// <summary>
        /// Creates contact and retrieves its details.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="token">Authentication token</param>
        /// <returns>ID of created contact</returns>
        public static async Task<string> CreateAndGetContact(HttpClient httpClient, string token)
        {
            // create contact
            var contactId = await ContactApiService.CreateContact(httpClient, "Test", "Contact", "contact@test.com", token);
            Console.WriteLine($"Created new contact ID: {contactId}");

            // get contact details
            string createdContact = await ContactApiService.GetContactById(httpClient, contactId);
            Console.WriteLine("\n" + createdContact);

            return contactId;
        }

        /// <summary>
        /// Updates contact and retrieves its updated details.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="contactId">ID of contact to update</param>
        /// <param name="token">Authentication token</param>
        /// <returns></returns>
        public static async Task UpdateAndGetContact(HttpClient httpClient, string contactId, string token)
        {
            await ContactApiService.UpdateContact(httpClient, contactId, "update@test2.com", token);

            string updatedContact = await ContactApiService.GetContactById(httpClient, contactId);
            Console.WriteLine("\n" + updatedContact);
        }
        /// <summary>
        /// Updates account to link a contact and retrieves updated account details.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="accountId">ID of account to update</param>
        /// <param name="contactId">ID of contact to link</param>
        /// <param name="token">Authentication token</param>
        /// <returns></returns>
        public static async Task UpdateAccountContactAndGet(HttpClient httpClient, string accountId, string contactId, string token)
        {
            await AccountApiService.UpdateAccountContact(httpClient, accountId, contactId, token);
            string updatedAccount = await AccountApiService.GetAccountById(httpClient, accountId);
            Console.WriteLine("\n" + updatedAccount);
        }

        /// <summary>
        /// Creates incident linked to an account and retrieves its details
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="accountId">ID of account to link incident to</param>
        /// <param name="token">Authentication token</param>
        /// <returns>ID of created incident</returns>
        public static async Task<string> CreateAndGetIncident(HttpClient httpClient, string accountId, string token)
        {
            var incidentId = await IncidentApiService.CreateIncident(httpClient, "Test Case", "Creating test case", accountId, 1, token);
            Console.WriteLine($"Created incident ID: {incidentId}");
            string createdCase = await IncidentApiService.GetIncidentById(httpClient, incidentId);
            Console.WriteLine("\n" + createdCase);

            return incidentId;
        }

        /// <summary>
        /// Updates incident and retrieves its updated details
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="incidentId">ID of incident to update</param>
        /// <param name="token">Authentication token</param>
        /// <returns></returns>
        public static async Task UpdateAndGetIncident(HttpClient httpClient, string  incidentId, string token)
        {
            await IncidentApiService.UpdateIncident(httpClient, incidentId, 4, "updated@case.com", token);
            string updatedCase = await IncidentApiService.GetIncidentById(httpClient, incidentId);
            Console.WriteLine("\n" + updatedCase);
        }

        /// <summary>
        /// Deletes specified account, contact, and incident.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <param name="accountId">ID of account to delete</param>
        /// <param name="contactId">ID of contact to delete</param>
        /// <param name="incidentId">ID of incident to delete</param>
        /// <returns></returns>
        public static async Task DeleteEntities(HttpClient httpClient, string accountId, string contactId, string incidentId)
        {
            await IncidentApiService.DeleteIncident(httpClient, incidentId);
            await ContactApiService.DeleteContact(httpClient, contactId);
            await AccountApiService.DeleteAccount(httpClient, accountId);
        }

        /// <summary>
        /// Gets all accounts, contacts, and incidents from Dataverse.
        /// </summary>
        /// <param name="httpClient">Http client used for API requests</param>
        /// <returns></returns>
        public static async Task GetAllEntities(HttpClient httpClient)
        {
            // get all accounts
            var allAccounts = await AccountApiService.GetAllAccounts(httpClient);
            Console.WriteLine("\nAll accounts:");
            foreach (Account account in allAccounts)
            {
                Console.WriteLine(UtilityHelper.FormatInfo(account));
            }
            Console.WriteLine("-------------------------");
            // get all contacts
            var allContacts = await ContactApiService.GetAllContacts(httpClient);
            Console.WriteLine("\nAll contacts:");
            foreach (Contact contact in allContacts)
            {
                Console.WriteLine(UtilityHelper.FormatInfo(contact));
            }
            Console.WriteLine("-------------------------");
            // get all incidents
            var allIncidents = await IncidentApiService.GetAllIncidents(httpClient);
            Console.WriteLine("\nAll incidents:");
            foreach (Incident incident in allIncidents)
            {
                Console.WriteLine(UtilityHelper.FormatInfo(incident));
            }
        }

    }
}
