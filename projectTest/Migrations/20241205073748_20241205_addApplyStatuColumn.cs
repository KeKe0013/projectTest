using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectTest.Migrations
{
    /// <inheritdoc />
    public partial class _20241205_addApplyStatuColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "isApplyRenewal",
                table: "PartTime_order",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "0");

            migrationBuilder.AddColumn<string>(
                name: "isApplyResignation",
                table: "PartTime_order",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "0");

            migrationBuilder.AddColumn<string>(
                name: "isApplySalaryChange",
                table: "PartTime_order",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "0");

            migrationBuilder.AddColumn<string>(
                name: "projtype",
                table: "PartTime_order",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "projtype",
                table: "FullTime_order_detail",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "isApplyRenewal",
                table: "FullTime_order",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "0");

            migrationBuilder.AddColumn<string>(
                name: "isApplyResignation",
                table: "FullTime_order",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "0");

            migrationBuilder.AddColumn<string>(
                name: "isApplySalaryChange",
                table: "FullTime_order",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isApplyRenewal",
                table: "PartTime_order");

            migrationBuilder.DropColumn(
                name: "isApplyResignation",
                table: "PartTime_order");

            migrationBuilder.DropColumn(
                name: "isApplySalaryChange",
                table: "PartTime_order");

            migrationBuilder.DropColumn(
                name: "projtype",
                table: "PartTime_order");

            migrationBuilder.DropColumn(
                name: "projtype",
                table: "FullTime_order_detail");

            migrationBuilder.DropColumn(
                name: "isApplyRenewal",
                table: "FullTime_order");

            migrationBuilder.DropColumn(
                name: "isApplyResignation",
                table: "FullTime_order");

            migrationBuilder.DropColumn(
                name: "isApplySalaryChange",
                table: "FullTime_order");
        }
    }
}
