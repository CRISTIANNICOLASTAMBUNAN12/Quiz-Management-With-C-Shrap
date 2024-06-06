    using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Pastikan menggunakan directive ini

namespace Service_MataPelajaran.Models
{
    public class Materi
    {

        public int Id { get; set; }
        public string Judul { get; set; }
        public string Konten { get; set; }
        
    }
}
