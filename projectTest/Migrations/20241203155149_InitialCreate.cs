using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectTest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepBudget",
                columns: table => new
                {
                    depno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    depid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    projectno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fundno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fundname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    budget_remain = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sourcetype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    leader = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    leaderid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    depcname = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "FullTime_order",
                columns: table => new
                {
                    FullTime_orderNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicationOffice = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ApplicationTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EmployedName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Birthday = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmployedStartDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmployedEndDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EducationLevel = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SalaryGrade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Salary = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SalaryChangeReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResignDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SalaryChangeDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FullTime_order", x => x.FullTime_orderNo);
                });

            migrationBuilder.CreateTable(
                name: "ManagerAuthority",
                columns: table => new
                {
                    EmployeeNo = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Authority = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerAuthority", x => new { x.Authority, x.EmployeeNo });
                });

            migrationBuilder.CreateTable(
                name: "PartTime_order",
                columns: table => new
                {
                    PartTime_orderNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicationOffice = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ApplicationTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PlanNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CommissionName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PlanStartDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PlanEndDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LeaderName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmployedName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Birthday = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmployedStartDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmployedEndDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StudyStatus = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StudyStatusMemo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    EducationLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SalaryCategory = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Salary = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsTechPerson = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ResignDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SalaryChangeDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTime_order", x => x.PartTime_orderNo);
                });

            migrationBuilder.CreateTable(
                name: "ProjectBudget",
                columns: table => new
                {
                    projectno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    projectname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fundno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fundname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    budget_remain = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sourcetype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    projno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pjttype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    leadercard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    leadername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    com = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    comname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    enddate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    extenddate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Studentinfo",
                columns: table => new
                {
                    studentID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    birthday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pclass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sclass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "FullTime_order_detail",
                columns: table => new
                {
                    FullTime_orderNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AccountNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlanNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CommissionName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PlanStartDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PlanEndDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LeaderName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FullTime_order_detail", x => new { x.FullTime_orderNo, x.AccountNo });
                    table.ForeignKey(
                        name: "FK_FullTime_Order_detail_FullTime_Order",
                        column: x => x.FullTime_orderNo,
                        principalTable: "FullTime_order",
                        principalColumn: "FullTime_orderNo",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepBudget");

            migrationBuilder.DropTable(
                name: "FullTime_order_detail");

            migrationBuilder.DropTable(
                name: "ManagerAuthority");

            migrationBuilder.DropTable(
                name: "PartTime_order");

            migrationBuilder.DropTable(
                name: "ProjectBudget");

            migrationBuilder.DropTable(
                name: "Studentinfo");

            migrationBuilder.DropTable(
                name: "FullTime_order");
        }
    }
}
