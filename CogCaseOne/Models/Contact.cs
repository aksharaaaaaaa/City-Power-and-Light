using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Identity.Client;  // Microsoft Authentication Library (MSAL)
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CogCaseOne.Models;
using System.Security.Principal;

namespace CogCaseOne.Models
{
    public class ContactResponse
    {
        [JsonPropertyName("value")]
        public List<Contact> Value { get; set; }
    }
    public class Contact
    {
        [JsonPropertyName("emailaddress1")]
        public string Email { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("fullname")]
        public string FullName { get; set; }

        [JsonPropertyName("contactid")]
        public Guid ContactId { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("telephone1")]
        public string Phone { get; set; }


    }
}