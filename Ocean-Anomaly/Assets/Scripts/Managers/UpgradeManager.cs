using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanAnomaly.Managers
{
	public static class UpgradeManager
	{
		// Movement specific
		public static float dashTime = 1;			// smaller for better
		public static float moveFactor = 1;			// larger for better
		// Attack specific
		public static float projectileSpeed = 1;	// larger for better
		public static float attackTime = 1;			// smaller for better
		public static float projectileAccuracy = 1; // smaller for better

		public static List<UpgradeScriptable> playerUpgrades = new List<UpgradeScriptable>();
	}
}
