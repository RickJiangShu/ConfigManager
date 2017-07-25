![logo](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/Logo.jpg "logo")

# 本工具优势
1. **光速解析**：解析完全放在编辑模式下完成，运行时只需加载序列化文件即可。
2. **支持任意格式**：txt、csv、json、xml、xls、xlsx等等。
3. **无体积**：无运行时脚本，完全不占发行包体积。
4. **智能判断类型**：不需要配置人员懂类型概念，自动选取最优类型。
5. **防止出错**："."操作符索引数据，编译器自动检查。

# 快速使用
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
        
MonsterConfig monsterCfg = MonsterConfig.Get(210102)
print(monsterCfg.name);
```
### 范例
https://github.com/RickJiangShu/ConfigManager-Example
# 特点
### 高性能
大量数据的解析工作完全放在编辑器下完成。<br>
![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p1.jpg "")
<br>
### 易用
提供了操作简单的编辑窗口，轻松点击鼠标操作。<br>
![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p2.jpg "")  
<br>
### 解耦
不关心您项目的资源管理策略，只需在加载配置文件后调用Deserializer即可。<br>
```
//Resource加载
SerializableSet set = Resources.Load<SerializableSet>("SerializableSet");
Deserializer.Deserialize(set);

//AssetBundle加载
AssetBundle bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/config.ab");
SerializableSet set = bundle.LoadAsset<SerializableSet>("SerializableSet");
Deserializer.Deserialize(set);
```
### 支持多种格式
目前支持的格式有：.txt .csv<br>
未来支持：.json .xml，请关注~<br>

### 直接配置数据类型
支持所有C#值类型和数组类型，合理运用有助于减少内存占用。<br>

# 表格格式
以列为属性，以行为一项；<br>
第1行为**注释**；<br>
第2行为**数据类型**<br>
第3行为**字段标识符**<br>
第4行及以下为各项配置<br>
![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p3.jpg "")<br>
<br>

# 支持的数据类型
| 配置类型 | 对应C#类型  | 取值范围 |
| :------------: |:---------------:| :-----:|
|bool|bool|0、false、False、False 都为否，其他为是|
|byte、uint8|byte|0 ~ 255|
|ushort、uint16|ushort|0 ~ 65,535|
|uint、uint32|uint|0 ~ 4,294,967,295|
|ulong、uint64|ulong|0 ~ 18,446,744,073,709,551,615|
|sbyte、int8|sbyte|-128 ~ 127|
|short、int16|short|-32,768 ~ 32,767|
|int、int32|int|-2,147,483,648 ~ 2,147,483,647|
|long、int64|long|-9,223,372,036,854,775,808 ~ 9,223,372,036,854,775,807|
|float|float|-3.4 × 10^38 ~ +3.4 × 10^38|
|double|double|±5.0 × 10^−324 ~ ±1.7 × 10^308|
|string|string|Any|

并支持以上基础类型的所有**数组类型**。<br>
例如：字符串数组类型是string[]，值是Hello,World<br>

# 优化建议
1. 在Deserialize之后，把加载的序列化数据卸载掉。
```
Resources.UnloadUnusedAssets();//Resources卸载内存

bundle.Unload(true);//AssetBundle卸载内存
```
2. 注意GetKeys()、GetValues()是Copy一个数组出来，所以做好缓存的工作。

# 贡献者名单
如果你有任何Bug、问题和意见请在Issues或[蛮牛](http://www.manew.com/thread-105598-1-1.html "一键读取Txt、Excel等表格配置【源码+原理】")里提出来，有时间一定立马回复，意见一经采纳就被列入“贡献者名单”。
1. LiGo 提供希望支持cvs的建议
2. k1104480005 提供希望支持Get所有数据的方法
3. fuliufuliu 希望直接解析xls和xlsx文件
4. zhengyiunity MAC电脑上解析时，注释部分中文乱码。
5. takaaptech 让我意识到不要覆盖AssetBundleName和提供打包回调接口
