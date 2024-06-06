using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TeknetProject.Models;

namespace TeknetProject.Controllers
{
    public class QuestionController : Controller
    {
        private readonly QuizDbContext _context;
        private readonly ILogger<QuestionController> _logger; // Define logger

        public QuestionController(QuizDbContext context, ILogger<QuestionController> logger)
        {
            _context = context;
            _logger = logger; // Initialize logger
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CreateQuestion(int quizId)
        {
            TempData["QuizId"] = quizId;
            var question = new Question
            {
                QuizID = quizId
            };
            return View(question);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                // Menetapkan jawaban yang benar berdasarkan nilai properti IsCorrect
                if (question.CorrectAnswer == "Option A")
                {
                    question.OptionA = $"{question.OptionA} ";
                }
                else if (question.CorrectAnswer == "Option B")
                {
                    question.OptionB = $"{question.OptionB}";
                }
                else if (question.CorrectAnswer == "Option C")
                {
                    question.OptionC = $"{question.OptionC}";
                }
                else if (question.CorrectAnswer == "Option D")
                {
                    question.OptionD = $"{question.OptionD}";
                }

                _context.Add(question);
                await _context.SaveChangesAsync();

                int quizId = (int)TempData["QuizId"];
                return RedirectToAction("EditQuiz", "Quiz", new { id = quizId });
            }

            return View(question);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuestion(int? questionId)
        {
              if (questionId == null)
            {
                return NotFound();
            }

            // Find the quiz by id
            var question = await _context.Questions.FirstOrDefaultAsync(m => m.QuestionID == questionId);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int questionId)
        {
            try
            {
                var question = await _context.Questions.FindAsync(questionId);
                if (question == null)
                {
                    return NotFound();
                }

                int quizId = question.QuizID;
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();

                return RedirectToAction("EditQuiz", "Quiz", new { id = quizId });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while deleting the question.");

                // Handle exceptions appropriately
                ModelState.AddModelError("", "An error occurred while deleting the question.");
                return View("DeleteQuestion", await _context.Questions.FindAsync(questionId));
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditQuestion(int? questionId)
        {
            if (questionId == null)
            {
                return NotFound();
            }

            // Cari pertanyaan berdasarkan ID
            var question = await _context.Questions.FindAsync(questionId);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(int questionId, int quizId, Question question)
        {
            if (questionId != question.QuestionID)
            {
                return NotFound();
            }

            // Ensure that QuizID has a value
            if (question.QuizID == 0)
            {
                ModelState.AddModelError("", "QuizID is required.");
                return View(question);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update question in the database
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.QuestionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to EditQuiz action with the provided quizId
                return RedirectToAction("EditQuiz", "Quiz", new { id = quizId });
            }

            return View(question);
        }


        [Authorize(Roles = "Admin")]
        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.QuestionID == id);
        }

    }

}
