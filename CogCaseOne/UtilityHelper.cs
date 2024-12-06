using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CogCaseOne.Models;
using System.Text.Json;

namespace CogCaseOne
{
    internal class UtilityHelper
    {
        /// <summary>
        /// Creates JSON content from an object.
        /// </summary>
        /// <param name="payload">Object to serialise</param>
        /// <returns>Serialised JSON content</returns>
        public static StringContent CreateJsonContent(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }


        /// <summary>
        /// Formats entity information into readable string.
        /// </summary>
        /// <param name="entity">Entity to format</param>
        /// <returns>Formatted entity information as string</returns>
        /// <exception cref="Exception"></exception>
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
                    entityInfo.AppendLine($"City: {account.City}");
                    entityInfo.AppendLine($"Primary contact ID: {account.PrimaryContactId}");
                    if (account.PrimaryContact != null)
                    {
                        entityInfo.AppendLine($"Primary contact name: {account.PrimaryContact.FullName}");
                    }
                    break;
                case Contact contact:
                    entityInfo.AppendLine($"Contact ID: {contact.ContactId}");
                    entityInfo.AppendLine($"Full Name: {contact.FullName}");
                    entityInfo.AppendLine($"Company: {contact.Company}");
                    entityInfo.AppendLine($"Email: {contact.Email}");
                    entityInfo.AppendLine($"Phone: {contact.Phone}");
                    break;
                case Incident incident:
                    entityInfo.AppendLine($"Incident ID: {incident.IncidentId}");
                    entityInfo.AppendLine($"Ticket number: {incident.TicketNumber}");
                    entityInfo.AppendLine($"Title: {incident.Title}");
                    entityInfo.AppendLine($"Description: {incident.Description}");
                    entityInfo.AppendLine($"Customer ID: {incident.CustomerId}");
                    if (incident.CustomerAccount != null)
                    {
                        entityInfo.AppendLine($"Customer account name: {incident.CustomerAccount.Name}");
                    }
                    entityInfo.AppendLine($"Email: {incident.Email}");
                    entityInfo.AppendLine($"Status Code: {incident.StatusCode}");
                    entityInfo.AppendLine($"Priority Code: {incident.PriorityCode}");
                    entityInfo.AppendLine($"Created On: {incident.CreatedOn}");
                    break;
                default:
                    throw new Exception("Invalid entity type");
            }
            return entityInfo.ToString();
        }

    }
}
