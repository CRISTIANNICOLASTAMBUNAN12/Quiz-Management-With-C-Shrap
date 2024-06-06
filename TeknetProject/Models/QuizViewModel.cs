using Microsoft.AspNetCore.Identity;
using TeknetProject.Areas.Identity.Data;

namespace TeknetProject.Models
{
    public class QuizViewModel
    {
        public List <Quizzes> MyQuizzes { get; set; }
        public List<ApplicationUser> MyUser { get; set; }

        public List<ApplicationUser> MyAdmin { get; set; }
        public List<QuizResult> MyResult { get; set; }

    }
}
