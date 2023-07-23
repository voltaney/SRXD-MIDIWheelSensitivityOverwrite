using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;

namespace MIDIWheelSensitivityOverwrite
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Patch), PluginInfo.PLUGIN_GUID);
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    public class Patch
    {
        [HarmonyPatch(typeof(MidiSerializableControl), "Deserialise")]
        [HarmonyPrefix]
        private static void Prefix(MidiSerializableControl __instance)
        {
            var configFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"{PluginInfo.PLUGIN_NAME}.cfg"), true);
            var configWheelNoteNumber = configFile.Bind("General",
                "MIDIWheelNoteNumber",
                17,
                "MIDIホイールのNote番号");
            var configWheelSensitivity = configFile.Bind("General",
                "MIDIWheelSensitivity",
                -110,
                "MIDIホイールの感度。ゲーム設定画面の感度-10000は-100に該当するので、それを参考に。");
            if (__instance.noteNumber == configWheelNoteNumber.Value)
            {
                __instance.sensitivity = configWheelSensitivity.Value;
            }
        }
    }
}
