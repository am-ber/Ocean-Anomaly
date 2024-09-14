using Newtonsoft.Json;
using System;

public interface SaveData
{
	[JsonIgnore]
	public bool IsNew { get; set; }
}