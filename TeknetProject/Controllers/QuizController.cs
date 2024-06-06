using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks; // Add this namespace for Task
using TeknetProject.Areas.Identity.Data;
using TeknetProject.Models;

namespace TeknetProject.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;
        private readonly QuizDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(ILogger<QuizController> logger, QuizDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET: Quiz/CreateQuiz
        [Authorize(Roles = "Admin")]
        public IActionResult CreateQuiz()
        {
            // Check if TempData contains values from previous form submission
            if (TempData["QuizData"] != null)
            {
                // Restore form data from TempData
                Quizzes quizData = (Quizzes)TempData["QuizData"];
                return View(quizData);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuiz([Bind("Title, Description, DurationInSeconds, IsReusable,Code")] Quizzes quiz)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    string currentUsername = user.FirstName;

                    // Set properties of the quizzes object
                    quiz.CreatedBy = currentUsername;
                    quiz.CreatedDate = DateTime.Now;

                    // Add quiz to the context
                    _context.Quizzes.Add(quiz);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(ListQuiz)); // Redirect to the list after creation.
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately
                    ModelState.AddModelError("", "An error occurred while creating the quiz.");
                    _logger.LogError(ex, "An error occurred while creating the quiz.");
                    return View("CreateQuiz", quiz);
                }
            }
            return View("CreateQuiz", quiz); // If model state is not valid, return to the create quiz page with the model data.
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditQuiz(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    // Find the quiz by id
    var quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.QuizID == id);

    if (quiz == null)
    {
        return NotFound();
    }

    return View(quiz);
}





        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuiz(int id, [Bind("QuizID,Title,Description")] Quizzes quiz)
        {
            if (id != quiz.QuizID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    string currentUsername = user.FirstName;

                    // Set the CreatedBy and CreatedDate properties of the quizzes object
                    quiz.CreatedBy = currentUsername;
                    quiz.CreatedDate = DateTime.Now;
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListQuiz));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.QuizID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(quiz);
        }
        [Authorize(Roles = "Admin")]
        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.QuizID == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuiz(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the quiz by id
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.QuizID == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var quiz = await _context.Quizzes.FindAsync(id);
                if (quiz == null)
                {
                    return NotFound();
                }

                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListQuiz)); // Redirect to the list after deletion.
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                ModelState.AddModelError("", "An error occurred while deleting the quiz.");
                _logger.LogError(ex, "An error occurred while deleting the quiz.");
                return View("DeleteQuiz", await _context.Quizzes.FindAsync(id));
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ListQuiz()
        {
            var quizzes = _context.Quizzes.ToList();
            return View(quizzes);
        }
        // GET: Quiz/RunQuiz
        public IActionResult RunQuiz()
        {
            return View();
        }

        // POST: Quiz/RunQuiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunQuiz(string code)
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




        // GET: Quiz/TakeQuiz/{id}
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuiz(int quizId, Dictionary<int, string> answers)
        {
            var quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.QuizID == quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            // Check if all questions are answered
            foreach (var question in quiz.Questions)
            {
                if (!answers.ContainsKey(question.QuestionID) || string.IsNullOrEmpty(answers[question.QuestionID]))
                {
                    ModelState.AddModelError("", $"Please answer question {question.QuestionID} before submitting.");
                    return View("TakeQuiz", quiz); // Return the TakeQuiz view with error message
                }
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
            return RedirectToAction("QuizResult", new { quizId = quizResults.QuizID });
        }


        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> QuizResult(int quizId)
        {
            var quizResults = await _context.QuizResults
                .Include(q => q.User)
                .Where(q => q.QuizID == quizId) // Filter berdasarkan ID kuis
                .OrderByDescending(q => q.Score)
                .ToListAsync();

            return View(quizResults);
        }
                [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteResult(int id, int quizId)
        {
            // Lakukan logika untuk menghapus data dengan ID yang diberikan
            // Contoh:
            var quizResult = _context.QuizResults.FirstOrDefault(q => q.ResultID == id);
            if (quizResult != null)
            {
                _context.QuizResults.Remove(quizResult);
                _context.SaveChanges();
            }

            return RedirectToAction("QuizResult", new { quizId = quizId }); // Redirect kembali ke halaman QuizResult dengan menyertakan ID kuis
        }




    }
}