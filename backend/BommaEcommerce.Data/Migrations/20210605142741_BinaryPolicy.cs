using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class BinaryPolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FirstPolicyGuid",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SecondPolicyGuid",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_FirstPolicyGuid",
                table: "Policies",
                column: "FirstPolicyGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_SecondPolicyGuid",
                table: "Policies",
                column: "SecondPolicyGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Policies_FirstPolicyGuid",
                table: "Policies",
                column: "FirstPolicyGuid",
                principalTable: "Policies",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Policies_SecondPolicyGuid",
                table: "Policies",
                column: "SecondPolicyGuid",
                principalTable: "Policies",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Policies_FirstPolicyGuid",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Policies_SecondPolicyGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_FirstPolicyGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_SecondPolicyGuid",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "FirstPolicyGuid",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "SecondPolicyGuid",
                table: "Policies");
        }
    }
}
