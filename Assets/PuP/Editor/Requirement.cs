using System.IO;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using Active.Core; using static Active.Status;

public class Requirement{

    public string package;
    public string version;
    AddRequest request;

    public Requirement(string package, string version){
        this.package = package; this.version = version;
    }

    public status Load(){
        if(request == null){
            Debug.Log($"Adding {package}...");
            request = Client.Add(package);
            return cont();
            
        }else{
            if(request.IsCompleted){
                if(request.Status == StatusCode.Success)
                    return done();
                else
                    return fail();
            }else
                return cont();
        }
    }

}
