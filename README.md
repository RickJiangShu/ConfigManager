# ConfigManager

## 简介
ConfigManager使开发人员一键导入并使用配置文件。<br>
Github：https://github.com/RickJiangShu/ConfigManager

该插件免费使用，对该插件有任何问题请联系QQ：402745287

## 特点
1、将*.txt文件一键解析成*.cs文件（称之getter），方便直观；<br>
2、支持所有C#基础类型且支持数组格式；


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

