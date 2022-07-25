using System.Collections.Generic;
using System.Linq;
using InvOp = System.InvalidOperationException;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using static UnityEngine.Debug;

namespace Activ.PuP{
public class ProcessingQueue{

    public static ProcessingQueue ι;
    float startTime;
    float requestStartTime;
    int depCount;
    List<Dependency> plist;
    List<string> remList;
    Request request;
    public bool stopping{ get; private set; }
    public string statusString {get; private set; }

    public ProcessingQueue(){
        ι = this; startTime = UnityEngine.Time.time;
    }

    public void StopProcessing(){ plist = null; stopping = true; }

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
        // TODO - skip updates should not prevent
        // the agent from removing a package... or adding a missing
        // package
        //while(plist.Count > 0 && plist[0].skipUpdates){
        //    plist.RemoveAt(0);
        //}
        if(plist.Count == 0){
            return false;
        }
        var dep = plist[0];
        if(dep.isRequired){
            var src = Preprocessor.UpdateSource(dep, out string message);
            if(src != null)
            {
                statusString = $"adding {dep.name}";
                StartRequest(Client.Add(src));
            }else{
                //LogWarning($"Skipping processing {dep}: {message}");
                plist.RemoveAt(0);
                return Continue();
            }
        }else if(dep.isExcluded){
            statusString = $"removing {dep.name}";
            StartRequest(Client.Remove(dep.name));
        }
        plist.RemoveAt(0);
        return true;
    }

    void StartRequest(Request arg){
        request = arg;
        requestStartTime = UnityEngine.Time.time;
        EditorApplication.update += OnProgressUpdate;
    }

    void OnProgressUpdate(){
        if(!request.IsCompleted) return;
        float δ = UnityEngine.Time.time - requestStartTime;
        switch(request){
            case AddRequest add:
                OnAddReqComplete(add, δ);
                break;
            case RemoveRequest rem:
                OnRemReqComplete(rem, δ);
                break;
        }
        EditorApplication.update -= OnProgressUpdate;
        request = null;
        Continue();
    }

    bool End(){
        plist = null;
        var t = UnityEngine.Time.time;
        var δ = t - startTime;
        Log($"PuP: Processed {depCount} package(s) in {δ:0.0}s");
        return true;
    }

    void OnAddReqComplete(AddRequest addRequest, float δ){
        if (addRequest.Status == StatusCode.Success){
            Log($"PuP: Added {addRequest.Result.packageId} in {δ:0.0}s");
        }else if (addRequest.Status >= StatusCode.Failure){
            LogWarning($"PuP: " + addRequest.Error.message);
        }
    }

    void OnRemReqComplete(RemoveRequest removeRequest, float δ){
        if (removeRequest.Status == StatusCode.Success){
            Log($"PuP: Removed {removeRequest.PackageIdOrName} in {δ:0.0}s");
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
