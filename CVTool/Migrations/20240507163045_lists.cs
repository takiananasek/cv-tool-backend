using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVTool.Migrations
{
    public partial class lists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_Resumes_ResumeId",
                table: "Component");

            migrationBuilder.DropForeignKey(
                name: "FK_ComponentEntries_Component_ComponentId",
                table: "ComponentEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Component",
                table: "Component");

            migrationBuilder.RenameTable(
                name: "Component",
                newName: "Components");

            migrationBuilder.RenameIndex(
                name: "IX_Component_ResumeId",
                table: "Components",
                newName: "IX_Components_ResumeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Components",
                table: "Components",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentEntries_Components_ComponentId",
                table: "ComponentEntries",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Resumes_ResumeId",
                table: "Components",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentEntries_Components_ComponentId",
                table: "ComponentEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_Resumes_ResumeId",
                table: "Components");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Components",
                table: "Components");

            migrationBuilder.RenameTable(
                name: "Components",
                newName: "Component");

            migrationBuilder.RenameIndex(
                name: "IX_Components_ResumeId",
                table: "Component",
                newName: "IX_Component_ResumeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Component",
                table: "Component",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Resumes_ResumeId",
                table: "Component",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentEntries_Component_ComponentId",
                table: "ComponentEntries",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
