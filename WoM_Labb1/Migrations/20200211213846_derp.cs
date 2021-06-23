using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WoM_Labb1.Migrations
{
    public partial class derp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    personnummer = table.Column<string>(nullable: false),
                    fornamn = table.Column<string>(nullable: false),
                    efternamn = table.Column<string>(nullable: false),
                    postadress = table.Column<string>(nullable: false),
                    postnr = table.Column<int>(nullable: false),
                    ort = table.Column<string>(nullable: false),
                    epost = table.Column<string>(nullable: false),
                    telefonnummer = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.personnummer);
                });

            migrationBuilder.CreateTable(
                name: "Produkt",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productName = table.Column<string>(maxLength: 60, nullable: false),
                    productDescription = table.Column<string>(maxLength: 350, nullable: false),
                    productPrice = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    productInStore = table.Column<int>(nullable: false),
                    productImage = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produkt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    orderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personnummer = table.Column<string>(nullable: true),
                    Customerpersonnummer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_Order_Customer_Customerpersonnummer",
                        column: x => x.Customerpersonnummer,
                        principalTable: "Customer",
                        principalColumn: "personnummer",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    VarukorgId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<int>(nullable: false),
                    ProduktId = table.Column<int>(nullable: true),
                    Antal = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.VarukorgId);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Produkt_ProduktId",
                        column: x => x.ProduktId,
                        principalTable: "Produkt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    ProduktId = table.Column<int>(nullable: true),
                    Antal = table.Column<int>(nullable: false),
                    Pris = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailsId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Produkt_ProduktId",
                        column: x => x.ProduktId,
                        principalTable: "Produkt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Order_orderId",
                        column: x => x.orderId,
                        principalTable: "Order",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Customerpersonnummer",
                table: "Order",
                column: "Customerpersonnummer");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProduktId",
                table: "OrderDetails",
                column: "ProduktId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_orderId",
                table: "OrderDetails",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_ProduktId",
                table: "ShoppingCart",
                column: "ProduktId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Produkt");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
