using System.Collections.Generic;
using System.Linq;
using InvOp = System.InvalidOperationException;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class UPMClientMethods2{

    static List<(string name, string source)> addList;
    static List<string> remList;
    static AddRequest addRequest;
    static RemoveRequest removeRequest;

    public static void AddAndRemove((string name, string source)[] adding,
                                    string[] removing){
        if(hasPendingJobs) throw new InvOp("UPM client is busy");
        addList = adding.ToList();
        remList = removing.ToList();
        Continue();
    }

    static void OnAddProgressUpdate(){
        if (!addRequest.IsCompleted) return;
        if (addRequest.Status == StatusCode.Success){
            Log("PuP: Added " + addRequest.Result.packageId);
        }else if (addRequest.Status >= StatusCode.Failure){
            LogWarning("PuP: " + addRequest.Error.message);
        }
        EditorApplication.update -= OnAddProgressUpdate;
        addRequest = null;
        if(hasPendingJobs) Continue();
    }

    static void OnRemoveProgressUpdate(){
        if (!removeRequest.IsCompleted) return;
        if (removeRequest.Status == StatusCode.Success){
            Log("PuP: Request complete");
        }else if (removeRequest.Status >= StatusCode.Failure){
            var msg = removeRequest.Error.message;
            if(IsIrrelevant(msg)){
                //Log("...already removed or not installed.");
            }else{
                LogWarning($"PuP: {msg}");
            }
        }
        EditorApplication.update -= OnRemoveProgressUpdate;
        removeRequest = null;
        if(hasPendingJobs) Continue();
    }

    // NOTE - "Errors" indicating cannot remove a package cause not
    // available
    static bool IsIrrelevant(string msg){
        if(msg == "Not Found") return true;
        if(    msg.StartsWith ("Unable to remove package")
            && msg.EndsWith   ("cannot be found in the project manifest")){
           return true;
        }
        return false;
    }

    static bool Continue()
        => pending || RemoveNext() || AddNext();

    static bool RemoveNext(){
        if(remList == null) return false;
        if(remList.Count == 0) remList = null;
        if(remList == null) return false;
        var toRemove = remList[0];
        removeRequest = Client.Remove(toRemove);
        remList.RemoveAt(0);
        EditorApplication.update += OnRemoveProgressUpdate;
        //og($"Removing {toRemove}...");
        return true;
    }

    static bool AddNext(){
        if(addList == null) return false;
        if(addList.Count == 0) addList = null;
        if(addList == null) return false;
        var toAdd = addList[0];
        addRequest = Client.Add(toAdd.source);
        addList.RemoveAt(0);
        EditorApplication.update += OnAddProgressUpdate;
        //og($"Adding {toAdd.source}...");
        return true;
    }

    static bool pending
        => addRequest != null || removeRequest != null;

    public static int pendingJobsCount{ get{
        int count = 0;
        if(addList != null) count += addList.Count;
        if(remList != null) count += remList.Count;
        return count;
    }}

    public static bool hasPendingJobs
        => pending
        || (addList != null)
        || (remList != null);

}}
