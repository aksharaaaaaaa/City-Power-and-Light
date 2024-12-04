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

        [JsonPropertyName("fullname")]
        public string FullName { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("contactid")]
        public string ContactId { get; set; }
    }
}
