using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeknetProject.Models
{
    public class Materi
    {
        [Key]
        public int Id { get; set; }
        public string Judul { get; set; }
        public string Konten { get; set; }
    }
}
