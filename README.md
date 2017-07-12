

# ConfigManager
一键使用Excel等表格配置<br>
![logo](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/Logo.jpg "logo")  

# 特点

### __高性能__
大量数据的解析工作完全放在编辑器下完成。<br>
![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p1.jpg "")  

### 易用
提供了操作简单的编辑窗口，轻松点击鼠标操作。<br>
![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p2.jpg "")  

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
目前支持的格式有：*.txt *.cvs<br>

### 支持直接配置数据类型和注释





### 表格格式

### 支持的数据类型
| 配置类型 | 对应C#类型  | 取值范围 |
| :------------: |:---------------:| :-----:|
|bool|bool|0、false、False、False 都为否，其他为是|


## 简介
ConfigManager使开发人员一键导入并使用配置文件。<br>
Github：https://github.com/RickJiangShu/ConfigManager<br>

## 特点
1、将*.txt文件一键解析成*.cs文件（称之getter），方便直观；<br>
2、支持所有C#基础类型且支持数组格式；





* 解耦
与资源加载解耦，等您的资源加载完成后，只需调用一个方法反序列化即可。


## 使用流程
1、策划从文档导出约定格式的txt文件；<br>
2、将txt放入到工程Resources/下面；<br>
3、点击菜单ConfigManager/Output；<br>
4、在代码中调用一次ConfigLoader.Load()；<br>
5、在使用 XXConfig.Get(id) 即可。<br>

## 配置文件格式
1、文件类型*.txt；<br>
2、编码选择UTF-8；<br>
3、第1行注释，第2行类型，第3行标识符；<br>
4、配置以“行”为单位，即每行为一个数据。第一列用于索引数据；<br>
5、字符串不要加上双引号""；<br>


* 做一张宣传图

* 写插件手册
	用途
	特点
	快速使用
	优化建议
	示例
	贡献名单

