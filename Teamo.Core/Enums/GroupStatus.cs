using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GroupStatus
    {
        [EnumMember(Value = "Recruiting")]
        Recruiting,
        [EnumMember(Value = "Full")]
        Full,
        [EnumMember(Value = "Archived")]
        Archived,
        [EnumMember(Value = "Deleted")]
        Deleted
    }
}