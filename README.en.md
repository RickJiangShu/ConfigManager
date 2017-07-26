# Config Manager
Fast & Easy Config Manage Tool<br>

*Read this in other languages: [简体中文](README.md) [English](README.en.md)*
![logo](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Poster/Icon/Icon516x389.png "logo")

# Features
1. **Fast Parse**: Parse in Editor
2. **Formats**: txt,csv,json,xml,xls,xlsx...
3. **No size**: all scripts in editor
4. **Intelligent**: Auto select type 
5. **Reduce errors**: "."operation

# Quick Start
### Edit Configs：
![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p3.jpg "")<br>
[Sheet](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Doc/Sheet.md "Sheet")<br>
[Json](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Doc/Json.md "Json")<br>
[Xml](https://github.com/RickJiangShu/ConfigManager-Example/blob/master/Doc/Xml.md "Xml")<br>

### In Editor：
1. Press menu "Window/Config Manager";
2. Set paths;
3. Press "output"

![](https://raw.githubusercontent.com/RickJiangShu/ConfigManager-Example/master/Poster/p4.jpg "")
<br>
### In Runtime：
1. Call Deserialize();
2. Call Get() use them.
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

### Example：
https://github.com/RickJiangShu/ConfigManager-Example

