using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SalesMonitor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Продукты",
                columns: table => new
                {
                    IDпродукта = table.Column<int>(name: "ID продукта", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Названиепродукта = table.Column<string>(name: "Название продукта", type: "character varying(70)", maxLength: 70, nullable: false),
                    Себестоимость = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Ценапродажи = table.Column<decimal>(name: "Цена продажи", type: "numeric(18,2)", nullable: false),
                    Впродаже = table.Column<bool>(name: "В продаже", type: "boolean", nullable: false),
                    Датадобавлениявассортимент = table.Column<DateTime>(name: "Дата добавления в ассортимент", type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Продукты", x => x.IDпродукта);
                });

            migrationBuilder.CreateTable(
                name: "Активность клиентов",
                columns: table => new
                {
                    IDпродукта_ = table.Column<int>(name: "ID продукта_", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDпродукта = table.Column<int>(name: "ID продукта", type: "integer", nullable: false),
                    Датапоследнейпродажи = table.Column<DateTime>(name: "Дата последней продажи", type: "date", nullable: true),
                    Среднийинтервалмеждузаказами = table.Column<decimal>(name: "Средний интервал между заказами", type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Активность клиентов", x => x.IDпродукта_);
                    table.ForeignKey(
                        name: "FK_Активность клиентов_Продукты_ID продукта",
                        column: x => x.IDпродукта,
                        principalTable: "Продукты",
                        principalColumn: "ID продукта",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Продажи",
                columns: table => new
                {
                    IDпродажи = table.Column<int>(name: "ID продажи", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDпродукта = table.Column<int>(name: "ID продукта", type: "integer", nullable: false),
                    Датапродажи = table.Column<DateTime>(name: "Дата продажи", type: "date", nullable: false),
                    Количество = table.Column<int>(type: "integer", nullable: false),
                    Общаясуммачека = table.Column<decimal>(name: "Общая сумма чека", type: "numeric(18,2)", nullable: false),
                    Маржа = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Продажи", x => x.IDпродажи);
                    table.ForeignKey(
                        name: "FK_Продажи_Продукты_ID продукта",
                        column: x => x.IDпродукта,
                        principalTable: "Продукты",
                        principalColumn: "ID продукта",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Активность клиентов_ID продукта",
                table: "Активность клиентов",
                column: "ID продукта",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Продажи_ID продукта",
                table: "Продажи",
                column: "ID продукта");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Активность клиентов");

            migrationBuilder.DropTable(
                name: "Продажи");

            migrationBuilder.DropTable(
                name: "Продукты");
        }
    }
}
