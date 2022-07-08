using UnityEditor;
using UnityEngine;
using Ed = UnityEditor.EditorApplication;
using GL = UnityEngine.GUILayout;
using EGL = UnityEditor.EditorGUILayout;
using static UnityEngine.Debug;

namespace Activ.PuP{
[CustomEditor(typeof(Requirements), true)]
public class RequirementsEd : Editor {

    Requirements requirements;
    string inputURL;

    public override void OnInspectorGUI(){
        if(EditorBusy(out string doing)){
            EGL.LabelField($"Editor is {doing}...");
            return;
        }
        requirements = (Requirements)target;
        Dependency del = null;
        foreach(var e in requirements.dependencies){
            if(!e.isExcluded)
                Draw(e, out del);
        }
        if(del != null){
            del.isExcluded = true;
            Manager.ApplyDeps();
        }
        EGL.LabelField("_____________");
        EGL.Space(8);
        PresentAddPackageUI();
    }

    void Draw(Dependency arg, out Dependency delete){
        EditorGUIUtility.labelWidth = 70;
        EGL.LabelField("_____________");
        EGL.Space(8);
        arg.name = EGL.TextField(arg.name);
        arg.file = EGL.TextField("File", arg.file);
        arg.gitURL = EGL.TextField("Git URL", arg.gitURL);
        EGL.BeginHorizontal();
        EGL.EnumPopup(arg.resolution);
        arg.runTests = EGL.Toggle("run tests", arg.runTests);
        GL.FlexibleSpace();
        delete = GL.Button("x", GL.MaxWidth(16f)) ? arg : null;
        EGL.EndHorizontal();
        EditorGUIUtility.labelWidth = 0;
    }

    void PresentAddPackageUI(){
        EditorGUIUtility.labelWidth = 60;
        EGL.LabelField("Add package...");
        // Add via internal registry
        PresentAddCommonPackageUI();
        // Add local package (via crawler)
        PresentAddLocalPackageUI();
        // Add via URL
        PresentAddViaGitURLUI();
        //
        EditorGUIUtility.labelWidth = 0;
    }

    void PresentAddViaGitURLUI(){
        EGL.BeginHorizontal();
        inputURL = EGL.TextField("Git URL", inputURL);
        if(GL.Button(" + ", GL.MaxWidth(40f))) AddViaURL(inputURL);
        EGL.EndHorizontal();
    }

    void PresentAddCommonPackageUI(){
        int sel = 0;
        sel = EGL.Popup("Common", sel, InternalRegistry.avail);
        if(sel > 0){
            InternalRegistry.AddPackage(sel, to: requirements);
            Manager.ApplyDeps();
        }
    }

    void PresentAddLocalPackageUI(){
        int sel = 0;
        sel = EGL.Popup("Local", sel, localPackages);
        if(sel > 0) AddLocalPackage(sel-1);
    }

    void AddLocalPackage(int index){
        var @ref = Manager.GetLocalPackage(index);
        Log($"PuP: Adding {@ref}");
        requirements.dependencies.Add(new Dependency(){
            name = @ref.name,
            //displayName = @ref.displayName,
            file = @ref.path
        });
        Manager.ApplyDeps();
    }

    void AddViaURL(string url){
        requirements.dependencies.Add(new Dependency(){
            gitURL = url
        });
        Manager.ApplyDeps();
    }

    // TODO we don't want to regenerate the array often
    string[] localPackages{ get{
        var pkgs = Manager.localPackages;
        var choices = new string[pkgs.Count + 1];
        choices[0] = "...";
        for(int i = 0; i < pkgs.Count; i++){
            // NOTE: backslashes lest popup sees a nested structure
            choices[i + 1] = pkgs[i].ToString().Replace('/', '\\');
        }
        return choices;
    }}

    static bool EditorBusy(out string doing){
        doing = null;
        if(UPMClientMethods2.hasPendingJobs) doing = $"processing {UPMClientMethods2.pendingJobsCount} package(s)";
        if(Ed.isCompiling) doing = "compiling";
        if(Ed.isPlaying)   doing = "playing";
        if(Ed.isPaused)    doing = "paused";
        if(Ed.isPaused)    doing = "updating";
        return doing != null;
    }

}}
