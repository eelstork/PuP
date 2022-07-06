using UnityEngine;

public static class ArrayExt{

    public static bool ContainsEntryWithSubstring(
                                     this string[] self, string arg){
        foreach(var e in self){
            if(e == null){
                Debug.LogWarning("NULL ENTRY IN FILE???");
                continue;
            }else{
                // Debug.LogWarning($"LN {e}");
            }
            if(e.Contains(arg)) return true;
        }
        return false;
    }

}
