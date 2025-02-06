using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GroupMemberRole
    {
        [EnumMember(Value = "Member")]
        Member,
        [EnumMember(Value = "Leader")]
        Leader
    }
}