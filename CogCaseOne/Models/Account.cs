using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CogCaseOne.Models
{
    public class AccountResponse
    {
        [JsonPropertyName("value")]
        public List<Account> Value { get; set; }
    }
    public class Account
    {
        [JsonPropertyName("accountid")]
        public string AccountId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
