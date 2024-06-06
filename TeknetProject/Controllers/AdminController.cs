using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeknetProject.Areas.Identity.Data;
using TeknetProject.Models;


namespace TeknetProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuizDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor with QuizDbContext dependency injection
        public AdminController(QuizDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            var quizzes = _context.Quizzes.ToList(); // Ambil daftar kuis dari basis data
            var users = await _userManager.Users.ToListAsync(); // Ambil daftar pengguna dari ASP.NET Identity

            // Ambil daftar pengguna yang tidak memiliki peran tertentu (misalnya, "admin")
            var usersWithoutRoleA = new List<ApplicationUser>();
            var adminUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, "admin");
                if (!isInRole)
                {
                    usersWithoutRoleA.Add(user);
                }
                else
                {
                    adminUsers.Add(user);
                }
            }

            var viewModel = new QuizViewModel
            {
                MyQuizzes = quizzes,
                MyUser = usersWithoutRoleA,
                MyAdmin = adminUsers
            };

            return View(viewModel); // Kirim view model ke view
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Murid()
        {
            var quizzes = _context.Quizzes.ToList(); // Ambil daftar kuis dari basis data
            var users = await _userManager.Users.ToListAsync(); // Ambil daftar pengguna dari ASP.NET Identity

            // Ambil daftar pengguna yang tidak memiliki peran tertentu (misalnya, "RoleA")
            var usersWithoutRoleA = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, "admin");
                if (!isInRole)
                {
                    usersWithoutRoleA.Add(user);
                }
            }

            var viewModel = new QuizViewModel
            {
                MyQuizzes = quizzes,
                MyUser = usersWithoutRoleA
            };

            return View(viewModel); // Kirim view model ke view
        }

    }
}
