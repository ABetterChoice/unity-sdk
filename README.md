
# 准备工作

您需要在设置模块找到当前游戏的 **Game ID** 和 **API Key**：

- **Game ID**：小游戏在本平台的唯一标识，创建小游戏时由本平台自动生成。
- **API Key**：授权 SDK 访问小游戏数据的密钥，由平台提供，确保数据安全传输和访问控制。

![IMG](https://cdn.abetterchoice.cn/static/cms/4eb92c5def.png)
---

## 一、快速集成

### 1. 导入 SDK

有两种方式将 **ABetterChoice SDK** 集成到您的 Unity 项目中：

#### 方法一：通过 UnityPackage 导入

下载最新版本的 [**ABetterChoice.unitypackage**](https://cdn.abetterchoice.cn/static/cms/ABetterChoice.unitypackage) 资源文件，并将其导入到您的 Unity 项目中：

1. 在 Unity 编辑器顶部菜单栏，选择 **Assets > Import Package > Custom Package**。
2. 在弹出的文件选择对话框中，找到并选择您下载的 **ABetterChoice.unitypackage** 文件。
3. 点击 **Import**，等待 Unity 完成导入。

#### 方法二：通过 Git 安装

您也可以通过 Git 将 **ABetterChoice SDK** 集成到您的项目中：

1. 打开 Unity 编辑器，选择 **Window > Package Manager**。
2. 在 Package Manager 窗口中，点击左上角的 **+** 按钮，选择 **Add package from git URL...**。
3. 在弹出的输入框中，输入 SDK 的 Git 仓库地址：

   ```
   https://github.com/ABetterChoice/unity-sdk.git
   ```

4. 点击 **Add**，Unity 将自动从指定的 Git 仓库拉取 SDK 并添加到您的项目中。

> **注意：**
>
> - **微信小游戏**：需要先安装微信团队官方提供的 [Unity 微信小游戏开发插件](https://github.com/wechat-miniprogram/minigame-unity-webgl-transform?tab=readme-ov-file)。
> - **抖音小游戏**：需要先安装字节提供的 [StarkSDK 插件](https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/guide/game-engine/rd-to-SCgame/unity-game-access/sc_stark_sdk)。

### 2. 添加全局宏参数

针对不同的平台添加对应的全局宏参数：

- **微信小游戏**：`ABC_WECHAT_MINIGAME`
- **抖音小游戏**：`ABC_BYTEDANCE_MINIGAME`

添加步骤如下：

1. 打开 Unity 编辑器，选择 **Edit > Project Settings**。
2. 在左侧菜单中，点击 **Player**，然后选择相应的目标平台（如 **WebGL**）。
3. 展开 **Other Settings**，找到 **Scripting Define Symbols**。
4. 在对应的平台下，输入上述宏参数，多个宏参数之间使用分号 **` ; `** 分隔。
5. 输入完成后，Unity 会自动应用设置。

> **注意：**
>
> 请确保正式打包上线时，选中的宏参数正常生效，否则会影响到对应平台的事件上报，影响实验结果。

---

通过以上步骤，您可以选择最适合的方式将 **ABetterChoice SDK** 集成到您的 Unity 项目中。如果您对 Git 更加熟悉，方法二可以帮助您方便地管理 SDK 的版本和更新。
### 3. 初始化 SDK 参数

在您的代码中，初始化 SDK。示例如下：

```csharp

// 创建配置对象
AbcSDKSpace.Config config = new AbcSDKSpace.Config
{
    GameId = "YOUR_GAME_ID", // 项目游戏ID，必填，可以在ABetterChoice平台管理页查看
    ApiKey = "YOUR_API_KEY", // 项目API KEY，必填，可以在ABetterChoice平台管理页查看
    AutoTrack = new AbcSDKSpace.AutoTrackConfig  // 可选，自动采集配置，默认全部打开
    {
        MgShow = true,  // 自动采集，游戏启动，或从后台进入前台，可选
        MgHide = true,  // 自动采集，游戏从前台进入后台，可选
        MgShare = true  // 自动采集，游戏分享时自动采集，可选
    }
};

// 初始化 SDK
AbcSDKSpace.ABetterChoiceAPI.Init(config, result =>
{
    if (result.statusCode == AbcSDKSpace.StatusCode.Success)
    {
        // 初始化成功
        Debug.Log("SDK 初始化成功");
        
        // 必须，用户的登录唯一标识
        AbcSDKSpace.ABetterChoiceAPI.Login("USER_ID");
    }
    else
    {
        // 初始化失败
        Debug.LogError("SDK 初始化失败：" + result.message);
    }
});
```

**Config 对象其他可选参数说明：**

- **Attributes**：可选，实验分流的属性条件，类型为 `Dictionary<string, List<string>>`，其中 `string` 为条件属性，`List<string>` 为对应的条件属性值数组。
- **EnableAutoExposure**：可选，实验分流使用，默认值为 `false`。如果设置为 `true`，当调用 AB 实验分流时，曝光数据将自动上报。
- **EnableAutoPoll**：可选，实验分流使用，默认值为 `true`。如果设置为 `true`，实验和功能标志数据将每 10 分钟轮询并更新。

**警示**：无论您获取帐号 ID 是异步还是同步的，请在使用 SDK 接入完成后用下面的login接口进行帐号 ID 的登陆，以确保数据计算结果的准确性。
```csharp
// 用户的唯一标识，此数据对应上报数据里的 user_id
AbcSDKSpace.ABetterChoiceAPI.Login("USER_ID");
```

> **注意：**
>
> 如果您的项目为微信小游戏或者抖音小游戏，在上报数据与实验分流之前，请先在对应平台的开发设置中，将下面默认请求 URL 加入到服务器域名的 request 列表中，主要有：
>
> - **分流 URL**：`https://mobile.abetterchoice.cn`
> - **数据上报 URL**：`https://data.abetterchoice.cn`

---

## 二、常用功能

在使用常用功能之前，确保 SDK 已初始化成功并已登录帐号 ID。

### 2.1 设置帐号 ID

在可以获取到用户唯一性信息时调用本方法登录，推荐首次安装启动时调用，其他方法均需在本方法成功之后才可正常使用。

多次调用 `Login` 将覆盖先前的账号 ID。

```csharp
// 用户的唯一标识，此数据对应上报数据里的 user_id
AbcSDKSpace.ABetterChoiceAPI.Login("USER_ID");
```

### 2.2 设置公共属性

公共事件属性指的是每个事件都会带有的属性，您可以调用 `SetCommonProperties` 来设置公共事件属性。我们建议您在发送事件前，先设置公共事件属性。对于一些重要的属性，例如用户的会员等级、来源渠道等，这些属性需要设置在每个事件中，此时您可以将这些属性设置为公共事件属性。

```csharp
// 创建公共属性字典
Dictionary<string, string> commonProperties = new Dictionary<string, string>
{
    { "vipSource", "ABC" }, // 字符串
    { "vipLevel", "1" },    // 数字类型需转换为字符串
    { "isVip", "true" }     // 布尔类型需转换为字符串
};

// 设置公共属性
AbcSDKSpace.ABetterChoiceAPI.SetCommonProperties(commonProperties);
```
公共事件属性将会被保存到缓存中，如果调用 setCommonProperties 上传了先前已设置过的公共事件属性，则会覆盖之前的属性。

- Key 为该属性的名称，为字符串类型，规定只能以字母开头，包含数字，字母和下划线 "_"，长度最大为 50 个字符。
- Value 为该属性的值，目前仅支持字符串。

### 2.3 发送事件

您可以调用 `Track` 方法来上传事件。建议您根据先前梳理的埋点文档来设置事件的属性以及发送信息的条件。以下以用户购买某商品作为范例：

```csharp
// 创建事件属性字典
Dictionary<string, string> eventProperties = new Dictionary<string, string>
{
    { "product_name", "商品名" },
    { "price", "99" },
    { "currency", "CNY" }
};

// 发送事件
AbcSDKSpace.ABetterChoiceAPI.Track("product_buy", eventProperties);
```

### 2.4 获取AB实验

```csharp
// 获取实验分流信息，默认会在获取分流信息同时会进行自动上报。
AbcSDKSpace.ExperimentInfo experiment = AbcSDKSpace.ABetterChoiceAPI.GetExperiment("YourLayerKey");

// 现在您可以获取参数并在代码中直接使用这些参数。
bool yourParamValue = experiment?.GetBoolValue("YourParamKey", true) ?? true;
```

### 2.5 实验曝光

当设置 `EnableAutoExposure` 为 false 时，您可以根据上面获取的实验分流信息，手动记录曝光。

```csharp
// 当设置 EnableAutoExposure 为 false 时，您可以根据上面获取的实验分流信息进行手动记录曝光
AbcSDKSpace.ABetterChoiceAPI.LogExperimentExposure(experiment);
```

### 2.6 获取配置开关

```csharp
// 获取配置开关信息
AbcSDKSpace.ConfigInfo configInfo = AbcSDKSpace.ABetterChoiceAPI.GetConfig("YourConfigKey");

// 获取配置开关的值
bool boolValue = configInfo?.GetBoolValue(true) ?? true;
// string stringValue = configInfo?.GetStringValue("") ?? "";
// int intValue = configInfo?.GetIntValue(0) ?? 0;
```

若配置了条件，如下图创建所示，假设您创造了开关配置参数名称为'new_feature_flag'，配置条件配置参数属性为'city'与'age'，条件参数属性值为'shenzhen'与'18'，则满足这个条件则会进行下发布尔值true。
![IMG](https://cdn.abetterchoice.cn/static/cms/5640e1e9ac.jpeg)
```csharp
// 初始化 SDK 时需配置条件属性 Attributes
AbcSDKSpace.Config config = new AbcSDKSpace.Config
{
    // 其他配置项
    Attributes = new Dictionary<string, List<string>>
    {
        { "city", new List<string> { "shenzhen" } },
        { "age", new List<string> { "18" } }
    }
};



// 获取配置开关参数名称为：new_feature_flag的配置开关值信息
AbcSDKSpace.ConfigInfo configInfo = AbcSDKSpace.ABetterChoiceAPI.GetConfig("new_feature_flag");

// 获取对应配置开关的参数值,其中参数为默认值
var boolValue = configInfo?.GetBoolValue(false) ?? false;
```

---

## 三、最佳实践

整合以上示例，完整展示 SDK 的集成和使用流程：

```csharp
using UnityEngine;
using System.Collections.Generic;

public class ABetterChoiceExample : MonoBehaviour
{
    void Start()
    {
        // 创建配置对象
        AbcSDKSpace.Config config = new AbcSDKSpace.Config
        {
            GameId = "YOUR_GAME_ID", // 项目游戏ID，必填，可以在ABetterChoice平台管理页查看
            ApiKey = "YOUR_API_KEY", // 项目API KEY，必填，可以在ABetterChoice平台管理页查看
            AutoTrack = new AbcSDKSpace.AutoTrackConfig  // 可选，自动采集配置，默认全部关闭
            {
                MgShow = true,  // 自动采集，游戏启动，或从后台进入前台
                MgHide = true,  // 自动采集，游戏从前台进入后台
                MgShare = true  // 自动采集，游戏分享时自动采集
            }
        };

        // 初始化 SDK
        AbcSDKSpace.ABetterChoiceAPI.Init(config, result =>
        {
            if (result.statusCode == AbcSDKSpace.StatusCode.Success)
            {
                // 初始化成功
                Debug.Log("SDK 初始化成功");

                // 登录用户
                AbcSDKSpace.ABetterChoiceAPI.Login("USER_ID");

                // 设置公共事件属性
                Dictionary<string, string> commonProperties = new Dictionary<string, string>
                {
                    { "channel", "ta" },
                    { "age", "1" },
                    { "isSuccess", "true" }
                };
                AbcSDKSpace.ABetterChoiceAPI.SetCommonProperties(commonProperties);

                // 发送事件
                Dictionary<string, string> eventProperties = new Dictionary<string, string>
                {
                    { "product_name", "商品名" },
                    { "price", "100" },
                    { "currency", "CNY" }
                };
                AbcSDKSpace.ABetterChoiceAPI.Track("product_buy", eventProperties);

                // 获取实验分流信息
                AbcSDKSpace.ExperimentInfo experiment = AbcSDKSpace.ABetterChoiceAPI.GetExperiment("abc_layer_name");
                bool shouldShowBanner = experiment?.GetBoolValue("should_show_banner", true) ?? true;
                if (shouldShowBanner)
                {
                    // 显示 Banner
                }
                else
                {
                    // 不显示 Banner
                }

                // 手动记录实验曝光
                AbcSDKSpace.ABetterChoiceAPI.LogExperimentExposure(experiment);

                // 获取配置开关
                AbcSDKSpace.ConfigInfo configInfo = AbcSDKSpace.ABetterChoiceAPI.GetConfig("new_feature_flag");
                bool boolValue = configInfo?.GetBoolValue(true) ?? true;
            }
            else
            {
                // 初始化失败
                Debug.LogError("SDK 初始化失败：" + result.message);
            }
        });
    }
}
```
