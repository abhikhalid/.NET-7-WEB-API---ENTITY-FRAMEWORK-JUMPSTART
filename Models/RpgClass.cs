using System.Text.Json.Serialization;

namespace Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Knight = 1,

        Rage = 2,

        Cleric = 3
    }
}