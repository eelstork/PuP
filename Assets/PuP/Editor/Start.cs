using UnityEngine; using UnityEditor;
using static UnityEngine.Debug;

namespace Activ.PuP{
[InitializeOnLoad] public class Start{

    static Start(){
        if(!Config.updateOnStart){
            Log("PuP: not updating packages");
            return;
        }
        Log("PuP: updating packages...");
        Manager.ApplyDeps();
    }

}}
