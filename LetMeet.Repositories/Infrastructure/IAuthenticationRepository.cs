using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface IAuthenticationRepository<TUser, TRole, TResult> 
        where TUser : class 
        where TResult : SignInResult
    {
        public Task<TResult> SignInPasswordAsync(TUser user,string password , bool isPersistent , bool lockoutOnFailure);
        public Task<TResult> SignInPasswordAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);

        public Task<TResult> CanSignIn(TUser user);
        public Task<TResult> SignOutAsync(TUser user);

        public Task<TResult> IsLockedOut(TUser user);

        public Task<TResult> LockedOut(TUser user);

        public Task<TResult> RefreshSignInAsync(TUser user);

        public Task<TResult> ResetLockout(TUser user);  

    }
}
