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

    public override void OnInspectorGUI(){
        requirements = (Requirements)target;
        Dependency del = null;
        foreach(var e in requirements.dependencies){
            Draw(e, out del);
        }
        if(del != null){
            requirements.dependencies.Remove(del);
        }
        EGL.LabelField("_____________");
        EGL.Space(8);
        PresentAddLocalPackageUI();
    }

    void Draw(Dependency arg, out Dependency delete){
        EditorGUIUtility.labelWidth = 60;
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

    void PresentAddLocalPackageUI(){
        int sel = 0;
        sel = EGL.Popup("Add local package", sel, localPackages);
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

    string[] localPackages{get{
        var pkgs = Manager.localPackages;
        var choices = new string[pkgs.Count + 1];
        choices[0] = "NONE";
        for(int i = 0; i < pkgs.Count; i++){
            // NOTE: backslashes lest popup sees a nested structure
            choices[i + 1] = pkgs[i].ToString().Replace('/', '\\');
        }
        return choices;
    }}

}}
