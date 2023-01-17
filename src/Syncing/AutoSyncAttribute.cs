using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace NetSimplified.Syncing;

/// <summary>
/// 此特性允许变量自动发包传输<br/>
/// 支持的变量类型: <see cref="byte"/>, <see cref="bool"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="sbyte"/>, <see cref="ushort"/>, <see cref="uint"/>, <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, <see cref="Vector2"/>, <see cref="Color"/>, <see cref="Point"/>, <see cref="Point16"/>, <see cref="Item"/> 及其数组类型
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
public class AutoSyncAttribute : Attribute
{
}