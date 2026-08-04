//
// Copyright (c) 2026 7Bpencil
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
//

using BepInEx;
using BepInEx.Configuration;
using EFT.UI;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SevenBoldPencil.ChangeVersionLabel
{
    [BepInPlugin("7Bpencil.ChangeVersionLabel", "7Bpencil.ChangeVersionLabel", "1.0.0")]
    public class Plugin : BaseUnityPlugin
	{
		public static ConfigEntry<string> VersionLabelValue;
		public static LocalizedText VersionLabel;

        private void Awake()
		{
			VersionLabelValue = Config.Bind<string>("Main", "Version Label Value", "Hello World!");
            VersionLabelValue.SettingChanged += (_, _) =>
			{
	            if (VersionLabel)
	            {
					VersionLabel.UpdateLocale();
	            }
			};

			new Patch_LocalizedText_UpdateLocale().Enable();
        }
    }

    public class Patch_LocalizedText_UpdateLocale : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocalizedText), nameof(LocalizedText.UpdateLocale));
        }

        [PatchPrefix]
        public static bool Prefix(LocalizedText __instance)
        {
			if (__instance == Plugin.VersionLabel)
			{
				__instance.SetLabelText(Plugin.VersionLabelValue.Value);
				return false;
			}
			if (!Plugin.VersionLabel && __instance.gameObject.name == "AlphaLabel")
			{
				Plugin.VersionLabel = __instance;
				__instance.SetLabelText(Plugin.VersionLabelValue.Value);
				return false;
			}

			return true;
        }
    }
}
