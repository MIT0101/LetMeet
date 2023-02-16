using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories
{
    public class RepositoryValidationResult
    {
        public bool IsValid { get; set; }
            
       public List<ValidationResult> ValidationErrors { get; init; }

        public static RepositoryValidationResult DataAnnotationsValidation<T>(T entity)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (entity is null) {
                validationResults.Add(new ValidationResult("Object Is Null."));
                return new RepositoryValidationResult() { IsValid = false, ValidationErrors = validationResults };

            }
               bool isValid= Validator.TryValidateObject(entity, new ValidationContext(entity), validationResults, true);

            return new RepositoryValidationResult() { IsValid= isValid, ValidationErrors=validationResults};
        }
       
    }
}
