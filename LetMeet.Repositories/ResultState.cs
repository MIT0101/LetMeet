
using System.Text.Json.Serialization;

namespace LetMeet.Repositories
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResultState
    {
        Created,
        Deleted,
        Updated,
        Found,
        Seccess,
        NotFound,
        ItemAlreadyExsist,
        DbError,
        ValidationError,
        UnExpectedError,
    }

   
}
