namespace SdsExamOlvido.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecyclableItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecyclableTypeId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ComputedRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemDescription = c.String(maxLength: 150, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RecyclableTypes", t => t.RecyclableTypeId, cascadeDelete: true)
                .Index(t => t.RecyclableTypeId);
            
            CreateTable(
                "dbo.RecyclableTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(maxLength: 10, unicode: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinKg = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxKg = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Type, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecyclableItems", "RecyclableTypeId", "dbo.RecyclableTypes");
            DropIndex("dbo.RecyclableTypes", new[] { "Type" });
            DropIndex("dbo.RecyclableItems", new[] { "RecyclableTypeId" });
            DropTable("dbo.RecyclableTypes");
            DropTable("dbo.RecyclableItems");
        }
    }
}
