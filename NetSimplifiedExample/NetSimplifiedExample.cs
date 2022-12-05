using System;
using System.IO;
using NetSimplified;
using Terraria.ModLoader;

namespace NetSimplifiedExample;

public class NetSimplifiedExample : Mod
{
    // 在这里调用 AddContent 以加载 AggregateModule, 自动传输等功能，必不可少
    public override void Load() {
        AddContent<NetModuleLoader>();
    }

    // 调用 NetModule.ReceiveModule 以进行收包处理，必不可少
    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        NetModule.ReceiveModule(reader, whoAmI);
    }
}