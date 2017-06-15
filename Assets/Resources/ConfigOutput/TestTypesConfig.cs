using System;
using System.Collections.Generic;

/// <summary>
/// 不要手动更改，由ConfigEditor自动生成的配置文件（模板为GetterTemplete）
/// </summary>
public class TestTypesConfig
{
	//属性
	public string id;//测试所有类型
	public bool p1;//布尔
	public byte p2;//uint8
	public ushort p3;//uint16
	public uint p4;//uint32
	public sbyte p5;//int8
	public short p6;//int16
	public int p7;//int32
	public long p8;//long
	public ulong p9;//ulong
	public float p10;//float
	public double p11;//double
	public string p12;//string
	public int[] p13;//int数组
	public uint[] p14;//uint数组
	public string[] p15;//string数组
	

	#region 静态方法
	public static Dictionary<string, TestTypesConfig> Map;

	public static TestTypesConfig Get(string id)
	{
		return Map[id];
	}
	public static void Parse(string cfgStr)
	{
		Map = new Dictionary<string, TestTypesConfig>();
		string[][] configArray = ConfigUtils.ParseConfig(cfgStr);
		int len = configArray.Length;
        
		for(int i = 3;i<len;i++)
		{
			string[] args = configArray[i];
			TestTypesConfig cfg = new TestTypesConfig();
			
			cfg.id = Convert.ToString(args[0]);
			cfg.p1 = bool.Parse(args[1]);
			cfg.p2 = byte.Parse(args[2]);
			cfg.p3 = ushort.Parse(args[3]);
			cfg.p4 = uint.Parse(args[4]);
			cfg.p5 = sbyte.Parse(args[5]);
			cfg.p6 = short.Parse(args[6]);
			cfg.p7 = int.Parse(args[7]);
			cfg.p8 = long.Parse(args[8]);
			cfg.p9 = ulong.Parse(args[9]);
			cfg.p10 = float.Parse(args[10]);
			cfg.p11 = double.Parse(args[11]);
			cfg.p12 = Convert.ToString(args[12]);
			cfg.p13 = ConfigUtils.ParseArray<int>(args[13], int.Parse);
			cfg.p14 = ConfigUtils.ParseArray<uint>(args[14], uint.Parse);
			cfg.p15 = ConfigUtils.ParseArray<string>(args[15], Convert.ToString);
			
			
			Map[cfg.id] = cfg;
		}
	}
	#endregion
}
