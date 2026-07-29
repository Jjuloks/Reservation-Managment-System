using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Projekt_Zarzadzanie_Rezerwacjami.Models;

namespace Projekt_Zarzadzanie_Rezerwacjami.Data
{
    public class Projekt_Zarzadzanie_RezerwacjamiContext : DbContext
    {
        public Projekt_Zarzadzanie_RezerwacjamiContext (DbContextOptions<Projekt_Zarzadzanie_RezerwacjamiContext> options)
            : base(options)
        {
        }

        public DbSet<Projekt_Zarzadzanie_Rezerwacjami.Models.Rezerwacja> Rezerwacja { get; set; } = default!;
        public DbSet<Projekt_Zarzadzanie_Rezerwacjami.Models.Room> Room { get; set; } = default!;
        public DbSet<Uzytkownik> Uzytkownik { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Room>()
                .HasKey(r => r.SalaId);

            modelBuilder.Entity<Room>()
                .Property(r => r.SalaId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Room>().HasData(
                new Room { SalaId = 0, salaName = "S01", Capacity = 6, HasTv = true },
                new Room { SalaId = 1, salaName = "S02", Capacity = 8, HasTv = true },
                new Room { SalaId = 2, salaName = "S03", Capacity = 4, HasTv = false },
                new Room { SalaId = 3, salaName = "S04", Capacity = 3, HasTv = false },
                new Room { SalaId = 4, salaName = "S05", Capacity = 10, HasTv = false },
                new Room { SalaId = 5, salaName = "S06", Capacity = 4, HasTv = true });

            modelBuilder.Entity<Rezerwacja>()
                .HasOne(r => r.Room)
                .WithMany(rm => rm.Rezerwacje)
                .HasForeignKey(r => r.RoomId)
                .HasPrincipalKey(rm => rm.SalaId);

            modelBuilder.Entity<Uzytkownik>()
                .HasIndex(u => u.Login)
                .IsUnique();
        }
    }

}

