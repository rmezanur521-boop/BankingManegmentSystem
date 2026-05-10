using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingManegmentSystem.Migrations
{
    public partial class Relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Transactions টেবিলে AccountId এর উপর Index তৈরি করো
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            // AccountId কে Foreign Key বানাও (Accounts টেবিলের সাথে সম্পর্ক)
            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Foreign Key ড্রপ করো
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_AccountId",
                table: "Transactions");

            // Index ড্রপ করো
            migrationBuilder.DropIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions");
        }
    }
}