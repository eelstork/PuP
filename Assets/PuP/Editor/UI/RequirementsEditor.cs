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
    bool adding = false;
    string inputURL;

    public override void OnInspectorGUI(){
        requirements = (Requirements)target;
        //Undo.RecordObject(requirements, $"Modified {requirements.name}");
        if(!Manager.CanEdit(requirements)){
            EGL.LabelField($"Claim the 'admin' role in PuP config to edit.");
            return;
        }
        EditorUtility.SetDirty(requirements);
        if(EditorBusy(out string doing, out bool canStop)){
            EGL.LabelField($"Editor is {doing}...");
            if(canStop && GL.Button("Stop", GL.Width(60))){
                Manager.StopProcessing();
            }
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
        //
        // FOOTER
        DrawFooter(refresh);
    }

    void DrawFooter(bool refresh){
        GL.BeginHorizontal();
        if(adding){
            if(GL.Button("▼ Add package", GL.Width(100)))
                adding = false;
        }else if(GL.Button("▶ Add package", GL.Width(100))){
            adding = true;
            Manager.FindLocalPackages();
        }
        if(refresh || (!adding && GL.Button("Apply all"))){
            Manager.ApplyDeps();
            return;
        }
        GL.EndHorizontal();
        //
        EGL.Space(8);
        if(adding) PresentAddPackageUI();
    }

    void DrawDependencyUI(Dependency arg,
                          ref Dependency delete,
                          ref bool refresh){
        EditorGUIUtility.labelWidth = 78;
        EGL.LabelField("_____________");
        EGL.Space(8);
        arg.name = EGL.TextField(arg.name);
        arg.file = EGL.TextField("File", arg.file);
        arg.gitURL = EGL.TextField("Git URL", arg.gitURL);
        arg.teamRoles = EGL.TextField("Team Roles", arg.teamRoles);
        arg.skipUpdates = !EGL.Toggle("auto-update", !arg.skipUpdates);
        arg.runTests  = EGL.Toggle("run tests", arg.runTests);
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
        if(arg.runTests){
            LogWarning("Enabling 'run tests' is not supported yet");
            arg.runTests = false;
        }
        GL.FlexibleSpace();
        if(GL.Button("x", GL.MaxWidth(16f))){
            delete = arg;
        }
        EGL.EndHorizontal();
        if(arg.log != null && arg.log.Length > 0){
            Vector2? scroll = null;
            UpdateLogDrawer.DrawLog(arg.log, ref scroll);
        }
        EditorGUIUtility.labelWidth = 0;
    }

    void PresentAddPackageUI(){
        EditorGUIUtility.labelWidth = 70;
        PresentAddViaGitURLUI();      // via git URL
        PresentAddLocalPackageUI();   // local packages
        PresentAddCommonPackageUI();  // internal reg.
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

    static bool EditorBusy(out string doing, out bool canStop){
        doing = null;
        canStop = false;
        var queue = ProcessingQueue.ι;
        if(queue?.hasPendingJobs ?? false){
            doing = $"processing {queue.pendingJobsCount} package(s)";
            if(queue.stopping){
                doing += $" ({queue.statusString} - stopping)";
            }else{
                doing += $" ({queue.statusString})";
            }
            canStop = true;
        }
        if(!manifestExists) doing = "awaiting signal; click outside the editor window";
        if(Ed.isCompiling) doing = "compiling";
        if(Ed.isPlaying)   doing = "playing";
        if(Ed.isPaused)    doing = "paused";
        if(Ed.isPaused)    doing = "updating";
        return doing != null;
    }

    static bool manifestExists
        => System.IO.File.Exists("Packages/manifest.json");

}}
