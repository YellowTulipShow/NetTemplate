# 模板调用命令

## 使用

下载源码后:

发布环境: `.Net Core 3.1+`
```powershell
./shell/release.ps1
```

安装:
```powershell
./shell/install_command_packages.ps1
```

获取运行帮助:
```powershell
ctemp -h
```

测试使用的示例, 可以参考
```powershell
./_test/run.ps1
```

### 注意!!!

在执行单条输出操作时:

最后的数据JSON文件路径不要写成:

```powershell
ctemp single ... --data <key>:<path> <key>:<path> <key>:<path> ...
```

请写成:

```powershell
ctemp single ... --data <key>:<path> --data <key>:<path> --data <key>:<path> --data ...
```

每一个键值数据对前面都加一个 `--data` 参数名称, 正常情况时是可以不用加的, 但使用的库: `System.CommandLine.2.0.0-beta4.22272.1` 不知道为什么不好用, 应该是BUG, 后续再看

* [.NET Core/.NET5/.NET6 开源项目汇总13：模板引擎](https://www.cnblogs.com/SavionZhang/p/15134445.html)
* [liquid 模板引擎语法中文文档](https://www.coderbusy.com/liquid/)
* [.net Fluid 基于 liquid 实现的模板引擎](https://www.nuget.org/packages/Fluid.Core)
