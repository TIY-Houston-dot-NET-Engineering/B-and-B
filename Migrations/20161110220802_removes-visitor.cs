using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace bnb2.Migrations
{
    public partial class removesvisitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Visitor_VisitorsId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_VisitorsId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VisitorsId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Visitor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visitor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitor", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "VisitorsId",
                table: "Messages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_VisitorsId",
                table: "Messages",
                column: "VisitorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Visitor_VisitorsId",
                table: "Messages",
                column: "VisitorsId",
                principalTable: "Visitor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
