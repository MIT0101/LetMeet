using System.ComponentModel.DataAnnotations;

namespace LetMeet.Data.Entites.Meetigs
{
    public class Meeting :GenrricEntity<Guid>
    {
        public static List<ValidationResult> validate<T>(T obj) {

            ValidationContext validationContext = new(obj,null,null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return validationResults;
        }


  
    }
}
