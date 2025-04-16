using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanLiQuanNe.Migrations
{
    /// <inheritdoc />
    public partial class themnguoidungvasudungmay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nguoiDung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    passWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sdt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false),
                    soDu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nguoiDung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suDungMay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    thoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    thoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    tongTien = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suDungMay", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nguoiDung");

            migrationBuilder.DropTable(
                name: "suDungMay");
        }
    }
}
