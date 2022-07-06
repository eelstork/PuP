using UnityEngine;
using UnityEditor;
using System.IO;

namespace Activ.Packaging{
public class PuP : MonoBehaviour
{

    [MenuItem("Window/Activ/Pup/Freeze requirements")]
    static void Freeze(){
        new FreezeAction().Apply(verbose: true);
    }

    [MenuItem("Window/Activ/Pup/Update requirements")]
    static void Apply(){
        Debug.Log("Load requirements from requirements.txt");
        new LoadAction().Apply();
    }

    [MenuItem("Window/Activ/Pup/Restore Manifest")]
    static void Restore(){
        File.Delete("Packages/manifest.json");
        File.Copy("Packages/manifest.json.backup",
                  "Packages/manifest.json");
    }

}}
