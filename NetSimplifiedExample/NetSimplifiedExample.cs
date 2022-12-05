using System.IO;
using NetSimplified;
using Terraria.ModLoader;

namespace NetSimplifiedExample;

public class NetSimplifiedExample : Mod
{
    // 这一行必加
    public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);
}