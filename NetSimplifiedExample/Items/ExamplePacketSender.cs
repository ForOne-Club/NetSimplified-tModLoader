using NetSimplified;
using NetSimplifiedExample.Packets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NetSimplifiedExample.Items;

public class ExamplePacketSender : ModItem
{
    public override void SetStaticDefaults() {
        // DisplayName.SetDefault("ExamplePacketSender"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        Tooltip.SetDefault("This is a basic modded sword.");
    }

    public override void SetDefaults() {
        Item.damage = 50;
        Item.DamageType = DamageClass.Melee;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 6;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
    }

    public override bool CanUseItem(Player player) {
        if (Main.netMode is NetmodeID.Server) {
            InventoryPacket.Get(player.whoAmI).Send(toClient: player.whoAmI);
        }

        return base.CanUseItem(player);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.DirtBlock, 10)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}