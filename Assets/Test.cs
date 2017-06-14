using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ConfigLoader.Load();

        MonsterConfig monsterCfg = MonsterConfig.Get(210102);
        EquipConfig equipCfg = EquipConfig.Get(601110);
        TestTypesConfig typesCfg = TestTypesConfig.Get("1");

        print(monsterCfg.name);
        print(equipCfg.boxBonus);
        print(typesCfg.p15);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
