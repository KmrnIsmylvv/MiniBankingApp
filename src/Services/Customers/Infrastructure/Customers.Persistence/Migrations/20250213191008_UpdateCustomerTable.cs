using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "Customers",
                type: "double precision",
                nullable: false,
                defaultValue: 50.0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 50m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 50m,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 50.0);
        }
    }
}
