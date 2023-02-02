# 如何設置開發環境 ?

## IDE
以下二選一即可
| IDE | Download Link |
|-|-|
| Visual Studio Code | https://code.visualstudio.com/download |
| Visual Studio 2022 Community | https://visualstudio.microsoft.com/free-developer-offers/ |

## .NET 7 SDK
https://dotnet.microsoft.com/en-us/download/dotnet/7.0

進去後, 表格左邊是 SDK, 右邊是 Runtime

我們要的是 SDK

(如果 IDE 選擇 Visual Studio 2022, 他安裝自帶 .NET 7 SDK, 就不用再安裝了)

### 要如何知道 .NET SDK 安裝成功 ?
在 Terminal 下打 `dontnet --info`

你會看到類似以下的訊息, 那就是安裝成功啦

```
> dotnet --info
.NET SDK:
 Version:   7.0.101
 Commit:    bb24aafa11

Runtime Environment:
 OS Name:     Windows
 OS Version:  10.0.19045
 OS Platform: Windows
 RID:         win10-x64
 Base Path:   C:\Program Files\dotnet\sdk\7.0.101\

Host:
  Version:      7.0.1
  Architecture: x64
  Commit:       97203d38ba

.NET SDKs installed:
  3.1.426 [C:\Program Files\dotnet\sdk]
  5.0.408 [C:\Program Files\dotnet\sdk]
  6.0.113 [C:\Program Files\dotnet\sdk]
  6.0.300 [C:\Program Files\dotnet\sdk]
  6.0.302 [C:\Program Files\dotnet\sdk]
  6.0.308 [C:\Program Files\dotnet\sdk]
  7.0.101 [C:\Program Files\dotnet\sdk]
```
## Docker Desktop
https://www.docker.com/products/docker-desktop/

### 要如何知道 Docker Desktop 安裝成功 ?
在 Terminal 下打 `docker --version`
你會看到類似以下的訊息, 那就是安裝成功啦
```
> docker --version
Docker version 20.10.21, build baeda1f
```


