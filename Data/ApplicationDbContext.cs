using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kipa_plus.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Kipa_plus.Models.DynamicAuth;
using Microsoft.AspNetCore.Identity;

namespace Kipa_plus.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if(databaseCreator!= null ) 
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if(!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine("ApplicationDbContext Error: " + ex.Message);
            }
        }
        public DbSet<Kipa_plus.Models.Kisa> Kisa { get; set; } = default!;
        public DbSet<Kipa_plus.Models.Vartio> Vartio { get; set; } = default!;
        public DbSet<Kipa_plus.Models.Sarja> Sarja { get; set; } = default!;
        public DbSet<Kipa_plus.Models.Rasti> Rasti { get; set; } = default!;
        public DbSet<Kipa_plus.Models.Tehtava> Tehtava { get; set; } = default!;
        public DbSet<Kipa_plus.Models.Tiedosto> Tiedosto { get; set;} = default!;
        public DbSet<Kipa_plus.Models.TehtavaVastaus> TehtavaVastaus { get; set;} = default!;
        public DbSet<Kipa_plus.Models.TagSkannaus> TagSkannaus { get; set;} = default!;
        public DbSet<Kipa_plus.Models.Tilanne> Tilanne { get; set;} = default!;
        public DbSet<Kipa_plus.Models.VapidDetailsWithId> VapidStore { get; set;} = default!;
        public DbSet<Kipa_plus.Models.Ilmoitus> Ilmoitukset { get; set;} = default!;
    }
}