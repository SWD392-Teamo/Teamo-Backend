using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MajorStatus
    {
        [EnumMember(Value = "Active")]
        Active,
        [EnumMember(Value = "Inactive")]
        Inactive
    }
}
