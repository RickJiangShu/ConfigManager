# Config Manager
ConfigManager帮您一键生成配置对应的解析类，并将其序列化。<br>
你只需轻点鼠标即可读取配置~<br>

*其他语言版本: [简体中文](README.md) [English](README.en.md)*<br>
![logo](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Poster/Icon/Icon516x389.png "logo")

# 本工具优势
1. **光速解析**：解析完全放在编辑模式下完成，运行时只需加载序列化文件即可。
2. **支持任意格式**：txt、csv、json、xml、xls、xlsx等等。
3. **无体积**：无运行时脚本，完全不占发行包体积。
4. **智能判断类型**：不需要配置人员懂类型概念，自动选取最优类型。
5. **防止出错**："."操作符索引数据，编译器自动检查。

# 快速使用
### 编辑配置：
![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p3.jpg "")<br>
[表格配置说明](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Doc/Sheet.md "表格配置说明")<br>
[Json配置说明](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Doc/Json.md "Json配置说明")<br>
[Xml配置说明](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Doc/Xml.md "Xml配置说明")<br>

### 编辑器：
1. 点击菜单栏"Window/Config Manager"；
2. 设置对应的输入/输出路径；
3. 点击Output。

![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p4.jpg "")
<br>
### 运行时：
1. 调用反序列化接口；
2. 使用配置文件。
```
SerializableSet set = Resources.Load<SerializableSet>("SerializableSet");
Deserializer.Deserialize(set);

/* 与加载解耦，不依赖加载方式
AssetBundle bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/config.ab");
set = bundle.LoadAsset<SerializableSet>("SerializableSet");
Deserializer.Deserialize(set);
*/
        
MonsterSheet monsterSheet = MonsterSheet.Get(210102)
print(monsterSheet.name);
```

![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p7.jpg "")

### 范例：
https://github.com/RickJiangShu/ConfigManager-Example

# 贡献者名单
如果你有任何Bug、问题和意见请在Issues或[蛮牛](http://www.manew.com/thread-105598-1-1.html "一键读取Txt、Excel等表格配置【源码+原理】")里提出来，有时间一定立马回复，意见一经采纳就被列入“贡献者名单”。
1. LiGo 提供希望支持csv的建议
2. k1104480005 提供希望支持Get所有数据的方法
3. fuliufuliu 希望直接解析xls和xlsx文件
4. zhengyiunity MAC电脑上解析时，注释部分中文乱码。
5. takaaptech 让我意识到不要覆盖AssetBundleName和提供打包回调接口
6. nijjkk 反馈在Mac上Excel输入中文会带拼音
