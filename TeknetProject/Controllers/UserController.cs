using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using TeknetProject.Areas.Identity.Data;
using TeknetProject.Models;

namespace TeknetProject.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly QuizDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        Uri baseAddress = new Uri("https://localhost:7068/api/Materi");
        private readonly HttpClient _client;

        public UserController(ILogger<UserController> logger, QuizDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Periksa apakah TempData berisi pesan kesalahan
            if (TempData.ContainsKey("ErrorMessage"))
            {
                // Tambahkan pesan kesalahan sebagai ModelStateError
                ModelState.AddModelError("ErrorMessage", TempData["ErrorMessage"].ToString());
            }

            // Inisialisasi list untuk menyimpan data Materi
            List<Materi> materiList = new List<Materi>();

            try
            {
                // Kirim permintaan GET ke API
                HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress);

                // Periksa apakah respons berhasil
                if (response.IsSuccessStatusCode)
                {
                    // Baca konten respons sebagai string
                    string data = await response.Content.ReadAsStringAsync();

                    // Deserialisasi string JSON menjadi list Materi
                    materiList = JsonConvert.DeserializeObject<List<Materi>>(data);
                }
                else
                {
                    // Jika respons tidak berhasil, tambahkan pesan kesalahan ke ModelState
                    ModelState.AddModelError("APIError", "Failed to retrieve data from the API.");
                }
            }
            catch (Exception ex)
            {
                // Tangani kesalahan jika terjadi
                ModelState.AddModelError("Exception", $"An error occurred: {ex.Message}");
            }

            // Kembalikan view dengan data Materi
            return View(materiList);
        }


        [Authorize]
        public IActionResult BackToHome()
        {
            // Redirect ke halaman utama
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError("code", "Please enter a code.");
                return View();
            }

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Code == code);

            if (quiz == null)
            {
                ViewData["Error"] = "Kode kuis tidak ditemukan.";
                return View();
            }

            // Check if the quiz is reusable
            if (!quiz.IsReusable)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user's ID

                // Check if the user has already attempted this quiz
                var previousAttempt = await _context.QuizResults
                    .FirstOrDefaultAsync(r => r.QuizID == quiz.QuizID && r.UserID == userId);

                if (previousAttempt != null)
                {
                    ViewData["Error"] = "Anda sudah melakukan kuis ini.";
                    return View();
                }
            }

            // Redirect to quiz page passing quiz ID as parameter
            return RedirectToAction("TakeQuiz", new { id = quiz.QuizID });
        }
        [Authorize]
        public async Task<IActionResult> TakeQuiz(int id)
        {
            var quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.QuizID == id);

            if (quiz == null)
            {
                return NotFound();
            }

            // Pass the quiz object to the view
            return View(quiz);
        }
        // POST: Quiz/SubmitQuiz
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuiz(int quizId, Dictionary<int, string> answers)
        {
            var quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.QuizID == quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            // Check if any question is unanswered
            bool hasUnansweredQuestion = false;
            foreach (var question in quiz.Questions)
            {
                if (!answers.ContainsKey(question.QuestionID) || string.IsNullOrEmpty(answers[question.QuestionID]))
                {
                    hasUnansweredQuestion = true;
                    break;
                }
            }

            // If there are unanswered questions, return the view with an error message
            if (hasUnansweredQuestion)
            {
                ModelState.AddModelError("", "Please answer all questions before submitting.");
                return View("TakeQuiz", quiz);
            }

            // Calculate the score
            int score = 0;
            foreach (var question in quiz.Questions)
            {
                if (answers.TryGetValue(question.QuestionID, out string selectedAnswer) && selectedAnswer == question.CorrectAnswer)
                {
                    score++;
                }
            }

            // Calculate the total number of questions
            int totalQuestions = quiz.Questions.Count;

            // Calculate the score percentage
            double scorePercentage = (double)score / totalQuestions * 100;
            int? scorePercentageInt = Convert.ToInt32(scorePercentage);

            // Save the quiz result
            var quizResults = new QuizResult            
            {
                QuizID = quizId,
                UserID = _userManager.GetUserId(User), // Assuming you're using ASP.NET Identity for user management
                Score = scorePercentageInt
            };

            _context.QuizResults.Add(quizResults);
            await _context.SaveChangesAsync();

            // Redirect to the quiz result page after saving the quiz result
            return RedirectToAction("QuizHasil", new { quizId = quizResults.QuizID });
        }

        [Authorize]
        public async Task<IActionResult> QuizHasil(int quizId)
        {
            var quizResults = await _context.QuizResults
                .Include(q => q.User)
                .Where(q => q.QuizID == quizId) // Filter berdasarkan ID kuis
                .OrderByDescending(q => q.Score)
                .ToListAsync();

            return View(quizResults);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Show(int id)
        {
            try
            {
                Materi materi = new Materi();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    materi = JsonConvert.DeserializeObject<Materi>(data);
                }
                return View(materi);
            }
            catch (Exception ex)
            {
                TempData["error Message"] = ex.Message;
                return View();
            }


        }
    }
}
