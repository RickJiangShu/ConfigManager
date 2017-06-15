using System;
using System.Collections.Generic;

/// <summary>
/// 不要手动更改，由ConfigEditor自动生成的配置文件（模板为GetterTemplete）
/// </summary>
public class EquipConfig
{
	//属性
	public uint EquipId;//装备ID
	public string EquipIcon;//装备图标
	public string EquipName;//装备名称
	public uint RandomRatio;//随机权值
	public uint EquipType;//装备部件
	public uint EquipStar;//装备品质
	public string EquipStat;//装备属性
	public uint MainStat;//主属性
	public float StatRatio;//主属性成长系数
	public float StatBase;//主属性基础值
	public uint NameShader;//名称材质处理
	public string Particles;//粒子特效
	public string ParticlesPara;//粒子特效参数
	public string Model;//武器模型
	public float boxBonus;//宝箱系数
	

	#region 静态方法
	public static Dictionary<uint, EquipConfig> Map;

	public static EquipConfig Get(uint EquipId)
	{
		return Map[EquipId];
	}
	public static void Parse(string cfgStr)
	{
		Map = new Dictionary<uint, EquipConfig>();
		string[][] configArray = ConfigUtils.ParseConfig(cfgStr);
		int len = configArray.Length;
        
		for(int i = 3;i<len;i++)
		{
			string[] args = configArray[i];
			EquipConfig cfg = new EquipConfig();
			
			cfg.EquipId = uint.Parse(args[0]);
			cfg.EquipIcon = Convert.ToString(args[1]);
			cfg.EquipName = Convert.ToString(args[2]);
			cfg.RandomRatio = uint.Parse(args[3]);
			cfg.EquipType = uint.Parse(args[4]);
			cfg.EquipStar = uint.Parse(args[5]);
			cfg.EquipStat = Convert.ToString(args[6]);
			cfg.MainStat = uint.Parse(args[7]);
			cfg.StatRatio = float.Parse(args[8]);
			cfg.StatBase = float.Parse(args[9]);
			cfg.NameShader = uint.Parse(args[10]);
			cfg.Particles = Convert.ToString(args[11]);
			cfg.ParticlesPara = Convert.ToString(args[12]);
			cfg.Model = Convert.ToString(args[13]);
			cfg.boxBonus = float.Parse(args[14]);
			
			
			Map[cfg.EquipId] = cfg;
		}
	}
	#endregion
}
