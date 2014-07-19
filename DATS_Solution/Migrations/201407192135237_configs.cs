namespace DATS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class configs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StadiumId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 50),
                        Val = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stadiums", t => t.StadiumId)
                .Index(t => t.StadiumId);
            
            AlterColumn("dbo.Clients", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Clients", "Contact", c => c.String(maxLength: 255));
            AlterColumn("dbo.Stadiums", "Address", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Configs", "StadiumId", "dbo.Stadiums");
            DropIndex("dbo.Configs", new[] { "StadiumId" });
            AlterColumn("dbo.Stadiums", "Address", c => c.String());
            AlterColumn("dbo.Clients", "Contact", c => c.String(nullable: false));
            AlterColumn("dbo.Clients", "Name", c => c.String(nullable: false));
            DropTable("dbo.Configs");
        }
    }
}
