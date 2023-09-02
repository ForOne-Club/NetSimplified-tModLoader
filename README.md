# Net Simplified
这是一个用于tModLoader联机代码的类库  
不需要Mod强引用(因为根本没有Mod)

[最新版本在这里](https://github.com/Crapsky233/NetSimplified-tModLoader/releases/latest)

## 添加到你的Mod中
在Mod根目录创建一个 `lib/` 文件夹，将 `dll` 与 `xml` 文件放入（在[Releases](https://github.com/Crapsky233/NetSimplified-tModLoader/releases/latest)界面下载最新版的这两个文件）  
在 `build.txt` 添加: `dllReferences = NetSimplified`  
最后在VS添加引用即可

要是你还不懂，建议查看[配套示例](NetSimplifiedExample)

*p.s. `dll` 文件为类库，即本体，`xml` 文件为注释*

### 建立框架
对于你的模组，你只需要编写两处激活性质的代码即可正常使用此库的全部内容:

在 `Mod` [主类](NetSimplifiedExample/NetSimplifiedExample.cs)中:
- 在 `Load` 调用 `AddContent<NetModuleLoader>()`，以激活 `AggregateModule` 和自动传输功能
- 在 `HandlePacket` 调用 `NetModule.ReceiveModule`，以让库接收并处理二进制数据包

你可以参考[示例模组](NetSimplifiedExample/NetSimplifiedExample.cs)中的写法

## 配套示例模组

关于这个库的基本用法的演示，可以看[此示例模组](NetSimplifiedExample)，以下是简要内容:

- `build.txt` 中添加了: `dllReferences = NetSimplified`
- `Mod` [主类](NetSimplifiedExample/NetSimplifiedExample.cs)中:
  - 在 `Load` 调用 `AddContent<NetModuleLoader>()`
  - 在 `HandlePacket` 调用 `NetModule.ReceiveModule`
- 三个 `NetModule` 类的[例子](NetSimplifiedExample/Packets)
- 一个发单独包的[例子](NetSimplifiedExample/Items/ExamplePacketSender.cs)
- 一个发 AggregateModule 合并包的[例子](NetSimplifiedExample/Items/ExampleAggregateSender.cs)

## AutoSync 自动传输特性

此库提供了一个自动通过 `Write` 与 `Read` 系列方法传输数据的功能，可以通过对类或字段标记 `[AutoSync]` 特性以使其自动同步，[这个例子](NetSimplifiedExample/Packets/InventoryPacket.cs)可以展示了它的使用方法。

目前，自动传输支持的变量为: ` byte, byte[], bool, short, int, long, sbyte, ushort, uint, ulong, float, double, char, string, Vector2, Color, Point, Point16, Item, Item[]`

**注意: 对于不支持的变量，无法使用自动传输，你仍需要自行编写传输代码**

### 对类使用特性

对类使用 `AutoSync` 特性可以让此类中所有支持自动传输的字段传输，用法示例如下:

```csharp
// 使用 AutoSync 特性以使变量自动传输，免去自己写 Send 和 Receive 的功夫
[AutoSync]
public class ExamplePacket : NetModule {
    private byte _exampleByte;
    [ItemSync(syncStack: true, syncFavorite: true)] private Item _exampleItem;
}
```

在这个例子中，`_exampleByte` 与 `_exampleItem` 变量均会自动传输，不需要手动写 `packet.Write(_exampleByte)` 与 `_exampleByte = reader.ReadByte()` 这类麻烦的代码。

其中，`_exampleItem` 使用了特性 `[ItemSync(syncStack: true, syncFavorite: true)]`，这意味着它会同时传输堆叠与是否被标记为收藏的信息，若 `Item` 类型传输不使用特性，则默认只传输堆叠，而不传输收藏信息。对于 `Color` 类型，可以使用 `ColorSync` 特性来决定是否传输 `Alpha` 信息（透明度）

### 对字段使用特性

对字段使用 `AutoSync` 特性可以选择性地自动传输需要的变量，用法示例如下:

```csharp
public class ExamplePacket : NetModule {
    // 使用 AutoSync 特性以使变量自动传输，免去自己写 Send 和 Receive 的功夫
    [AutoSync] private byte _exampleByte;
    private Item _exampleItem;

    public override void Send(ModPacket p) {
        p.Write(_exmapleItem, writeStack: true, writeFavorite: true);
    }

    public override void Read(BinaryReader r) {
        _exmapleItem = r.ReadItem(readStack: true, readFavorite: true);
    }
}
```

在这个例子中，只有 `_exampleByte` 会自动传输，而 `_exmapleItem` 则通过手动编写代码传输。这种方法可以使字段有选择性地自动传输，而在不同的情况下传输不同的变量，便于对数据包的特定操作。

***
## [NetModule 类](src/NetModule.cs)
这是一个基本的网络传输类，用于控制 `ModPacket` 包的发送/接收  
如果你要自定义发包，直接新建一个类并继承 `NetModule` 类即可（注意引用命名空间 `using NetSimplified`）

一个 `NetModule` 类应包含以下函数

### Send(ModPacket p)
该重写函数用于发包时向 `ModPacket` 写入数据，直接使用 `p.Write(xx)` 即可

本模组对于数据类型 `Point`, `Point16`, `Item`, `Item[]` 添加了 `ModPacket.Write()` 扩展方法，可参考[该文件](src/Extensions.cs)

### Read(BinaryReader r)
*不要与 `Receive()` 相混淆*

该重写函数用于读取数据，需要按照在 `Send(ModPacket p)` 中写入的顺序使用 `r.ReadXX()` 依次读取

本模组对于数据类型 `Point`, `Point16`, `Item`, `Item[]` 添加了 `BinaryReader.ReadXX()` 扩展方法，可参考[该文件](src/Extensions.cs)

### Receive()
该重写函数用于对接收到的数据进行操作，将其与 `Read(BinaryReader r)` 分开以规范程序并且实现发包时的 `runLocally` 功能，在下面会讲到

### 实例化
`NetModule` 类继承了类 `ModType`，即会在模组加载时自动读取并创建一个实例类，因此你不能重写构造函数，而应该使用 [`NetModuleLoader`](src/NetModuleLoader.cs) 内的方法来获取你想要的 `NetModule` 实例  
建议使用 `NetModuleLoader.Get<T>()` 方法，使用方法就和 `ModContent.ItemType<T>()` 什么的差不多，这里不多赘述了

### 例子
`NetModule` 结构可参考: [InventoryPacket](NetSimplifiedExample/Packets/InventoryPacket.cs)

发包可参考: [ExamplePacketSender](NetSimplifiedExample/Items/ExamplePacketSender.cs)

***
## [AggregateModule 类](src/AggregateModule.cs)
以一个 `ModPacket` 包的形式发送多个 `NetModule` 包, 能有效避免分散性地多次发包

与普通包一样, 发包时要调用 `AggregateModule.Send(Mod, int, int, bool)`, 注意有一个 `Mod` 参数在前面的，而不是一般的 `NetModule.Send(int, int, bool)`

正常情况下, 其 `NetModule.Type` 应为0, 获取时应调用 `AggregateModule.Get(List<NetModule>)` 而不是 `NetModuleLoader.Get<T>`, 否则会获取到 `null` 值

就是一次性把好几个包一起发出去，避免延迟上多包不同步导致的问题，非常的好用  
### 例子
来自配套的示例Mod，[文件在这](NetSimplifiedExample/Items/ExampleAggregateSender.cs)
```CSharp
// 获取包含了多个 NetModule 包的 AggregateModule 包实例
AggregateModule.Get(new List<NetModule> {
    // 第一个 NetModule 包
    SystemTimePacket.Get(DateTime.Now.ToString(CultureInfo.InvariantCulture)), // 注意逗号
    // 第二个 NetModule 包
    RandomStringPacket.Get()
}).Send(toClient: player.whoAmI); // 发送
```

***
## 发包
发包调用 `NetModule.Send` 即可，获取包实例看上面的  
### 传参
- `toClient` -> 如果不是 `-1`, 则包<b>只会</b>发送给对应的客户端
- `ignoreClient` -> 如果不是 `-1`, 则包<b>不会</b>发送给对应的客户端
- `runLocally` -> 如果为 `true` 则在发包时会调用相应 `NetModule` 包的 `NetModule.Receive()` 方法，<b>默认为 `false`</b>

若 `toClient` 和 `ignoreClient` 皆为 `-1` 时
- 在服务器调用 `Send` -> 发给所有客户端
- 在客户端调用 `Send` -> 发给服务器
***
## 扩展方法
直接看 [Extensions](src/Extensions.cs) 吧（摆烂
