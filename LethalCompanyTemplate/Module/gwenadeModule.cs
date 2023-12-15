using BepInEx.Logging;
using LethalLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LethalLib.Modules.Enemies;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using BepInEx.Bootstrap;
using static LethalLib.Modules.Items;
using System.Collections;
using HarmonyLib.Tools;
using LethalLib.Modules;
using static LethalLib.Modules.Levels;
using Unity.Netcode;


namespace GwenMod.Module
{
    public class gwenadeModule
    {
        private static Item gwenade_Item;
        private static gwenData gwenade_Data;
        private static GwenScript spawnedGwenade;
        private static GameObject gwenade_explosion;
        private static List<AudioClip> plush_clips = new List<AudioClip>();
        public static void Init(AssetBundle MainAssets)
        {
            // Basic Init
            gwenade_Item = MainAssets.LoadAsset<Item>("assets/gwenade/gwenadeitem.asset");
            gwenade_Data = MainAssets.LoadAsset<gwenData>("assets/gwenade/testdata.asset");

            Plugin.logger.LogInfo("Gwenade Item Loaded");
            gwenade_Item.spawnPrefab.AddComponent<NetworkObject>();
            var netObj = gwenade_Item.spawnPrefab.GetComponent<NetworkObject>();
            netObj.AutoObjectParentSync = false;
            Plugin.logger.LogInfo(gwenade_Item);

            // Script Init
            spawnedGwenade = gwenade_Item.spawnPrefab.GetComponent<GwenScript>();
            Plugin.logger.LogInfo(spawnedGwenade.itemVerticalFallCurveNoBounce);
            spawnedGwenade.itemProperties = gwenade_Item;
            Plugin.logger.LogInfo(spawnedGwenade.itemProperties.itemName);


            // Explosion Insertion
            Plugin.logger.LogInfo("Explosion Insert");
            gwenade_explosion = MainAssets.LoadAsset<GameObject>("assets/gwenade/assets/prefab/gwenadeexplosion.prefab");
            spawnedGwenade.gwenadeExplosion = gwenade_explosion;

            //Item Register
            Items.RegisterScrap(gwenade_Item, 510, LevelTypes.All);
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(gwenade_Item.spawnPrefab);
            Plugin.logger.LogInfo($"Gwenade Init Success!");
        }

    }

}
