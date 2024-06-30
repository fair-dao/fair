# Fair 插件开发
# 扩展开发流程

## 创建一个Razor库项目,项目名称如：fair.extensions.sample（举例）

##  添加 nuget包 fair.extensions.shared

### 创建 Extender.cs PageBase.cs类，参考 fair.extensions.wallet
###  如果插件有CSS隔离文件，请自行添加到相应的index.html里
  @import url("/_content/fair.extensions.sample/fair.extensions.sample.bundle.scp.css");

### 界面初始菜单配置
 修改 /res/initConfig.json,注意如果不在fair.extensions程序集则 Link需要指名程序集
 如:
 {
      "Id": "sample",
      "Parent": "tab",
      "Icon": "sample",
      "Text": "实例",
      "SortId": 100,
      "State": 1,
      "ShowMode": 8,
      "Link": "fair.extensions.sample.Pages.Index,fair.extensions.sample",
      "Roles": [],
      "SubMenus": []
    }

