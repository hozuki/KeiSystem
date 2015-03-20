# KeiSystem

分布式内网 Tracker 网络构建软件

当前版本: rev3 (2015-01-30)

------

<del>稍后添加详情。</del>

KeiSystem 的 Java 代码移植见 [KeiSystem-Java](https://github.com/GridScience/KeiSystem-Java/)，包含预计的概念更新。

## 模块

- Kei.Gui: 可视化操作支持。
- Kei.KNetwork: 客户端间的通信支持。
- Kei.KTracker: Tracker 服务器支持。
- Kei.Shared: 包含 KeiSystem 公用类。
- Kei.Tests: KeiSystem 调试程序。 **(已过时)**
- Kei.TorrentNormalize: 修改 .torrent 文件使之成为所用的标准格式。
- MonoTorrent: 引用自 Mono 开发团队的 MonoTorrent，用于编解码 B-编码结构。
- Mono.Nat: 引用自 Alan McGovern, Ben Motmans 的 Mono.Nat，用于实现 NAT 穿越。
