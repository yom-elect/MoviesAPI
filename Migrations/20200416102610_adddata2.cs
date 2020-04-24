using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class adddata2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "InTheaters", "Poster", "ReleaseDate", "Summary", "Title" },
                values: new object[,]
                {
                    { 5, true, null, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers: Endgame" },
                    { 6, false, null, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers: Infinity Wars" },
                    { 7, false, null, new DateTime(2020, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Sonic the Hedgehog" },
                    { 8, false, null, new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Emma" },
                    { 9, false, null, new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Greed" }
                });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "Biography", "DateOfBirth", "Name", "Picture" },
                values: new object[,]
                {
                    { 4, null, new DateTime(1962, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jim Carrey", null },
                    { 5, null, new DateTime(1965, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert Downey Jr.", null },
                    { 6, null, new DateTime(1981, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris Evans", null }
                });

            migrationBuilder.InsertData(
                table: "genre",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 9, "Adventure" },
                    { 8, "Animation" },
                    { 7, "Romance" }
                });

            migrationBuilder.InsertData(
                table: "MoviesActors",
                columns: new[] { "PersonId", "MovieId", "Character", "Order" },
                values: new object[,]
                {
                    { 4, 7, "Dr. Ivo Robotnik", 1 },
                    { 5, 5, "Tony Stark", 1 },
                    { 5, 6, "Tony Stark", 1 },
                    { 6, 5, "Steve Rogers", 2 },
                    { 6, 6, "Steve Rogers", 2 }
                });

            migrationBuilder.InsertData(
                table: "MoviesGenres",
                columns: new[] { "GenreId", "MovieId" },
                values: new object[,]
                {
                    { 9, 5 },
                    { 9, 6 },
                    { 7, 8 },
                    { 7, 9 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonId", "MovieId" },
                keyValues: new object[] { 4, 7 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonId", "MovieId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonId", "MovieId" },
                keyValues: new object[] { 5, 6 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonId", "MovieId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonId", "MovieId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 7, 8 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 7, 9 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 9, 5 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 9, 6 });

            migrationBuilder.DeleteData(
                table: "genre",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "genre",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "genre",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
