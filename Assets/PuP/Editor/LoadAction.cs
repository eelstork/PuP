using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using Active.Core; using static Active.Status;

namespace Activ.Packaging{
[InitializeOnLoad]
public class LoadAction{

    const string Path       = "Packages/requirements.txt";
    const string ResumeFile = "../PuP.LoadAction";

    PackageCollection library;
    ListRequest       listRequest;
    List<Requirement> requirements;

    static LoadAction(){
        if(File.Exists(ResumeFile)){
            new LoadAction().Apply();
        }
    }

    public void Apply() => EditorApplication.update += Update;

    public void Update(){
        status op = LoadLibrary() && LoadRequirements()
                                   && AddPackages();
        if(!op.running)
            EditorApplication.update -= Update;
    }

    status LoadLibrary(){
        if(library != null) return true;
        if(listRequest != null){
            if(!listRequest.IsCompleted) return cont();
            if(listRequest.Status == StatusCode.Success){
                library = listRequest.Result;
                return done();
            }else{
                return fail();
            }
        }else{
            listRequest = Client.List(true);
            return cont();
        }
    }

    action LoadRequirements(){
        if(requirements != null) return @void();
        requirements = new List<Requirement>();
        using (var reader = new StreamReader(Path)){
            string line;
            while((line = reader.ReadLine()) != null){
                if(line.Length ==  0 ) continue;
                if(line[0]     == '#') continue;
                int i = line.IndexOf("==");
                var package = line.Substring(0, i);
                var version = line.Substring(i + 2);
                if(AlreadySatisfied(package, version)) continue;
                requirements.Add(new Requirement(package, version));
            }
        }
        if(requirements.Count > 0){
            File.WriteAllText(ResumeFile, "RESUME");
        }else{
            File.Delete(ResumeFile);
            Log("Requirements up to date");
        }
        return @void();
    }

    status AddPackages(){
        if(requirements.Count == 0) return done();
        var req = requirements[0];
        status loaded = req.Load();
        if(loaded.running) return cont();
        if(loaded.failing){
            Log($"Failed to load {req.package}");
        }
        requirements.RemoveAt(0);
        return cont();
    }

    bool AlreadySatisfied(string package, string version){
        foreach(var e in library){
            if(e.name == package) return true;
        }
        return false;
    }

    void Log(string arg) => Debug.Log(arg);

}}
