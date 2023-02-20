namespace LetMeet.Test
{
    public class Test1
    {
        public static void SaveAccountToFile(string email, string password) {

                string filePath = "Accounts.txt";

                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.WriteLine($"{email},{password}");
                }
            
        }
    }
}