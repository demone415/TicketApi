using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TicketApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticketheaders",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fssign = table.Column<string>(type: "text", nullable: true),
                    fsdoc = table.Column<string>(type: "text", nullable: true),
                    fsid = table.Column<string>(type: "text", nullable: true),
                    tsmp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    fetchtries = table.Column<short>(type: "smallint", nullable: false),
                    nextfetch = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ticketid = table.Column<string>(type: "text", nullable: true),
                    ticketsum = table.Column<decimal>(type: "numeric", nullable: false),
                    shopinn = table.Column<string>(type: "text", nullable: true),
                    shopname = table.Column<string>(type: "text", nullable: true),
                    shopaddress = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    @operator = table.Column<string>(name: "operator", type: "text", nullable: true),
                    username = table.Column<string>(type: "text", nullable: true),
                    manual = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticketheaders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ticketlines",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    cost = table.Column<decimal>(type: "numeric", nullable: false),
                    category1 = table.Column<string>(type: "text", nullable: true),
                    category2 = table.Column<string>(type: "text", nullable: true),
                    category3 = table.Column<string>(type: "text", nullable: true),
                    tags = table.Column<string>(type: "text", nullable: true),
                    essential = table.Column<bool>(type: "boolean", nullable: false),
                    paymenttype = table.Column<decimal>(type: "numeric", nullable: true),
                    producttype = table.Column<decimal>(type: "numeric", nullable: true),
                    rawproductcode = table.Column<string>(type: "text", nullable: true),
                    productidtype = table.Column<string>(type: "text", nullable: true),
                    TicketHeaderId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticketlines", x => x.id);
                    table.ForeignKey(
                        name: "FK_ticketlines_ticketheaders_TicketHeaderId",
                        column: x => x.TicketHeaderId,
                        principalTable: "ticketheaders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ticketlines_TicketHeaderId",
                table: "ticketlines",
                column: "TicketHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticketlines");

            migrationBuilder.DropTable(
                name: "ticketheaders");
        }
    }
}
