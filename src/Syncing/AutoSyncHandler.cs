using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace NetSimplified.Syncing;

internal static class AutoSyncHandler
{
    internal static void HandleAutoSend(NetModule netModule, BinaryWriter bw) {
        var fields = netModule.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        var isClassAutoSync = Attribute.IsDefined(netModule.GetType(), typeof(AutoSyncAttribute));

        foreach (var fieldInfo in fields) {
            if (!isClassAutoSync && !Attribute.IsDefined(fieldInfo, typeof(AutoSyncAttribute))) continue;

            if (fieldInfo.FieldType == typeof(byte)) {
                bw.Write((byte) fieldInfo.GetValue(netModule)!);
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

            if (fieldInfo.FieldType == typeof(string)) {
                bw.Write((string) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Vector2)) {
                bw.WriteVector2((Vector2) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Color)) {
                bw.WriteRGB((Color) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Point)) {
                bw.Write((Point) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Point16)) {
                bw.Write((Point16) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Item)) {
                bw.Write((Item) fieldInfo.GetValue(netModule)!);
            }

            if (fieldInfo.FieldType == typeof(Item[])) {
                bw.Write((Item[]) fieldInfo.GetValue(netModule)!);
            }
        }
    }

    internal static void HandleAutoRead(NetModule netModule, BinaryReader mp) {
        var fields = netModule.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        var isClassAutoSync = Attribute.IsDefined(netModule.GetType(), typeof(AutoSyncAttribute));

        foreach (var fieldInfo in fields) {
            if (!isClassAutoSync && !Attribute.IsDefined(fieldInfo, typeof(AutoSyncAttribute))) continue;

            if (fieldInfo.FieldType == typeof(byte)) {
                fieldInfo.SetValue(netModule, mp.ReadByte());
            }

            if (fieldInfo.FieldType == typeof(bool)) {
                fieldInfo.SetValue(netModule, mp.ReadBoolean());
            }

            if (fieldInfo.FieldType == typeof(short)) {
                fieldInfo.SetValue(netModule, mp.ReadInt16());
            }

            if (fieldInfo.FieldType == typeof(int)) {
                fieldInfo.SetValue(netModule, mp.ReadInt32());
            }

            if (fieldInfo.FieldType == typeof(long)) {
                fieldInfo.SetValue(netModule, mp.ReadInt64());
            }

            if (fieldInfo.FieldType == typeof(string)) {
                fieldInfo.SetValue(netModule, mp.ReadString());
            }

            if (fieldInfo.FieldType == typeof(Vector2)) {
                fieldInfo.SetValue(netModule, mp.ReadVector2());
            }

            if (fieldInfo.FieldType == typeof(Color)) {
                fieldInfo.SetValue(netModule, mp.ReadRGB());
            }

            if (fieldInfo.FieldType == typeof(Point)) {
                fieldInfo.SetValue(netModule, mp.ReadPoint());
            }

            if (fieldInfo.FieldType == typeof(Point16)) {
                fieldInfo.SetValue(netModule, mp.ReadPoint16());
            }

            if (fieldInfo.FieldType == typeof(Item)) {
                fieldInfo.SetValue(netModule, mp.ReadItem());
            }

            if (fieldInfo.FieldType == typeof(Item[])) {
                fieldInfo.SetValue(netModule, mp.ReadItemArray());
            }
        }
    }
}