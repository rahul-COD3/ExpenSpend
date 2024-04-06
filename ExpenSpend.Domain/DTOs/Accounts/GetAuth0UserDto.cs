using System.Text.Json.Serialization;

namespace ExpenSpend.Domain.DTOs.Accounts
{
    public class GetAuth0UserDto
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("picture")]
        public string? Picture { get; set; }
        public string? JwtLoginToken { get; set; }
    }
}
