namespace LetMeet.Helpers
{
    public class DevelopmentHelper
    {
        public static void SaveAccountToFile(string email, string password,string role)
        {

            string filePath = "Accounts.txt";

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"{email},{password},{role}");
            }

        }
    }
}
