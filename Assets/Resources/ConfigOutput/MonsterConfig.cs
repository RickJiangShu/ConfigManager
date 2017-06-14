using System;
using System.Collections.Generic;

/// <summary>
/// 不要手动更改，由ConfigEditor自动生成的配置文件（模板为GetterTemplete）
/// </summary>
public class MonsterConfig
{
	//属性
	public uint id;//怪物ID
	public string name;//名称
	public byte coinBase;//金币系数
	public byte tokenBase;//转生币系数
	public byte expBase;//经验系数
	public uint eggID;//宠物蛋ID
	public uint Attack;//攻击
	public uint HealthPpint;//生命
	public float HealthRegen;//生命回复
	public uint SkillAttack;//技能攻击
	public float AttackSpead;//攻击速度
	public float AttackRange;//攻击距离
	public float CritChance;//暴击几率
	public float CriticalDamage;//暴击伤害
	public float SplashDamage;//溅射伤害
	public float Dodge;//闪避
	public float MoveSpeed;//移动速度
	public string modelID;//怪物形象
	public float modelSize;//怪物尺寸
	public string effectName;//攻击特效名称
	public float effectTime;//攻击特效持续时间
	public int atkAnime;//攻击动作
	public string detail1;//BOSS说明1
	public string detail2;//BOSS说明2
	public string detail3;//BOSS说明3
	

	#region 静态方法
	public static Dictionary<uint, MonsterConfig> Map;

	public static MonsterConfig Get(uint id)
	{
		return Map[id];
	}
	public static void Parse(string cfgStr)
	{
		Map = new Dictionary<uint, MonsterConfig>();
		string[][] configArray = ConfigUtils.ParseConfig(cfgStr);
		int len = configArray.Length;
        
		for(int i = 3;i<len;i++)
		{
			string[] args = configArray[i];
			MonsterConfig cfg = new MonsterConfig();
			
			cfg.id = uint.Parse(args[0]);
			cfg.name = Convert.ToString(args[1]);
			cfg.coinBase = byte.Parse(args[2]);
			cfg.tokenBase = byte.Parse(args[3]);
			cfg.expBase = byte.Parse(args[4]);
			cfg.eggID = uint.Parse(args[5]);
			cfg.Attack = uint.Parse(args[6]);
			cfg.HealthPpint = uint.Parse(args[7]);
			cfg.HealthRegen = float.Parse(args[8]);
			cfg.SkillAttack = uint.Parse(args[9]);
			cfg.AttackSpead = float.Parse(args[10]);
			cfg.AttackRange = float.Parse(args[11]);
			cfg.CritChance = float.Parse(args[12]);
			cfg.CriticalDamage = float.Parse(args[13]);
			cfg.SplashDamage = float.Parse(args[14]);
			cfg.Dodge = float.Parse(args[15]);
			cfg.MoveSpeed = float.Parse(args[16]);
			cfg.modelID = Convert.ToString(args[17]);
			cfg.modelSize = float.Parse(args[18]);
			cfg.effectName = Convert.ToString(args[19]);
			cfg.effectTime = float.Parse(args[20]);
			cfg.atkAnime = int.Parse(args[21]);
			cfg.detail1 = Convert.ToString(args[22]);
			cfg.detail2 = Convert.ToString(args[23]);
			cfg.detail3 = Convert.ToString(args[24]);
			
			
			Map[cfg.id] = cfg;
		}
	}
	#endregion
}
