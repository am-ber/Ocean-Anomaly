using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSaveData : SaveData
{
	public bool IsNew { get; set; }
	public UpgradeSaveData()
	{
		IsNew = true;
	}
}
