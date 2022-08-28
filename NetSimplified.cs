using System.IO;
using Terraria.ModLoader;

namespace NetSimplified
{
	/// <summary>
	/// 分别在 <see cref="Mod.HandlePacket(BinaryReader, int)"/> 与 <see cref="Mod.Unload"/> 调用其中的两个 <see langword="static"/> 方法
	/// </summary>
	public static class NetSimplified
	{
		/// <summary>接收来自你的Mod的发包, 请在 <see cref="Mod.HandlePacket(BinaryReader, int)"/> 调用</summary>
		public static void HandleModule(BinaryReader reader, int whoAmI) {
			NetModule.ReceiveModule(reader, whoAmI);
		}

		/// <summary>Mod卸载时卸载残余的字典字段, 以回收内存, 请在 <see cref="Mod.Unload"/> 或 <see cref="ModSystem.OnModUnload"/> 调用</summary>
		public static void Unload() {
			NetModule.UnloadDictionaries();
		}
	}
}