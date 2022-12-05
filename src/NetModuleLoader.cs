using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetSimplified.Syncing;
using Terraria.ModLoader;

namespace NetSimplified;

/// <summary>
/// 用于加载 <see cref="NetModule"/> 的类。
/// 需要在 <see cref="Mod.Load"/> 中对此类调用 <see cref="Mod.AddContent"/> 以添加实质内容
/// </summary>
public class NetModuleLoader : ModSystem
{
    private static List<NetModule> _modules = new();
    internal static Dictionary<int, FieldInfo[]> FieldInfos = new();

    /// <inheritdoc />
    public override void Load() {
        Mod.AddContent<AggregateModule>();
    }

    /// <inheritdoc />
    public override void Unload() {
        _modules = null;
        FieldInfos = null;
    }

    internal static void Register(NetModule netModule) {
        _modules ??= new List<NetModule>();
        FieldInfos ??= new Dictionary<int, FieldInfo[]>();

        if (_modules.Contains(netModule)) {
            throw new Exception("不能重复注册! " + netModule.Name);
        }

        netModule.Type = _modules.Count;
        _modules.Add(netModule);

        // 设立 fieldInfo 列表
        var fieldsIncluded = netModule.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        var isClassAutoSync = Attribute.IsDefined(netModule.GetType(), typeof(AutoSyncAttribute));
        
        // 只有类或变量被标记AutoSync且是支持的变量类型才能纳入
        var fields = fieldsIncluded.Where(fieldInfo =>
            (isClassAutoSync || Attribute.IsDefined(fieldInfo, typeof(AutoSyncAttribute))) &&
            AutoSyncHandler.SupportedTypes.Contains(fieldInfo.FieldType));
        var fieldInfos = fields as FieldInfo[] ?? fields.ToArray();
        if (fieldInfos.Any()) {
            FieldInfos[netModule.Type] = fieldInfos.ToArray();
        }
    }

    /// <summary>
    /// 根据 <paramref name="type"/> 获取相应的 <see cref="NetModule"/> 实例
    /// </summary>
    /// <returns><see cref="NetModule"/> 实例</returns>
    public static NetModule Get(int type) => _modules[type];

    /// <summary>
    /// 获取带有Mod与Type信息的 <see cref="NetModule"/> 实例
    /// </summary>
    /// <returns><see cref="NetModule"/> 实例</returns>
    public static T Get<T>() where T : NetModule => ModContent.GetInstance<T>();
}