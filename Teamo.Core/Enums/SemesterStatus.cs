using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SemesterStatus
    {
        [EnumMember(Value = "Opened")]
        Opened,
        [EnumMember(Value = "Closed")]
        Closed,
        [EnumMember(Value = "Upcoming")]
        Upcoming
    }
}