using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanLiQuanNe.Models;

namespace quanLiQuanNe.Data
{
    public class quanLiQuanNeContext : DbContext
    {
        public quanLiQuanNeContext (DbContextOptions<quanLiQuanNeContext> options)
            : base(options)
        {
        }

        public DbSet<quanLiQuanNe.Models.mayTinh> mayTinh { get; set; } = default!;
        public DbSet<quanLiQuanNe.Models.nguoiDung> nguoiDung { get; set; } = default!;
        public DbSet<quanLiQuanNe.Models.suDungMay> suDungMay { get; set; } = default!;
        public DbSet<Game> Game { get; set; } // Thêm DbSet cho Game

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure suDungMay relationships
            modelBuilder.Entity<suDungMay>()
                .HasOne(s => s.nguoiDung)
                .WithMany()
                .HasForeignKey(s => s.maNguoiDung)
                .HasPrincipalKey(n => n.userName)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<suDungMay>()
                .HasOne(s => s.mayTinh)
                .WithMany()
                .HasForeignKey(s => s.maMay)
                .HasPrincipalKey(m => m.id.ToString())
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
