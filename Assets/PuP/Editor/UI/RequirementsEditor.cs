using UnityEditor;
using UnityEngine;
using Ed = UnityEditor.EditorApplication;
using GL = UnityEngine.GUILayout;
using EGL = UnityEditor.EditorGUILayout;
using static UnityEngine.Debug;
using Model = Activ.PuP.RequirementsModel;

namespace Activ.PuP{
[CustomEditor(typeof(Requirements), true)]
public class RequirementsEd : Editor {

    Requirements requirements;
    string inputURL;

    public override void OnInspectorGUI(){
        requirements = (Requirements)target;
        //Undo.RecordObject(requirements, $"Modified {requirements.name}");
        if(!Manager.CanEdit(requirements)){
            EGL.LabelField($"Claim the 'admin' role in PuP config to edit.");
            return;
        }
        EditorUtility.SetDirty(requirements);
        if(EditorBusy(out string doing)){
            EGL.LabelField($"Editor is {doing}...");
            return;
        }
        Dependency del = null;
        bool refresh = false;
        foreach(var e in requirements.dependencies){
            DrawDependencyUI(e, ref del, ref refresh);
        }
        // Remove from requirements if [x] was selected
        if(del != null){
            Model.Remove(del, from: requirements);
        }
        EGL.Space(8);
        if(refresh || GL.Button("Apply all")){
            Manager.ApplyDeps();
        }
        EGL.Space(8);
        PresentAddPackageUI();
    }

    void DrawDependencyUI(Dependency arg,
                          ref Dependency delete,
                          ref bool refresh){
        EditorGUIUtility.labelWidth = 70;
        EGL.LabelField("_____________");
        EGL.Space(8);
        arg.name = EGL.TextField(arg.name);
        arg.file = EGL.TextField("File", arg.file);
        arg.gitURL = EGL.TextField("Git URL", arg.gitURL);
        arg.teamRoles = EGL.TextField("Team Roles", arg.teamRoles);
        // BOTTOM ROW
        EGL.BeginHorizontal();
        //
        var newRes =
            (Dependency.Resolution) EGL.EnumPopup(arg.resolution);
        if(newRes != arg.resolution){
            arg.resolution = newRes;
            refresh = true;
        }
        //
        arg.runTests = EGL.Toggle("run tests", arg.runTests);
        if(arg.runTests){
            LogWarning("Enabling 'run tests' is not supported yet");
            arg.runTests = false;
        }
        GL.FlexibleSpace();
        if(GL.Button("x", GL.MaxWidth(16f))){
            delete = arg;
        }
        EGL.EndHorizontal();
        EditorGUIUtility.labelWidth = 0;
    }

    void PresentAddPackageUI(){
        EditorGUIUtility.labelWidth = 70;
        EGL.LabelField("Add package...");
        // Add via URL
        PresentAddViaGitURLUI();
        // Add local package (via crawler)
        PresentAddLocalPackageUI();
        // Add via internal registry
        PresentAddCommonPackageUI();
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
        int i = EGL.Popup("Tea's picks", 0, InternalRegistry.avail);
        Model.AddCommonPackage(i, to: requirements);
    }

    void PresentAddLocalPackageUI(){
        if(Config.enableScan){
            var sel = EGL.Popup("Local", 0, localPackages);
            Model.AddLocalPackage(sel - 1, to: requirements);
        }else{
            EGL.LabelField("(Enable 'scan' in PuP config for local packages)");
        }
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
        if(UPMAgent.ι?.hasPendingJobs ?? false){
            doing = $"processing {UPMAgent.ι.pendingJobsCount} package(s)";
        }
        if(Ed.isCompiling) doing = "compiling";
        if(Ed.isPlaying)   doing = "playing";
        if(Ed.isPaused)    doing = "paused";
        if(Ed.isPaused)    doing = "updating";
        return doing != null;
    }

}}
