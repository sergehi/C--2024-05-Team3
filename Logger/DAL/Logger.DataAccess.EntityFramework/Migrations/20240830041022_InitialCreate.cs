using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logger.DataAccess.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityPK = table.Column<Guid>(type: "uuid", nullable: false),
                    Entity = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Time",
                table: "Logs",
                column: "Time");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION is_uuid(value text)
                RETURNS boolean
                LANGUAGE plpgsql
                AS $function$
                DECLARE
                    v_uuid UUID;
	            BEGIN
		            IF value = '00000000-0000-0000-0000-000000000000' THEN
                    RETURN FALSE;
                    END IF;
		            v_uuid := value::UUID;
                    RETURN TRUE;
                EXCEPTION WHEN OTHERS THEN
                    RETURN FALSE;
	            END;
                $function$
                ;");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.loglist(time_b bigint, time_e bigint, e_action integer, user_id text, entity_type text, entity_pk text)
                RETURNS SETOF ""Logs""
                LANGUAGE plpgsql
                AS $function$
	            BEGIN
	            IF NOT is_uuid(user_id) THEN 
		            user_id := '00000000-0000-0000-0000-000000000000'; 
		            END IF;
	            IF NOT is_uuid(entity_type) THEN 
		            entity_type := '00000000-0000-0000-0000-000000000000'; 
		            END IF;
	            IF NOT is_uuid(entity_pk) THEN 
		            entity_pk := '00000000-0000-0000-0000-000000000000'; 
		            END IF;
	            RETURN QUERY  
	            SELECT * FROM ""Logs"" WHERE 
		            ""Time"" >= time_b AND 
		            ""Time"" <= time_e AND 
		            CASE WHEN e_action = 0 THEN 1 ELSE CASE WHEN ""Action"" = e_action THEN 1 ELSE 0 END END = 1 AND 
		            CASE WHEN NOT is_uuid(user_id) THEN 1 ELSE CASE WHEN ""UserId"" = user_id::UUID THEN 1 ELSE 0 END END = 1 AND 
		            CASE WHEN NOT is_uuid(entity_type) THEN 1 ELSE CASE WHEN ""EntityType"" = entity_type::UUID THEN 1 ELSE 0 END END = 1 AND 
		            CASE WHEN NOT is_uuid(entity_pk) THEN 1 ELSE CASE WHEN ""EntityPK"" = entity_pk::UUID THEN 1 ELSE 0 END END = 1;
	            END;
                $function$
                ;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION loglist(time_b bigint, time_e bigint, e_action integer, user_id text, entity_type text, entity_pk text);");
            migrationBuilder.Sql(@"DROP FUNCTION is_uuid(value text);");

            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
