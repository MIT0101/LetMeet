using LetMeet.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository
{
    public class PasswordGenrationRepository : IPasswordGenrationRepository
    {
       private string validChars;
       private Random random=new Random();
       public static string DefaultValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&_";


        public PasswordGenrationRepository(string validChars)
        {
            this.validChars = validChars;
        }

        public PasswordGenrationRepository()
        {
            validChars = DefaultValidChars;
        }

        public async Task<string> GenerateRandomPassword(int length=16)
        {
            return await Task.Run(() =>
            {
                var password = new char[length];

                for (int i = 0; i < length; i++)
                {
                    password[i] = validChars[random.Next(validChars.Length)];
                }

                return new string(password);
            });
        }
    }
}
