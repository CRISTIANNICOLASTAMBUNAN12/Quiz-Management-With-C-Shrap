using Microsoft.EntityFrameworkCore;

namespace Service_MataPelajaran.Models
{
    public class MateriContext:DbContext
    {
        public MateriContext(DbContextOptions<MateriContext> options): base(options) 
        {
        }
        public DbSet<Materi>Materi{get; set;}
    }
}
