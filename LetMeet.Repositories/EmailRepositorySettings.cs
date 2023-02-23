using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories
{
    public class EmailRepositorySettings
    {

        public static string SectionName = "EmailRepositorySettings";

        [Required]
        public string Mail { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Host { get; set; }
        [Required]
        public int Port { get; set; }
    }
}
