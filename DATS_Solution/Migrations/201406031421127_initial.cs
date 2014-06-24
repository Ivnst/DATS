namespace DATS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Contact = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StadiumId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        BeginsAt = c.DateTime(nullable: false),
                        Duration = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stadiums", t => t.StadiumId, cascadeDelete: true)
                .Index(t => t.StadiumId);
            
            CreateTable(
                "dbo.Stadiums",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Address = c.String(),
                        SchemePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sectors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StadiumId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Color = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stadiums", t => t.StadiumId, cascadeDelete: true)
                .Index(t => t.StadiumId);
            
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectorId = c.Int(nullable: false),
                        Row = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        Location = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sectors", t => t.SectorId, cascadeDelete: true)
                .Index(t => t.SectorId);
            
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatchId = c.Int(nullable: false),
                        SectorId = c.Int(nullable: false),
                        PriceValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
                .ForeignKey("dbo.Sectors", t => t.SectorId)
                .Index(t => t.MatchId)
                .Index(t => t.SectorId);
            
            CreateTable(
                "dbo.SoldPlaces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatchId = c.Int(nullable: false),
                        PlaceId = c.Int(nullable: false),
                        Summ = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        IsReservation = c.Boolean(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
                .ForeignKey("dbo.Places", t => t.PlaceId)
                .Index(t => t.MatchId)
                .Index(t => t.PlaceId)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SoldPlaces", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.SoldPlaces", "MatchId", "dbo.Matches");
            DropForeignKey("dbo.SoldPlaces", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Prices", "SectorId", "dbo.Sectors");
            DropForeignKey("dbo.Prices", "MatchId", "dbo.Matches");
            DropForeignKey("dbo.Sectors", "StadiumId", "dbo.Stadiums");
            DropForeignKey("dbo.Places", "SectorId", "dbo.Sectors");
            DropForeignKey("dbo.Matches", "StadiumId", "dbo.Stadiums");
            DropIndex("dbo.SoldPlaces", new[] { "ClientId" });
            DropIndex("dbo.SoldPlaces", new[] { "PlaceId" });
            DropIndex("dbo.SoldPlaces", new[] { "MatchId" });
            DropIndex("dbo.Prices", new[] { "SectorId" });
            DropIndex("dbo.Prices", new[] { "MatchId" });
            DropIndex("dbo.Places", new[] { "SectorId" });
            DropIndex("dbo.Sectors", new[] { "StadiumId" });
            DropIndex("dbo.Matches", new[] { "StadiumId" });
            DropTable("dbo.SoldPlaces");
            DropTable("dbo.Prices");
            DropTable("dbo.Places");
            DropTable("dbo.Sectors");
            DropTable("dbo.Stadiums");
            DropTable("dbo.Matches");
            DropTable("dbo.Clients");
        }
    }
}
