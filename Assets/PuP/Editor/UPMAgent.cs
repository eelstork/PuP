using System.Collections.Generic;
using System.Linq;
using InvOp = System.InvalidOperationException;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using static UnityEngine.Debug;

namespace Activ.PuP{
public class UPMAgent{

    public static UPMAgent ι;
    float startTime;
    int depCount;
    List<Dependency> plist;
    List<string> remList;
    Request request;

    public UPMAgent(){
        ι = this;
        startTime = UnityEngine.Time.time;
    }

    public void StartProcessing(List<Dependency> deps){
        plist = deps;
        depCount = deps.Count;
        Continue();
    }

    bool Continue() => pending || ProcessNext() || End();

    bool ProcessNext(){
        if(plist == null) return false;
        if(plist.Count == 0) plist = null;
        if(plist == null) return false;
        var dep = plist[0];
        if(dep.isRequired){
            request = Client.Add(dep.source);
        }else if(dep.isExcluded){
            request = Client.Remove(dep.name);
        }
        EditorApplication.update += OnProgressUpdate;
        plist.RemoveAt(0);
        return true;
    }

    void OnProgressUpdate(){
        if(!request.IsCompleted) return;
        switch(request){
            case AddRequest add:
                OnAddReqComplete(add);
                break;
            case RemoveRequest rem:
                OnRemReqComplete(rem);
                break;
        }
        EditorApplication.update -= OnProgressUpdate;
        request = null;
        Continue();
    }

    bool End(){
        var t = UnityEngine.Time.time;
        var δ = t - startTime;
        Log($"PuP: Processed {depCount} package(s) in {δ:0.0}s");
        return true;
    }

    void OnAddReqComplete(AddRequest addRequest){
        if (addRequest.Status == StatusCode.Success){
            Log("PuP: Added " + addRequest.Result.packageId);
        }else if (addRequest.Status >= StatusCode.Failure){
            LogWarning("PuP: " + addRequest.Error.message);
        }
    }

    void OnRemReqComplete(RemoveRequest removeRequest){
        if (removeRequest.Status == StatusCode.Success){
            Log($"PuP: Removed {removeRequest.PackageIdOrName}");
        }else if (removeRequest.Status >= StatusCode.Failure){
            var msg = removeRequest.Error.message;
            if(IsIrrelevant(msg)){
                //Log("...already removed or not installed.");
            }else{
                LogWarning($"PuP: {msg}");
            }
        }
    }

    // NOTE - "Errors" indicating cannot remove a package cause not
    // available
    bool IsIrrelevant(string msg){
        if(msg == "Not Found") return true;
        if(    msg.StartsWith ("Unable to remove package")
            && msg.EndsWith   ("cannot be found in the project manifest")){
           return true;
        }
        return false;
    }

    bool pending => request != null;

    public int pendingJobsCount{ get{
        int count = 0;
        if(plist != null) count += plist.Count;
        return count + 1;
    }}

    public bool hasPendingJobs => pending || (plist != null);

}}
