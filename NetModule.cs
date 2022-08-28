using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NetSimplified
{
    /// <summary>
    /// 用于写入、读取 <see cref="ModPacket"/> 的基类 <para/>
    // </summary>
    public abstract class NetModule : ILoadable
    {
        /// <inheritdoc/>
        public void Load(Mod mod) {
            typeModMap.Add(GetType(), mod.Name);
            idModuleMap.Add(idCounter, this);
            ID = idCounter++;
        }

        /// <inheritdoc/>
        public void Unload() { }

        private static Dictionary<Type, string> typeModMap;
        private static Dictionary<int, NetModule> idModuleMap;
        private static int idCounter = 0;

        /// <summary>这个 <see cref="NetModule"/> 所属的模组</summary>
        protected Mod Mod => ModLoader.GetMod(typeModMap[GetType()]);

        /// <summary>包的发送者</summary>
        protected int Sender { get; private set; } = Main.myPlayer;

        /// <summary>该 <see cref="NetModule"/> 被分配到的ID</summary>
        public int ID { get; private set; }

        /// <summary>
        /// 使用这个函数来自行发送字段
        /// </summary>
        /// <param name="p">用于发包的 <see cref="ModPacket"/> 实例</param>
        public abstract void Send(ModPacket p);

        /// <summary>
        /// 通过 <see cref="ModPacket"/> 发包
        /// </summary>
        /// <param name="toClient">如果不是 -1, 则包<b>只会</b>发送给对应的客户端</param>
        /// <param name="ignoreClient">如果不是 -1, 则包<b>不会</b>发送给对应的客户端</param>
        /// <param name="runLocally">如果为 <see langword="true"/> 则在发包时会调用 <see cref="Receive()"/> 方法</param>
        public void Send(int toClient = -1, int ignoreClient = -1, bool runLocally = true) {
            if (PreSend(toClient, ignoreClient)) {
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    ModPacket mp = Mod.GetPacket();
                    //serializer.Serialize(mp.BaseStream, this);
                    mp.Write(ID);
                    Send(mp);
                    mp.Send(toClient, ignoreClient);
                }
                if (runLocally) {
                    Receive();
                }
            }
        }

        /// <summary>
        /// 使用这个函数来自行读取字段
        /// </summary>
        /// <param name="r">用于读取的 <see cref="BinaryReader"/> 实例</param>
        public abstract void Read(BinaryReader r);

        /// <summary>
        /// 使用这个函数来进行接收后的操作 (与 <see cref="Read(BinaryReader)"/> 分开以适配 runLocally)
        /// </summary>
        public abstract void Receive();

        /// <summary>发包前调用, 返回 <see langword="false"/> 则不会发包, 也不会调用 <see cref="Receive()"/>。 默认为 <see langword="true"/>.</summary>
        protected virtual bool PreSend(int toClient = -1, int ignoreClient = -1) => true;

        internal static void ReceiveModule(BinaryReader reader, int whoAmI) {
            NetModule module = idModuleMap[reader.ReadInt32()];
            module.Sender = whoAmI;
            module.Read(reader);
            module.Receive();
        }

        internal static void LoadDictionaries() {
            if (typeModMap == null) {
                typeModMap = new();
            }
            if (idModuleMap == null) {
                idModuleMap = new();
            }
        }

        internal static void UnloadDictionaries() {
            typeModMap = null;
            idModuleMap = null;
        }
    }
}
