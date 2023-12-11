using Microsoft.EntityFrameworkCore;
using TrampolineCenterAPI.Models;

namespace TrampolineCenterAPI.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions options) : base(options)
        {
        }

       public DbSet<Client> Clients {  get; set; }
    }
}
