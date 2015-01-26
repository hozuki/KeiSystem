# KeiGui

KeiGui 是 KeiSystem 的图形界面操作程序。它通过图形化的方式提供 KeiSystem 的基础操作。

以下部分将介绍 KeiGui 的[界面组成](#interface)及[使用方式](#usage)。

## <span id="interface">界面</span>

### 菜单栏

包括“文件”菜单、“操作”菜单和“帮助”菜单。

**“文件”菜单**

- 退出: 退出 KeiGui。

**“操作”菜单**

- 强制广播: 开启“强制广播”模式。见[使用](#usage)部分。
- 选项: 显示一些 KeiGui 的选项。

**“帮助”菜单**

- 帮助主题: 显示帮助内容。
- 关于: 显示关于 KeiSystem 的信息。

### “服务器”分组框

- “内网地址”下拉框: 选择准备使用的内网地址。见[使用](#usage)部分。
- “K客户端监听端口”: 填入本 KeiSystem 要使用的监听端口。默认为 `9029`。
- “Tracker 服务器监听端口”: 填入 tracker 服务器要使用的监听端口。默认为 `9057`。
- “启动”按钮: 确认上述设置正确后，点击以启动服务器。

### “接入分布网络”分组框

- “接入点”下拉框: 填入或选择要使用的接入点。
- “连接”: 连接到接入点，加入 KS 网络。

### “信息”分组框

- “本地K客户端端点”: 其他客户端用来连接本客户端时，对方应该填入的端点信息。
- “本地 Tracker”: 本地种子应该使用的 tracker 服务器地址。

### 状态栏

- 状态栏: 当前软件的工作状态。

### “选项”窗口

- “启用日志记录”: 是否记录日志并保存到 `ksyslog.log`（以供调试）。默认为选中。
- “强制广播时间”: 强制广播的持续时间。

### 通知区域图标

双击可以显示主窗口。

菜单项：

- 显示: 显示主窗口。
- 强制广播: 开启“强制广播”模式。见[使用](#usage)部分。
- 退出: 退出 KeiGui。

## <span id="usage">使用</span>

### 第 1 步: 启动 KeiSystem 客户端

双击 KeiGui.exe（MacOS/Linux 用户请执行 `mono KeiGui.exe` 命令），可见 KeiGui 窗口显示在屏幕上。

如果弹出提示“无有效内网地址”，说明您的网络配置有问题，请咨询专业人士解决。

### 第 2 步: 启动本地服务器

在“内网地址”下拉框中选择一个有效的内网地址。“K客户端监听端口”和“Tracker 服务器监听端口”保持默认即可。

> 有效的内网地址一般形式为 10.\*.\*.\*、172.16.\*.\*-172.31.\*.\*、192.168.\*.\*。但是请注意，一些保留地址，如 192.168.0.1，不是有效的内网地址。下拉列表中含有多项的同学特别要注意。如果启动失败，大部分原因是因为您使用了一个无效的内网地址，不过也不排除端口被占用的情形。可以查阅程序日志（如果启用了日志记录）查看失败的原因。

### 第 3 步: 接入 KS 网络

在“接入点”下拉框中填入或选择一个接入点，点击“连接”按钮。

如果连接失败，请更换接入点。如果所有接入点连接失败……好吧，您暂时无法加入 KS 网络，不过内网中的其他人也许能连接到您这儿。不过即使接入失败，本机的服务器依然是有效的。

### 第 4 步: 启动 BT 客户端

启动 BT 客户端，例如 μTorrent、BitComet 和 Transmission。

### 第 5 步: 与 BT 相关的操作

在 BT 客户端中添加、删除种子等等，KS 客户端会自动处理。

### 第 6 步: 退出

退出 BT 客户端和 KS 客户端。

### 其他

#### “强制广播”模式

“强制广播”模式是设计用来解决因某些“恰巧”的客户端连接时机导致无法找到一些种子的情况的。当该模式开启的时候，所有的 tracker 消息都会变成广播消息。这有助于快速找到种子（但不保证一定能找到），不过会极大地占用网络资源，所以请尽量少使用该模式。

如果您确定有多个客户端连接着，种子源充足，但是仍然无法找到种子，可以尝试按顺序执行以下操作：

1. 在 BT 客户端中选取要找的种子；
2. 停止这些种子对应的任务；
3. 启动这些种子对应的任务。

这样，就可以很大可能性地找到刚才选中的那些种子。

------

最后更新于 2015-01-27

[返回主页](./index.htm)

Copyright (C) 2015 micstu@FGBT