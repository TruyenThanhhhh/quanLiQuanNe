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
    }
}
