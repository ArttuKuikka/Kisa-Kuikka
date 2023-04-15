using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kisa_Kuikka.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Kisa_Kuikka.Models.DynamicAuth;
using Microsoft.AspNetCore.Identity;

namespace Kisa_Kuikka.Data
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
        public DbSet<Kisa_Kuikka.Models.Kisa> Kisa { get; set; } = default!;
        public DbSet<Kisa_Kuikka.Models.Vartio> Vartio { get; set; } = default!;
        public DbSet<Kisa_Kuikka.Models.Sarja> Sarja { get; set; } = default!;
        public DbSet<Kisa_Kuikka.Models.Rasti> Rasti { get; set; } = default!;
        public DbSet<Kisa_Kuikka.Models.Tehtava> Tehtava { get; set; } = default!;
        public DbSet<Kisa_Kuikka.Models.Tiedosto> Tiedosto { get; set;} = default!;
        public DbSet<Kisa_Kuikka.Models.TehtavaVastaus> TehtavaVastaus { get; set;} = default!;
        public DbSet<Kisa_Kuikka.Models.TagSkannaus> TagSkannaus { get; set;} = default!;
        public DbSet<Kisa_Kuikka.Models.Tilanne> Tilanne { get; set;} = default!;
        public DbSet<Kisa_Kuikka.Models.VapidDetailsWithId> VapidStore { get; set;} = default!;
        public DbSet<Kisa_Kuikka.Models.Ilmoitus> Ilmoitukset { get; set;} = default!;
    }
}