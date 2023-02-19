using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface IErrorMessagesRepository
    {
        public string DbError();
        public string DbError(string message);

        public string Error([NotNull] string message);

        public string SingleItemNotFound();
        public string SingleItemNotFound(string message);

        public string MultipleItemsNotFound();
        public string MultipleItemsNotFound(string message);

        public string ValidationError();
        public string ValidationError(string message);

        public string UnExpectedError();
        public string UnExpectedError(string message);

    }
}
