# ConfigManager：一键使用Excel等表格配置
![logo](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/Logo.jpg "logo")

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
        
MonsterConfig monsterCfg = MonsterConfig.Get(210102)
print(monsterCfg.name);
```
<br>
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
<br>

### 支持多种格式
目前支持的格式有：*.txt *.cvs<br>
<br>
### 直接配置数据类型
支持所有C#值类型和数组类型，合理运用有助于减少内存占用。<br>
<br>
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
<br>
并支持以上基础类型的所有**数组类型**。<br>
例如：字符串数组类型是string[]，值是Hello,World<br>
<br>



* 解耦
与资源加载解耦，等您的资源加载完成后，只需调用一个方法反序列化即可。




* 做一张宣传图

* 写插件手册
	用途
	特点
	快速使用
	优化建议
	示例
	贡献名单

