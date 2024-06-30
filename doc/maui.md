# Fair 移动端(MAUI) 应用开发
## 一、创建Maui Blazor 项目
## 二、引用 NuGet包  
 + app端 (必须, fair.maui)
 + 主插件 (必须, fair.extensions.main,也可以自己实现)
 + 账户插件 (可选,fair.extensions.wallet或其它)
 
## 三、修改index.html,加入client.css和client.js
在</head>前加入css文件

```
    <link href="_content/fair.extensions.shared/client.css" rel="stylesheet" />
```
在</body>前面加入 js 脚本
```
    <script type="module" src="_content/fair.extensions.shared/client.js"></script>
```
## 四、并修改  MauiProgram.cs
```
        public static MauiApp CreateMauiApp()
        {
         
            var builder = MauiApp.CreateBuilder();
            return builder
                .UseMauiApp<App>()
              .ConfigureFonts(fonts =>
              {
                  fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              }).RunFairHost( 
                new fair.extensions.main.Extender(),
                new fair.extensions.wallet.Extender() ).Result;
        }
```

## 五、修改 MainPage
```
   public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.blazorWebView.InitBlazorWebView();
        }

    }
```
## 六、修改 MainActivity
```
  public class MainActivity : fair.maui.MainActivity
    {
    }
```