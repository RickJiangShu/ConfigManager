using System.Collections.Generic;

[System.Serializable]
public class MonsterConfig
{
    /// <summary>
    /// 怪物ID
    /// </summary>
    public uint id;

    /// <summary>
    /// 名称
    /// </summary>
    public string name;

    /// <summary>
    /// 金币系数
    /// </summary>
    public byte coinBase;

    /// <summary>
    /// 转生币系数
    /// </summary>
    public byte tokenBase;

    /// <summary>
    /// 经验系数
    /// </summary>
    public byte expBase;

    /// <summary>
    /// 宠物蛋ID
    /// </summary>
    public uint eggID;

    /// <summary>
    /// 攻击
    /// </summary>
    public uint Attack;

    /// <summary>
    /// 生命
    /// </summary>
    public uint HealthPpint;

    /// <summary>
    /// 生命回复
    /// </summary>
    public float HealthRegen;

    /// <summary>
    /// 技能攻击
    /// </summary>
    public uint SkillAttack;

    /// <summary>
    /// 攻击速度
    /// </summary>
    public float AttackSpead;

    /// <summary>
    /// 攻击距离
    /// </summary>
    public float AttackRange;

    /// <summary>
    /// 暴击几率
    /// </summary>
    public float CritChance;

    /// <summary>
    /// 暴击伤害
    /// </summary>
    public float CriticalDamage;

    /// <summary>
    /// 溅射伤害
    /// </summary>
    public float SplashDamage;

    /// <summary>
    /// 闪避
    /// </summary>
    public float Dodge;

    /// <summary>
    /// 移动速度
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// 怪物形象
    /// </summary>
    public string modelID;

    /// <summary>
    /// 怪物尺寸
    /// </summary>
    public float modelSize;

    /// <summary>
    /// 攻击特效名称
    /// </summary>
    public string effectName;

    /// <summary>
    /// 攻击特效持续时间
    /// </summary>
    public float effectTime;

    /// <summary>
    /// 攻击动作
    /// </summary>
    public int atkAnime;

    /// <summary>
    /// BOSS说明1
    /// </summary>
    public string detail1;

    /// <summary>
    /// BOSS说明2
    /// </summary>
    public string detail2;

    /// <summary>
    /// BOSS说明3
    /// </summary>
    public string detail3;


}
