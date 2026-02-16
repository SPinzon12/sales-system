using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price", "Stock", "ImageUrl", "IsActive" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "Laptop Dell XPS 15", 1299.99m, 25, "https://static.wixstatic.com/media/a9655c_ce698c21fa404c7eaeda93b9c7ffdc0b~mv2.png/v1/fill/w_1080,h_1080,al_c,q_90,enc_avif,quality_auto/a9655c_ce698c21fa404c7eaeda93b9c7ffdc0b~mv2.png", true });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price", "Stock", "ImageUrl", "IsActive" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "Mouse Logitech MX Master", 99.99m, 50, "https://logitech.com.mx/cdn/shop/files/MX_MASTER_3S_Graphite_01.png", true });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price", "Stock", "ImageUrl", "IsActive" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "Teclado Mecánico Corsair K70", 149.99m, 30, "https://example.com/corsair-k70.jpg", true });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price", "Stock", "ImageUrl", "IsActive" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), "Monitor Samsung 27''", 349.99m, 20, "https://example.com/samsung-monitor.jpg", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));
        }
    }
}
