using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GroupPositionStatus
    {
        [EnumMember(Value = "Open")]
        Open,
        [EnumMember(Value = "Closed")]
        Closed
    }
}