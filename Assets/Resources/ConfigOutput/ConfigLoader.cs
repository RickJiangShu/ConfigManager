using UnityEngine;
/// <summary>
/// 不要手动更改，由ConfigEditor自动生成的配置文件（模板为LoaderTemplete）
/// </summary>
public class ConfigLoader
{
	public static void Load()
	{
        //各配置加载自动生成在此行下面
		int startIndex = ConfigManagerSettings.FilesURL.LastIndexOf("Resources/") + 10;
		int length = ConfigManagerSettings.FilesURL.Length - startIndex;
		string relativeURL = ConfigManagerSettings.FilesURL.Substring(startIndex, length);

		string EquipText = Resources.Load<TextAsset>(relativeURL + "/" + "Equip").text;
		EquipConfig.Parse(EquipText);

		string MonsterText = Resources.Load<TextAsset>(relativeURL + "/" + "Monster").text;
		MonsterConfig.Parse(MonsterText);

		string TestTypesText = Resources.Load<TextAsset>(relativeURL + "/" + "TestTypes").text;
		TestTypesConfig.Parse(TestTypesText);


	}
}