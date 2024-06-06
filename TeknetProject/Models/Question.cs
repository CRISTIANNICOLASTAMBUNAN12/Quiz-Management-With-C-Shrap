using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TeknetProject.Models
{
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }
        public int QuizID { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

  
        public string OptionA { get; set; }


        public string OptionB { get; set; }

   
        public string OptionC { get; set; }

   
        public string OptionD { get; set; }

        public string? OptionE { get; set; }

    }

}
