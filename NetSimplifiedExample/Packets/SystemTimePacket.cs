using System;
using System.Linq;
using NetSimplified;
using NetSimplified.Syncing;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;

namespace NetSimplifiedExample.Packets;

// 使用 AutoSync 特性以使变量自动传输，免去自己写 Send 和 Receive 的功夫
[AutoSync]
public class SystemTimePacket : NetModule
{
    private string _time;
    
    public static SystemTimePacket Get(string time) {
        var module = NetModuleLoader.Get<SystemTimePacket>();
        module._time = time;
        return module;
    }

    public override void Receive() {
        if (Main.netMode is not NetmodeID.MultiplayerClient) return;

        Main.NewText($"服务器系统时间: {_time}");
    }
}