
using System.Text.Json.Serialization;

namespace LetMeet.Repositories
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResultState
    {

        Seccess,
        ValidationError,
        NotFound,
        ItemAlreadyExsist,
        DbError,
        Error

    }

    //Created,
    //    Deleted,
    //    Updated,
    //    Found,
    //    Seccess,
    //    NotFound,
    //    MultipleNotFound,
    //    ItemAlreadyExsist,
    //    DbError,
    //    ValidationError,
    //    UnExpectedError,
    //    Error


}
