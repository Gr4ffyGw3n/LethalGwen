using System;
using UnityEngine;
using UnityEditor;

public class AssetBundleBuiilder
{
    [MenuItem("Assets/Create Asset Bundle")]
     static void PerformAssetBundleBuild()
    {
        string dirPath = Application.dataPath + "/../AssetBundles/";
        try { 
        BuildPipeline.BuildAssetBundles(dirPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }
}
