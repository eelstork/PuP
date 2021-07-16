using UnityEngine;
using UnityEditor;

namespace Activ.Packaging{
public class PuP : MonoBehaviour
{

    [MenuItem("Window/Activ/Pup/Freeze requirements")]
    static void Freeze(){
        new FreezeAction().Apply(verbose: true);
    }

    [MenuItem("Window/Activ/Pup/Apply requirements")]
    static void Apply(){
        Debug.Log("Load requirements from requirements.txt");
        Debug.Log("Ensure requirements are in UPM");
    }

}}
