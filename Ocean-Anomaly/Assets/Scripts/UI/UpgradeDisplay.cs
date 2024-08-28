using OceanAnomaly.Components;
using OceanAnomaly.Tools;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplay : MonoBehaviour
{
	public UpgradeScriptable Upgrade;
	public Button UpgradeButton;
	public TextMeshProUGUI UpgradeNameText;
	public TextMeshProUGUI UpgradeDescriptionText;
	private void OnValidate()
	{
		UpdateDisplayElements();
	}
	void Start()
	{
		UpdateDisplayElements();
	}
	private void ResetDisplayElements()
	{
		// Null checks
		if (!UpgradeNameText || !UpgradeDescriptionText || !UpgradeButton)
		{
			return;
		}
		// Icon setting
		UpgradeButton.image.sprite = null;
		UpgradeButton.image.transform.position = new Vector3();
		UpgradeButton.image.transform.Rotate(new Vector3(0, 0, 0));
		// Text setting
		UpgradeNameText.text = "Upgrade Name";
		UpgradeDescriptionText.text = "Upgrade Description";
	}
	public void UpdateDisplayElements()
	{
		// Null checks
		if (!Upgrade)
		{
			ResetDisplayElements();
		}
		if (!UpgradeNameText || !UpgradeDescriptionText || !UpgradeButton)
		{
			return;
		}
		try
		{
			// Icon setting
			UpgradeButton.image.sprite = Upgrade.DisplayIcon;
			UpgradeButton.image.transform.position = Upgrade.IconOffset.ToVector3();
			UpgradeButton.image.transform.Rotate(new Vector3(0, 0, Upgrade.IconAngleOffset));
			// Text setting
			UpgradeNameText.text = Upgrade.name;
			UpgradeDescriptionText.text = Upgrade.Description;
		}
		catch { }
	}
	public void OnButtonPress()
	{
		if (!Upgrade)
		{
			Upgrade.Unlock();
		}
	}
}
