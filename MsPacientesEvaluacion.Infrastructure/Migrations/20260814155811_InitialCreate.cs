using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsPacientesEvaluacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Contacto_Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contacto_Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Contacto_Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NutricionistaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConsultaInicial_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConsultaInicial_Fecha = table.Column<DateOnly>(type: "date", nullable: true),
                    ConsultaInicial_Peso = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    ConsultaInicial_Altura = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Anamnesis_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Anamnesis_HabitosAlimenticios = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Anamnesis_AntecedentesClinicos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Anamnesis_NecesidadesEspecificas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesSeguimiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Medidas_Cintura = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Medidas_Cadera = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Medidas_Imc = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Adherencia = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesSeguimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluacionesSeguimiento_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesSeguimiento_PacienteId",
                table: "EvaluacionesSeguimiento",
                column: "PacienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluacionesSeguimiento");

            migrationBuilder.DropTable(
                name: "Pacientes");
        }
    }
}
