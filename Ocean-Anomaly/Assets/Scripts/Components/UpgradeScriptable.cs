using OceanAnomaly.Attributes;
using OceanAnomaly.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptable : ScriptableObject
{
	[Header("Individual Variables")]
	[ReadOnly]
	public SerializableGuid uuid;
	public string Name;
	public string Description;
	public Sprite DisplayIcon;
	[Header("Relational Variables")]
	public bool IsUnique = false;
	public List<UpgradeScriptable> Prerequisites;
	public UpgradeScriptable()
	{
		uuid = Guid.NewGuid();
	}

}
