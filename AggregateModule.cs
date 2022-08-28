using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;

namespace NetSimplified
{
    /// <summary>以一个 <see cref="ModPacket"/> 包的形式发送多个 <see cref="NetModule"/> 包, 能有效避免分散性地多次发包。与普通包一样，发包时调用 <see cref="NetModule.Send(int, int, bool)"/> 即可</summary>
    public class AggregateModule : NetModule
    {
        /// <summary>所有要发的包</summary>
        public List<NetModule> Modules { get; set; }

        /// <summary>
        /// 创建一个 <see cref="AggregateModule"/> 包实例
        /// </summary>
        /// <param name="modules">所有要发的 <see cref="NetModule"/> 包</param>
        public AggregateModule(List<NetModule> modules) {
            Modules = modules;
        }

        /// <inheritdoc/>
        public override void Read(BinaryReader r) {
            foreach (var module in Modules) {
                module.Read(r);
            }
        }

        /// <inheritdoc/>
        public override void Receive() {
            foreach (var module in Modules) {
                module.Receive();
            }
        }

        /// <inheritdoc/>
        public override void Send(ModPacket p) {
            foreach (var module in Modules) {
                module.Send(p);
            }
        }
    }
}