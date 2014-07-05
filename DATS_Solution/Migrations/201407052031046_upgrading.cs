namespace DATS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upgrading : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SoldPlaces", "ClientId", "dbo.Clients");
            DropIndex("dbo.SoldPlaces", new[] { "ClientId" });
            AddColumn("dbo.Clients", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Places", "Column", c => c.Int(nullable: false));
            AddColumn("dbo.Places", "RowPos", c => c.Int(nullable: false));
            AddColumn("dbo.Places", "ColumnPos", c => c.Int(nullable: false));
            AddColumn("dbo.SoldPlaces", "SectorId", c => c.Int(nullable: false));
            AlterColumn("dbo.Matches", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Stadiums", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Sectors", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.SoldPlaces", "ClientId", c => c.Int());
            CreateIndex("dbo.SoldPlaces", "ClientId");
            AddForeignKey("dbo.SoldPlaces", "ClientId", "dbo.Clients", "Id");
            DropColumn("dbo.Sectors", "Color");
            DropColumn("dbo.Places", "Position");
            DropColumn("dbo.Places", "Location");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "Location", c => c.Int(nullable: false));
            AddColumn("dbo.Places", "Position", c => c.Int(nullable: false));
            AddColumn("dbo.Sectors", "Color", c => c.Int(nullable: false));
            DropForeignKey("dbo.SoldPlaces", "ClientId", "dbo.Clients");
            DropIndex("dbo.SoldPlaces", new[] { "ClientId" });
            AlterColumn("dbo.SoldPlaces", "ClientId", c => c.Int(nullable: false));
            AlterColumn("dbo.Sectors", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Stadiums", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Matches", "Name", c => c.String(nullable: false));
            DropColumn("dbo.SoldPlaces", "SectorId");
            DropColumn("dbo.Places", "ColumnPos");
            DropColumn("dbo.Places", "RowPos");
            DropColumn("dbo.Places", "Column");
            DropColumn("dbo.Clients", "IsActive");
            CreateIndex("dbo.SoldPlaces", "ClientId");
            AddForeignKey("dbo.SoldPlaces", "ClientId", "dbo.Clients", "Id", cascadeDelete: true);
        }
    }
}
