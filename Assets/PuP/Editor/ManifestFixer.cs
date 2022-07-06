using System.IO;
using UnityEngine;
using UnityEditor;

namespace Activ.Packaging{
[InitializeOnLoad] public class ManifestFixer{

    static ManifestFixer(){
        string text = File.ReadAllText("Packages/manifest.json");
        if(text.Contains("com.unity.cinemachine")) return;
        Debug.Log("Restore manifest.json from backup");
        File.Delete("Packages/manifest.json");
        File.Copy("Packages/manifest.json.backup",
                  "Packages/manifest.json");
    }

}}
