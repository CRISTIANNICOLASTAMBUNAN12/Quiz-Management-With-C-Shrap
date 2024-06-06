using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_MataPelajaran.Models;

namespace Service_MataPelajaran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriController : ControllerBase
    {
        private readonly MateriContext _dbContext;

        public MateriController(MateriContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public ActionResult GetMateris()
        {
            try
            {
                var materis = _dbContext.Materi.ToList();
                if (materis.Count == 0)
                {
                    return NotFound("Materi Tidak Tersedia");
                }
                return Ok(materis);
            }
            catch (Exception ex)
            {
            return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetMateri(int id)
        {
            try
            {
                var materi = _dbContext.Materi.Find(id);
                if (materi == null)
                {
                    return NotFound($"Materi Detail Not Found with id {id}");
                }
                return Ok(materi);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult PostMateri(Materi materi) {

            try
            {
                _dbContext.Add(materi);
                _dbContext.SaveChanges();
                return Ok("Materi Created");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

    
}

        [HttpPut]
        public ActionResult PutMateri(Materi materi)
        {
            if (materi == null || materi.Id == 0)
            {
                if (materi == null)
                {
                    return BadRequest("Model Data is invalid");
                }
                else if (materi.Id == 0)
                {
                    return BadRequest($"Materi id {materi.Id} is invalid");
                }
            }
                try
                {
                    var materis = _dbContext.Materi.Find(materi.Id);
                    if (materis == null)
                    {
                        return NotFound($"Materi not found with id {materi.Id}");
                    }
                    materis.Judul = materi.Judul;
                    materis.Konten = materi.Konten;
                    _dbContext.SaveChanges();
                    return Ok("Materi Updated");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        



        [HttpDelete("{id}")]
        public ActionResult DeleteMateri(int id)
        {
            try
            {
                var materi = _dbContext.Materi.Find(id);
                if (materi == null)
                {
                    return NotFound($"Materi Not Found");
                }
                _dbContext.Materi.Remove(materi);
                _dbContext.SaveChanges();
                return Ok("Materi Deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
