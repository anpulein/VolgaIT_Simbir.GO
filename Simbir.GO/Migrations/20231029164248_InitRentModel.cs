using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Simbir.GO.Migrations
{
    /// <inheritdoc />
    public partial class InitRentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransportId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    TimeStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PriceOfUnit = table.Column<double>(type: "double precision", nullable: false),
                    PriceType = table.Column<int>(type: "integer", nullable: false),
                    FinalPrice = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rents_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rents_Transports_TransportId",
                        column: x => x.TransportId,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "$2a$11$k.4NAqxybbLTZ9iYqtVLQe7VMBWAR0DVZp.6cLxHFSCHb57dvZv/q");

            migrationBuilder.CreateIndex(
                name: "IX_Rents_AccountId",
                table: "Rents",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Rents_TransportId",
                table: "Rents",
                column: "TransportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rents");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "$2a$11$YHIxk10qncL0c4ZdhYiJAeSGnOHGqZALy5UlD9h4erdMmqWfNPBEG");
        }
    }
}
