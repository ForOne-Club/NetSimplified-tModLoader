using System;
using System.Collections.Generic;
using System.IO;
using NetSimplified.Syncing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NetSimplified
{
    /// <summary>
    /// 以一个 <see cref="ModPacket"/> 包的形式发送多个 <see cref="NetModule"/> 包, 能有效避免分散性地多次发包。<br/>
    /// 与普通包一样, 发包时调用 <see cref="Send(Mod, int, int, bool)"/> 即可<br/>
    /// 正常情况下, 其 <see cref="NetModule.Type"/> 应为0, 获取时应调用 <see cref="Get(List{NetModule})"/> 而不是
    /// <see cref="NetModuleLoader.Get{T}"/>, 否则会获取到 <see langword="null"/> 值
    /// </summary>
    public sealed class AggregateModule : NetModule
    {
        /// <inheritdoc/>
        public override void SetStaticDefaults() {
            Type = 0;
        }

        /// <summary>所有要发的包</summary>
        public List<NetModule> Modules { get; set; }

        /// <summary>
        /// 创建一个 <see cref="AggregateModule"/> 包实例
        /// </summary>
        /// <param name="modules">所有要发的 <see cref="NetModule"/> 包</param>
        public static AggregateModule Get(List<NetModule> modules) {
            var module = NetModuleLoader.Get(0) as AggregateModule;
            module.Modules = modules;
            return module;
        }

        /// <inheritdoc/>
        public override void Read(BinaryReader r) {
            int count = r.ReadInt32();
            Modules = new List<NetModule>();
            for (int i = 0; i < count; i++) {
                int type = r.ReadInt32();
                var module = NetModuleLoader.Get(type);
                AutoSyncHandler.HandleAutoRead(module, r);
                module.Read(r);
                Modules.Add(module);
            }
        }

        /// <inheritdoc/>
        public override void Send(ModPacket p) {
            p.Write(Modules.Count);
            foreach (var module in Modules) {
                p.Write(module.Type);
                AutoSyncHandler.HandleAutoSend(module, p);
                module.Send(p);
            }
        }

        /// <inheritdoc/>
        public override void Receive() {
            foreach (var module in Modules) {
                module.Receive();
            }
        }
        
        /// <summary>
        /// 请不要对 <see cref="AggregateModule"/> 使用这个, 去用 <see cref="Send(Mod, int, int, bool)"/>
        /// </summary>
        [Obsolete("使用 Send(Mod mod, int toClient = -1, int ignoreClient = -1, bool runLocally = false)", true)]
        public new void Send(int toClient = -1, int ignoreClient = -1, bool runLocally = false) {
        }

        /// <summary>
        /// 通过 <see cref="ModPacket"/> 发包
        /// </summary>
        /// <param name="mod">该 <see cref="AggregateModule"/> 包是由哪个 <see cref="Mod"/> 发出的，传入自己模组的即可。由于 <see cref="AggregateModule"/> 是 <see cref="NetSimplified"/> 预定义的包类型，所以不具备 <see cref="Mod"/> 信息，需要自行指定</param>
        /// <param name="toClient">如果不是 -1, 则包<b>只会</b>发送给对应的客户端</param>
        /// <param name="ignoreClient">如果不是 -1, 则包<b>不会</b>发送给对应的客户端</param>
        /// <param name="runLocally">如果为 <see langword="true"/> 则在发包时会调用 <see cref="Receive()"/> 方法</param>
        public void Send(Mod mod, int toClient = -1, int ignoreClient = -1, bool runLocally = false) {
            if (PreSend(toClient, ignoreClient)) {
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    var mp = mod.GetPacket();
                    mp.Write(Type);
                    Send(mp);
                    mp.Send(toClient, ignoreClient);
                }

                if (runLocally) {
                    Receive();
                }
            }
        }
    }
}