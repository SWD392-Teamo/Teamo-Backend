using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SemesterStatus
    {
        [EnumMember(Value = "Ongoing")]
        Ongoing,
        [EnumMember(Value = "Past")]
        Past,
        [EnumMember(Value = "Upcoming")]
        Upcoming
    }
}