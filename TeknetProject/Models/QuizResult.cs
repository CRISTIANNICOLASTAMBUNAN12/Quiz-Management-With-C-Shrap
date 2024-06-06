using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeknetProject.Areas.Identity.Data;

namespace TeknetProject.Models
{
    public class QuizResult
    {
        [Key]
        public int ResultID { get; set; }

        public int? QuizID { get; set; }

        [ForeignKey("QuizID")]
        public virtual Quizzes Quiz { get; set; }

        [StringLength(450)]
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public int? Score { get; set; }

        [Column(TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime TakenDate { get; set; }
    }
}
