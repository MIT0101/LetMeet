﻿using System.ComponentModel.DataAnnotations;

namespace LetMeet.Data.Dtos.MeetingsStaff
{
    public class MeetingTaskDto
    {
        [Required(ErrorMessage = "Meeting Title Is Required")]
        [StringLength(maximumLength: 500, MinimumLength = 4, ErrorMessage = "Meeting Task Must Be Between 4 and 500 for length")]
        public string title { get; set; } = string.Empty;


        [StringLength(maximumLength: 500, MinimumLength = 4, ErrorMessage = "Meeting Task Description Must Be Between 4 and 500 for length")]

        public string description { get; set; } = string.Empty;


        public bool isCompleted { get; set; } = false;

    }
}
