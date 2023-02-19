using LetMeet.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository
{
    public class ErrorMessagesRepository : IErrorMessagesRepository
    {
        public string DbError(string message="")
        {
            if (message!="") {
                return message;
            }
            return "Data-Base Error";
        }

        public string DbError()
        {
            return DbError(string.Empty);
        }

        public string Error([NotNull] string message)
        {
            return message;
        }

        public string MultipleItemsNotFound(string message = "")
        {
            if (message != "")
            {
                return message;
            }
            return "No Items Found";
        }

        public string MultipleItemsNotFound()
        {

            return MultipleItemsNotFound(string.Empty);
        }

        public string SingleItemNotFound(string message = "")
        {
            if (message != "")
            {
                return message;
            }
            return "Item Not Found";
        }

        public string SingleItemNotFound()
        {
            return SingleItemNotFound(string.Empty);
        }

        public string UnExpectedError(string message="")
        {
            if (message != "")
            {
                return message;
            }
            return "UnExpected Erroe Happen";
        }

        public string UnExpectedError()
        {
            return UnExpectedError(string.Empty);
        }

        public string ValidationError(string message = "")
        {
            if (message != "")
            {
                return message;
            }
            return "Invalid Data";
        }

        public string ValidationError()
        {

            return ValidationError(string.Empty);
        }
    }
}
