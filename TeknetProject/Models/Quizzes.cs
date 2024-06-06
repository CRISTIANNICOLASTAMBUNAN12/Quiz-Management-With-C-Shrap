using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeknetProject.Areas.Identity.Data;

namespace TeknetProject.Models
{
    public class Quizzes
    {
        [Key]
        public int QuizID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(255)]
        public string Description { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int DurationInSeconds { get; set; } // Durasi kuis dalam detik

        public bool IsReusable { get; set; } // Menandai apakah kuis dapat diikuti berkali-kali atau hanya sekali

        public string Code { get; set; }
 
        [ForeignKey("QuizID")] // Menunjukkan bahwa properti Questions menggunakan kolom QuizID sebagai kunci asing
        public List<Question>? Questions { get; set; }
    }

}
