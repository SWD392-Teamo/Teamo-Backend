using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Teamo.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PostPrivacy
    {
        [EnumMember(Value = "Public")]
        Public,
        [EnumMember(Value = "Private")]
        Private
    }
}