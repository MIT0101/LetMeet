using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories
{
    public class RepositoryResult<TResult> where TResult : class 
    {
        public bool Success { get; set; }

        public ResultState State { get; set; }

        public TResult? Result { get; set; }

        public List<ValidationResult>? ValidationErrors { get; set; }

        public List<string> ErrorMessages { get; set; }



        public RepositoryResult(bool success,ResultState state, TResult? result, List<ValidationResult>? validationErrors, List<string> errorMessages)
        {
            Success = success;
            State  = state;
            Result = result;
            ValidationErrors = validationErrors;
            ErrorMessages = errorMessages;
            if (errorMessages == null)
            {
                ErrorMessages = new List<string>();
            }
            if (ValidationErrors == null)
            {
                ValidationErrors = new List<ValidationResult>();
            }
        }

        public static RepositoryResult<TResult> FailureValidationResult(List<ValidationResult>? validationErrors, List<string> errorMessages =null)
        {

            return new RepositoryResult<TResult>(success: false,state :ResultState.ValidationError, 
                result:null, validationErrors: validationErrors, errorMessages: errorMessages);
        }

        public static RepositoryResult<TResult> FailureResult(ResultState state,List<ValidationResult>? validationErrors, List<string> errorMessages = null) {

            return new RepositoryResult<TResult>(success: false,state:state,
                result: null, validationErrors: validationErrors, errorMessages: errorMessages);
        }

        public static RepositoryResult<TResult> SuccessResult(ResultState state,TResult result)
        {
            return new RepositoryResult<TResult>(success: true,state :state, 
                result: result, validationErrors: null, errorMessages: null);
        }

      
    }
}
