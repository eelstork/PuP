using System.Collections.Generic; using System.Linq;
using System.IO; using System.Text;
using Ex = System.Exception;
using UPMClient = UnityEditor.PackageManager.Client;
using static UnityEngine.Debug;


namespace Activ.PuP{

// REF: https://docs.unity3d.com/Manual/upm-api.html
// NOTE - Until (perhaps not all versions of) Unity 2020, no bulk
// add/remove, no API for updating, ...
public static class UPMClientMethods{

    const char Qt = '"';
    const string ManifestPath = "Packages/manifest.json";

    public static void AddAndRemove((string name, string source)[] adding,
                                    string[] removing){
        #if (UNITY_2018 || UNITY_2019)
            AddAndRemove2018(adding, removing);
        #else
            UPMClient.AddAndRemove(dep);
        #endif
    }

    public static void UpdateAllPackages(){
        System.IO.File.Delete("Packages/packages-lock.json");
        ForceRefresh();
    }

    // TODO needs testing
    static void AddAndRemove2018((string name, string source)[] adding,
                                  string[] removing){
        string[] manifest = File.ReadAllLines(ManifestPath);
        var addList = (from x in adding
                       where !manifest.ContainsEntryWithSubstring(x.source)
                       select x).ToList();
        var remList = (from x in removing
                       where manifest.ContainsEntryWithSubstring(x)
                       select x).ToList();
        if(addList.Count + remList.Count == 1){
            if(addList.Count == 1)
                UPMClient.Add(addList[0].source);
            if(remList.Count == 1)
                UPMClient.Remove(remList[0]);
        }else{
            // Add new packages at the top
            int i = 2;
            var outlines = manifest.ToList();
            foreach(var pkg in adding){
                outlines.Insert(i++, AsJSON(pkg.name, pkg.source));
            }
            // Write lines, filtering removable entries
            StringBuilder @out = new StringBuilder();
            foreach (string line in outlines){
                if(MatchesAny(line, removing)) continue;
                @out.Append(line + "\n");
            }
            File.WriteAllText(ManifestPath, @out.ToString());
            ForceRefresh();
        }
    }

    static string AsJSON(string name, string source)
    => $"    {Qt}{name}{Qt}: {Qt}{source}{Qt},";

    static void ForceRefresh(string message=null){
        #if UNITY_EDITOR_WIN
            WinUser.Refocus();
        #else
            Log(message ?? "PuP: KINDLY CLICK OUTSIDE THE EDITOR WINDOW TO APPLY CHANGES");
        #endif
    }

    static bool MatchesAny(string line, IEnumerable<string> arr){
        foreach(var e in arr){
            if(line.Contains(e)) return true;
        }
        return false;
    }

}}
