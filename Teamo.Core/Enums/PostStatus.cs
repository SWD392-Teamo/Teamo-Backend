using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PostStatus
    {
        [EnumMember(Value = "Posted")]
        Posted,
        [EnumMember(Value = "Edited")]
        Edited,
    }
}