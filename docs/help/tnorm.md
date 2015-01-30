# KeiSystem 种子文件标准化工具 (KeiTNorm)

KeiTNorm 是 KeiSystem.TorrentNormalizer 的简写，对应 KeiTNorm.exe。

这个程序的作用是将已有的种子标准化以用于 KS 网络内部的查找。其使用方式同时也是用于 KS 网络的种子的制作教程。

## 可视化使用方式

> 我认为批量处理于属于高级应用范畴，所以设计上可视化的方式一次只能处理一个文件。多个文件的处理请用命令行或其拖曳方式。

### 第 1 步

双击 KeiTNorm.exe（MacOS/Linux 用户请执行 `mono KeiTNorm.exe` 命令），可见 KeiTNorm 窗口显示在屏幕上。

### 第 2 步

将一个有效的种子文件（自制的、公网或者其他 PT 站的种子）拖曳到窗口中的文本框上。文本框中将显示出该种子的主要信息。

### 第 3 步

单击“生成新种子”按钮。如果预计的位置已经有了相应的文件，会询问您是否覆盖。

会在原文件的目录下生成以“[KS].”为前缀的新文件。

例如，原文件名为 `[FGBT].[CASO&SumiSora][Le_Fruit_de_la_Grisaia][01-13][GB][720p].torrent`，则生成的文件名将是 `[KS].[FGBT].[CASO&SumiSora][Le_Fruit_de_la_Grisaia][01-13][GB][720p].torrent`。

生成的种子就可以发布了。

### 第 4 步

将生成的种子文件添加到 BT 客户端的任务列表中。由于默认参数已经配置好，因此一般不需要其他配置。

如果出现红种现象，请检查：

1. KeiSystem 客户端是否在运行，如果不是请运行之。
2. 种子的 tracker 应为 `http://localhost:{本机tracker服务器端口号}/announce`，如果不是请修改之。

## 命令行使用方式

> 该方式同样支持拖曳，直接将一个或多个有效的种子文件拖曳到 KeiNorm.exe 上并释放鼠标，就会自动生成标准化后的种子文件。
>
> 另外，**MacOS 和 Linux 下的命令行方式未做测试**。

命令行格式如下：

`KeiTNorm.exe [{torrent1} [{torrent2} [...]]]`

则将分别在 `{torrent1}`、`{torrent2}` 等等位置生成对应的新种子文件。

------

最后更新于 2015-01-29

[返回主页](./index.htm)

Copyright (C) 2015 micstu@FGBT