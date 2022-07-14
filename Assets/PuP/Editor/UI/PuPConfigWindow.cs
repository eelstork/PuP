using UnityEngine;
using UnityEditor;
using System.Linq;
using static UnityEditor.EditorGUILayout;
using Ed = UnityEditor.EditorApplication;
using GL = UnityEngine.GUILayout;
using EGL = UnityEditor.EditorGUILayout;

namespace Activ.PuP{
public class PuPConfigWindow : EditorWindow{

    static PuPConfigWindow instance;

    void OnGUI(){
        LabelField("Claim roles for select packages");
        LabelField("(comma separated)");
        Config.roles = TextField("Team roles", Config.roles);
        LabelField("(Settings are shared across unity projects)");
        LabelField("Scan roots are used when searching local packages");
        Config.scanRoots = TextField("Paths", Config.scanRoots);
        Config.scanDepth = IntField("Max Depth", Config.scanDepth);
        Config.enableScan = Toggle("Enable scan", Config.enableScan);
        Config.updateOnStart = Toggle("Update on start", Config.updateOnStart);
    }

    public static void ShowWindow(){
        if(instance == null)
            instance = EditorWindow.GetWindow<PuPConfigWindow>(
                title: "PuP Config"
            );
        instance.Show();
    }

}}
