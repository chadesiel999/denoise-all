
## 当前使用的产品ID，记录在此，以免和被意外变更了，导致安装包无法正常安装
> 主程序安装包：3E665AA4-D8FB-4431-8950-CEACFFBAE903
> 
> 固件升级安装包：4FC819E0-BBF1-4891-B04F-0886EF695F02
>
> 如果脚本中的产品ID被修改，请以此处为准，否则导致`无法覆盖安装`，识别为两个产品了。

## 打包脚本说明
安装包的制作使用[Inno Setup](https://github.com/jrsoftware/issrc)来生成。
> [Inno Setup](https://github.com/jrsoftware/issrc)的官网地址为：[https://jrsoftware.org/isinfo.php](https://jrsoftware.org/isinfo.php)
>
> 多语言文件下载地址：[https://jrsoftware.org/files/istrans/](https://jrsoftware.org/files/istrans/)
>
> 中文下载地址：[https://raw.github.com/jrsoftware/issrc/main/Files/Languages/Unofficial/ChineseSimplified.isl](https://raw.github.com/jrsoftware/issrc/main/Files/Languages/Unofficial/ChineseSimplified.isl)
>
> 文档地址为：[https://jrsoftware.org/ishelp/](https://jrsoftware.org/ishelp/)
>
> 脚本文件地址[点击这里查看](./GHzScopeX.iss)

## Inno Setup安装说明
Inno Setup直接从官网下载安装即可，唯一需要注意的是，我们需要单独下载中文语言，放在Inno Setup安装目录下的`Languages`文件夹下。

## 安装功能列表
- U2程序安装
- 字体安装(MiSan)
- .Net6 安装
- TMC驱动程序安装
- PCIE驱动安装
- Vc++ 2010 安装
- Vc++ 2010 Sp1 安装 
- Vc++ Redistributable for Visual Studio 2015-2022 安装
- 操作系统 **`桌面壁纸`** 安装
- 操作系统 **`锁屏壁纸`** 安装
- Msedge浏览器安装
- 禁用操作系统开始菜单栏-电源中的 “睡眠” 按钮，使其不可见
- 修改操作系统中的OEM信息，包括：制造商、型号、支持时间、支持电话号码、支持网站URL
- 配置当前电源管理方案，将其中的 “`使计算机进入睡眠状态`”设置为“`从不`”
- 设置U2程序为开机启动
- 自动显示软键盘
- 固件升级功能，使用的是`ScopeX.Updater.exe`程序按照单个upd文件处理的

> **注意：** 以上配置除了U2会在卸载时被卸载，其他设置都不会被卸载。

## 脚本调整说明
脚本文件[GHzScopeX.iss](./GHzScopeX.iss)中定义了需要依赖的各个资源文件的地址，在不同的机器上可能不一样，此时需要调整。如下示例所示，所有路径都在`==目录定义==`中
```ini
;====================目录定义====================
#define SetupICO "D:\SourceCode\xxx\app.ico"
#define DotNet6Path "D:\XXX\dotnet-sdk-6.0.417-win-x64.exe"
;====================目录定义====================
```

### 字体安装说明
字体安装源来自U2程序目录下的`Fonts`文件夹下(也就是要求U2程序目录下要有`Fonts`文件夹，里面是所有要安装的字体文件)，且名字是固定的，不能修改，打包前请确保其存在。名字如下：
- Fonts\MiSans-Bold.ttf
- Fonts\MiSans-Demibold.ttf
- Fonts\MiSans-ExtraLight.ttf
- Fonts\MiSans-Heavy.ttf
- Fonts\MiSans-Light.ttf
- Fonts\MiSans-Medium.ttf
- Fonts\MiSans-Normal.ttf
- Fonts\MiSans-Regular.ttf
- Fonts\MiSans-Semibold.ttf
- Fonts\MiSans-Thin.ttf

### 驱动程序
驱动程序的名称是固定的，也就是脚本里面写死的
- PCIE的驱动文件夹下要求必须有一个名为：`ghz.scope.acq.inf`的驱动安装程序
- TMC驱动文件夹下要求必须有一个名为：`GigahertzUSB3.inf`的驱动安装文件

### 锁屏壁纸和桌面壁纸说明
目前锁屏壁纸文件名称是固定的，请确保其正确。
如果要修改其名称，则需要替换脚本中的所有对应字符串。

- 锁屏壁纸名称：`LockScreen.jpg`
- 桌面壁纸名称：`Desktop.jpg`