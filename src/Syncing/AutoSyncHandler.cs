using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace NetSimplified.Syncing;

internal static class AutoSyncHandler
{
    /// <summary>
    /// 自动传输支持的数据类型
    /// </summary>
    public static readonly Type[] SupportedTypes = {
        typeof(byte), typeof(bool), typeof(short), typeof(int), typeof(long), typeof(sbyte), typeof(ushort),
        typeof(uint), typeof(ulong), typeof(float), typeof(double), typeof(char), typeof(string), typeof(Vector2),
        typeof(Color), typeof(Point), typeof(Point16), typeof(Item), typeof(Item[]), typeof(byte[])
    };

    internal static void HandleAutoSend(NetModule netModule, BinaryWriter bw) {
        if (!NetModuleLoader.FieldInfos.TryGetValue(netModule.Type, out var fields)) {
            return;
        }

        foreach (var fieldInfo in fields) {
            if (fieldInfo.FieldType == typeof(byte)) {
                bw.Write((byte) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(byte[])) {
                var buffer = (byte[]) fieldInfo.GetValue(netModule)!;
                bw.Write(buffer.Length);
                bw.Write(buffer);
            }

            if (fieldInfo.FieldType == typeof(bool)) {
                bw.Write((bool) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(short)) {
                bw.Write((short) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(int)) {
                bw.Write((int) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(long)) {
                bw.Write((long) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(sbyte)) {
                bw.Write((sbyte) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(ushort)) {
                bw.Write((ushort) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(uint)) {
                bw.Write((uint) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(ulong)) {
                bw.Write((ulong) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(float)) {
                bw.Write((float) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(double)) {
                bw.Write((double) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(char)) {
                bw.Write((char) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(string)) {
                bw.Write((string) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Vector2)) {
                bw.WriteVector2((Vector2) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Point)) {
                bw.Write((Point) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Point16)) {
                bw.Write((Point16) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Color)) {
                var syncAlpha = true;
                if (TryGetCustomAttribute<ColorSyncAttribute>(fieldInfo, out var colorSyncAttr)) {
                    syncAlpha = colorSyncAttr.SyncAlpha;
                }

                if (syncAlpha) {
                    bw.WriteRGBA((Color) fieldInfo.GetValue(netModule)!);
                }
                else {
                    bw.WriteRGB((Color) fieldInfo.GetValue(netModule)!);
                }
            }

            bool syncStack = true;
            bool syncFavorite = false;

            if (Attribute.IsDefined(fieldInfo, typeof(ItemSyncAttribute))) {
                var attr = fieldInfo.GetCustomAttribute(typeof(ItemSyncAttribute));
                if (attr is ItemSyncAttribute itemSyncAttr) {
                    syncStack = itemSyncAttr.SyncStack;
                    syncFavorite = itemSyncAttr.SyncFavorite;
                }
            }

            if (fieldInfo.FieldType == typeof(Item)) {
                bw.Write((Item) fieldInfo.GetValue(netModule)!, syncStack, syncFavorite);
            }

            if (fieldInfo.FieldType == typeof(Item[])) {
                bw.Write((Item[]) fieldInfo.GetValue(netModule)!, syncStack, syncFavorite);
            }
        }
    }

    internal static void HandleAutoRead(NetModule netModule, BinaryReader r) {
        if (!NetModuleLoader.FieldInfos.TryGetValue(netModule.Type, out var fields)) {
            return;
        }

        foreach (var fieldInfo in fields) {
            if (fieldInfo.FieldType == typeof(byte)) {
                fieldInfo.SetValue(netModule, r.ReadByte());
            }

            if (fieldInfo.FieldType == typeof(byte[])) {
                int length = r.ReadInt32();
                fieldInfo.SetValue(netModule, r.ReadBytes(length));
            }

            if (fieldInfo.FieldType == typeof(bool)) {
                fieldInfo.SetValue(netModule, r.ReadBoolean());
            }

            if (fieldInfo.FieldType == typeof(short)) {
                fieldInfo.SetValue(netModule, r.ReadInt16());
            }

            if (fieldInfo.FieldType == typeof(int)) {
                fieldInfo.SetValue(netModule, r.ReadInt32());
            }

            if (fieldInfo.FieldType == typeof(long)) {
                fieldInfo.SetValue(netModule, r.ReadInt64());
            }

            if (fieldInfo.FieldType == typeof(sbyte)) {
                fieldInfo.SetValue(netModule, r.ReadSByte());
            }

            if (fieldInfo.FieldType == typeof(ushort)) {
                fieldInfo.SetValue(netModule, r.ReadUInt16());
            }

            if (fieldInfo.FieldType == typeof(uint)) {
                fieldInfo.SetValue(netModule, r.ReadUInt32());
            }

            if (fieldInfo.FieldType == typeof(ulong)) {
                fieldInfo.SetValue(netModule, r.ReadUInt64());
            }

            if (fieldInfo.FieldType == typeof(float)) {
                fieldInfo.SetValue(netModule, r.ReadSingle());
            }

            if (fieldInfo.FieldType == typeof(double)) {
                fieldInfo.SetValue(netModule, r.ReadDouble());
            }

            if (fieldInfo.FieldType == typeof(char)) {
                fieldInfo.SetValue(netModule, r.ReadChar());
            }

            if (fieldInfo.FieldType == typeof(string)) {
                fieldInfo.SetValue(netModule, r.ReadString());
            }

            if (fieldInfo.FieldType == typeof(Vector2)) {
                fieldInfo.SetValue(netModule, r.ReadVector2());
            }

            if (fieldInfo.FieldType == typeof(Point)) {
                fieldInfo.SetValue(netModule, r.ReadPoint());
            }

            if (fieldInfo.FieldType == typeof(Point16)) {
                fieldInfo.SetValue(netModule, r.ReadPoint16());
            }

            if (fieldInfo.FieldType == typeof(Color)) {
                var syncAlpha = true;
                if (TryGetCustomAttribute<ColorSyncAttribute>(fieldInfo, out var colorSyncAttr)) {
                    syncAlpha = colorSyncAttr.SyncAlpha;
                }

                fieldInfo.SetValue(netModule, syncAlpha ? r.ReadRGBA() : r.ReadRGB());
            }

            bool syncStack = true;
            bool syncFavorite = false;

            if (TryGetCustomAttribute<ItemSyncAttribute>(fieldInfo, out var itemSyncAttr)) {
                syncStack = itemSyncAttr.SyncStack;
                syncFavorite = itemSyncAttr.SyncFavorite;
            }

            if (fieldInfo.FieldType == typeof(Item)) {
                fieldInfo.SetValue(netModule, r.ReadItem(syncStack, syncFavorite));
            }

            if (fieldInfo.FieldType == typeof(Item[])) {
                fieldInfo.SetValue(netModule, r.ReadItemArray(syncStack, syncFavorite));
            }
        }
    }

    private static bool TryGetCustomAttribute<T>(MemberInfo fieldInfo, out T attribute) where T : Attribute {
        if (Attribute.IsDefined(fieldInfo, typeof(T))) {
            var customAttribute = fieldInfo.GetCustomAttribute(typeof(T));
            if (customAttribute is T attr) {
                attribute = attr;
                return true;
            }
        }

        attribute = null;
        return false;
    }
}