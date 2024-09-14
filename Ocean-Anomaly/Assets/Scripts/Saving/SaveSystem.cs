using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

public static class SaveSystem
{
	public static string SettingsFile = "settings.json";
	public static void SaveSettings(SettingsMenu settingsMenu)
	{
		// Set file path to the settings file
		string path = CombinePathAndFile(Application.persistentDataPath, SettingsFile);
		// Bring in the settings data object
		SettingsSaveData settingsData = new SettingsSaveData(settingsMenu);
		SerializeToFile(path, settingsData);
		Debug.Log($"Saved settings to: {path}");
	}
	public static SettingsSaveData LoadSettings()
	{
		// Set file path to the settings file
		string path = CombinePathAndFile(Application.persistentDataPath, SettingsFile);
		// Create a default SettingsSaveData
		SettingsSaveData data = DeserializeFromFile<SettingsSaveData>(path);
		Debug.Log($"Loaded settings from: {path}");
		return data;
	}
	public static string CombinePathAndFile(string path, string fileName)
	{
		return Path.Combine(Path.GetFullPath(path), fileName);
	}
	public static void SerializeToFile<T>(string path, T data) where T : SaveData
	{
		try
		{
			// Creates the directory for the path if it doesn't exist
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			// Data we need to serialize and store
			string fileData = JsonConvert.SerializeObject(data, Formatting.Indented);
			// Write to the new json file that will be made
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.Write(fileData);
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"Error with Serializing to file {path} : {e.Message}\n{e.StackTrace}");
		}
	}
	public static T DeserializeFromFile<T>(string path) where T : SaveData
	{
		// Dynamically create a new instance of a SaveData object
		T loadedData = (T)Activator.CreateInstance(typeof(T), new object[] {});
		// Check for file existence
		if (File.Exists(path))
		{
			try
			{
				// Data we need to serialize and store
				string fileData = "";
				// Write to the new json file that will be made
				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						fileData = reader.ReadToEnd();
					}
				}
				// Deserialize the data from the file
				if (fileData != null || fileData != string.Empty)
				{
					T attemptedData = JsonConvert.DeserializeObject<T>(fileData);
					// Check the data for correct Json parsing
					if (attemptedData != null)
					{
						loadedData = attemptedData;
					} else
					{
						throw new FileLoadException("Couldn't JSON the file correctly.");
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Error with Serializing to file {path} : {e.Message}\n{e.StackTrace}");
			}
		}
		return loadedData;
	}
}
