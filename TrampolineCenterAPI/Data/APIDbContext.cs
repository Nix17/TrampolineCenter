using Microsoft.EntityFrameworkCore;
using TrampolineCenterAPI.Models;

namespace TrampolineCenterAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

       public DbSet<Client> Clients {  get; set; }
    }
}
