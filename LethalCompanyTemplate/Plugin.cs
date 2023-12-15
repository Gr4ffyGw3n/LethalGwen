using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;
using GwenMod.Module;

namespace GwenMod
{
    public static class PluginInfo
    {
        public const string PLUGIN_ID = "GwenMod";
        public const string PLUGIN_GUID = "graffygwen.gwenmod";
        public const string PLUGIN_NAME = "Gwen Mod Supreme";
        public const string PLUGIN_VERSION = "1.0.0";
    }
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public AssetBundle MainAssets;
        public static BepInEx.Logging.ManualLogSource logger;
        //private Item gwenade_Item;
        //private GwenScript spawnedGwenade;
        public void Awake()
        {
            // Logger Setup
            logger = Logger;
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) is loaded!");

            //Asset Loading
            MainAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gwenbundle"));
            logger.LogInfo($"Main Asset Loaded");

            // Item Inits
            gwenadeModule.Init(MainAssets);
            logger.LogInfo($"Gwenade Init finished");
        }

        public static void Load(AssetBundle mainAssets)
        {
            Debug.Log("Load test");

            var test = mainAssets.LoadAllAssets<AudioClip>();


        }

    }
}
