using System;
using System.Collections.Generic;
using System.Globalization;
using NetSimplified;
using NetSimplifiedExample.Packets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NetSimplifiedExample.Items;

/// <summary>
/// 这个例子演示了如何使用 AggregateModule 合并发送两个包
/// </summary>
public class ExampleAggregateSender : ModItem
{
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
            // 获取包含了多个 NetModule 包的 AggregateModule 包实例
            AggregateModule.Get(new List<NetModule> {
                // 第一个 NetModule 包
                SystemTimePacket.Get(DateTime.Now.ToString(CultureInfo.InvariantCulture)), // 注意逗号
                // 第二个 NetModule 包
                RandomStringPacket.Get()
            }).Send(toClient: player.whoAmI); // 发送
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