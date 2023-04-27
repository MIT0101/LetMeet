using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace LetMeet.Helpers
{
    public class GenricControllerHelper
    {
        // to init errors and messages
        public static void InitAndAssginErrorsAndMessagesForView(ViewDataDictionary viewData, ref List<string>? errors, ref List<string>? messages)
        {
            errors ??= new List<string>();
            messages ??= new List<string>();

            viewData[ViewStringHelper.Errors] = errors;
            viewData[ViewStringHelper.Messages] = messages;
        }

        public static Guid GetUserInfoId(ClaimsPrincipal user)
        {
            string userInfoIdStr = user.FindFirstValue(ClaimsNameHelper.UserInfoId);
            Guid userInfoId = Guid.Parse(userInfoIdStr);
            return userInfoId;

        }
        public static UserRole GetUserRole(ClaimsPrincipal user)
        {
            var roles = user.FindAll(ClaimTypes.Role);
            UserRole currentUserRole = (UserRole)Enum.Parse(typeof(UserRole), roles.First().Value);
            return currentUserRole;
        }
    }
}
