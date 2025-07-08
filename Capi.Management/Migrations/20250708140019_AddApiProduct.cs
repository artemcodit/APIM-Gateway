using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capi.Management.Migrations
{
    /// <inheritdoc />
    public partial class AddApiProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hosts",
                table: "Apis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Apis",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OpenApiSpec",
                table: "Apis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Apis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateTable(
                name: "ApiProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiApiProduct",
                columns: table => new
                {
                    ApiProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiApiProduct", x => new { x.ApiProductsId, x.ApisId });
                    table.ForeignKey(
                        name: "FK_ApiApiProduct_ApiProducts_ApiProductsId",
                        column: x => x.ApiProductsId,
                        principalTable: "ApiProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiApiProduct_Apis_ApisId",
                        column: x => x.ApisId,
                        principalTable: "Apis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiApiProduct_ApisId",
                table: "ApiApiProduct",
                column: "ApisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiApiProduct");

            migrationBuilder.DropTable(
                name: "ApiProducts");

            migrationBuilder.DropColumn(
                name: "Hosts",
                table: "Apis");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Apis");

            migrationBuilder.DropColumn(
                name: "OpenApiSpec",
                table: "Apis");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Apis");
        }
    }
}
