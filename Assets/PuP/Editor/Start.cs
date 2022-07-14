using UnityEngine; using UnityEditor;
using static UnityEngine.Debug;

namespace Activ.PuP{
[InitializeOnLoad] public class Start{

    const string SessionKey = "Activ.PuP.SessionStart";

    static Start(){
        if(SessionState.GetBool(SessionKey, false))
            return;
        SessionState.SetBool(SessionKey, true);
        if(Config.updateOnStart){
            Log("PuP: updating packages...");
            Manager.ApplyDeps();
        }
    }

}}
