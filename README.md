# ZendeeAntiCheatClient

## 项目介绍

隶属于ZendeeAntiCheat项目的子项目，以下简称ZAntiCheat，或ZAC。功能为反作弊客户端，运行在用户电脑上，并在游戏时保持随时开启。

## 配置说明

该项目，即ZACClient，包含两个子项目，一个是Driver，包含驱动源码和模块，采用易语言编写，成品为`ZAntiCheat.dll`，需要和另一个子项目ZAntiCheatClient一起使用

ZAntiCheatClient文件夹为主项目，语言是C#，需要用VisualStudio2019或更高打开，然后编译。编译后和ZAntiCheat.dll放置于同一目录即可。

## 编译说明

### APIKEY的修改

首先需要你随机指定一个APIKEY，长度任意，只要是字符串就行，这个APIKEY尽量不要泄露。然后修改ZAntiCheatClient的项目中`NetWork.cs`文件中的

``` c#
private static string API_KEY = "[APIKEY]";
```

为你自己定义的KEY，同时注意与搭建项目ZACPHP里的apikey保持一致

下面的几个常量也需要自己修改为搭建ZACPHP服务器的地址

```c#
private static string API_MD5 = "http://[APIURL]/md5.php";
private static string API_SERVER_STATUS = "http://[APIURL]/ServerStatus.php";
private static string API_UPLOAD_UUID = "http://[APIURL]/UUIDbeat.php";
private static string API_CANCEL_UUID = "http://[APIURL]/cUUID.php";
private static string API_EXIT = "http://[APIURL]/canexit.php";
```

## 测试说明

如果你修改完APIKEY，并且在保证ZACPHP服务器开启的情况下，你可以运行ZACClient，如果软件界面没有显示任何错误，并且服务器状态处于正常，则表示配置正确，即可编译。

