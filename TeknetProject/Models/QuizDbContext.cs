using Microsoft.EntityFrameworkCore;
using TeknetProject.Areas.Identity.Data;
using TeknetProject.Models;

namespace TeknetProject.Models
{
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
        {
        }

        public DbSet<Quizzes> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }

        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<ApplicationUser> AspNetUsers { get; set; }

    }
}
