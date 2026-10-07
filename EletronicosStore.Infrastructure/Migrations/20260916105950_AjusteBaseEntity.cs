using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EletronicosStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjusteBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TB_CATEGORIA",
                columns: new[] { "Id", "Nome" },
                values: new object[] { 1, "Equipamentos" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TB_CATEGORIA",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
