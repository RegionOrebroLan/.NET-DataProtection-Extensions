using Microsoft.EntityFrameworkCore.Migrations;

namespace RegionOrebroLan.DataProtection.Data.Migrations.Sqlite
{
	public partial class SqliteDataProtectionContextMigration : Migration
	{
		#region Methods

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "DataProtectionKeys");
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "DataProtectionKeys",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					FriendlyName = table.Column<string>(type: "TEXT", nullable: true),
					Xml = table.Column<string>(type: "TEXT", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
				});
		}

		#endregion
	}
}