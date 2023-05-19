using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories
{
    public class RepositoryDataSettings
    {
        public const string NameOfSection = nameof(RepositoryDataSettings);

        [Range(minimum:1,maximum:int.MaxValue)]
        public int MaxResponsesPerTime { get; init; } = int.MaxValue;

        public long MaxProfileImageSizeInKb { get; set; } = 300;
    }
}
