using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace NetSimplified
{
    /// <summary>
	/// 用于加载 <see cref="NetModule"/> 的类
	/// </summary>
	public class NetModuleLoader
    {
		private static readonly List<NetModule> Modules = new() {
			new AggregateModule()
        };

		internal static void Register(NetModule netModule) {
			if (Modules.Contains(netModule)) {
				throw new Exception("不能重复注册! " + netModule.Name);
			}
            netModule.Type = Modules.Count;
            Modules.Add(netModule);
        }

		/// <summary>
		/// 根据 <paramref name="type"/> 获取相应的 <see cref="NetModule"/> 实例
		/// </summary>
		/// <returns><see cref="NetModule"/> 实例</returns>
		public static NetModule Get(int type) => Modules[type];

        /// <summary>
        /// 获取带有Mod与Type信息的 <see cref="NetModule"/> 实例
        /// </summary>
        /// <returns><see cref="NetModule"/> 实例</returns>
        public static T Get<T>() where T : NetModule => ModContent.GetInstance<T>();
	}
}
