using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CogCaseOne.Models
{
    public class IncidentResponse
    {
        [JsonPropertyName("value")]
        public List<Incident> Value { get; set; }
    }
    public class Incident
    {
        [JsonPropertyName("ticketnumber")]
        public string TicketNumber { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("incidentid")]
        public string IncidentId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("emailaddress")]
        public string Email { get; set; }

        [JsonPropertyName("_customerid_value")]
        public string CustomerId { get; set; }

        [JsonPropertyName("statuscode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("prioritycode")]
        public int PriorityCode { get; set; }

        [JsonPropertyName("createdon")]
        public string CreatedOn { get; set; }

        [JsonPropertyName("customerid_account")]
        public Account CustomerAccount { get; set; }
    }
}