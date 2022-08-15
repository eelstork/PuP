using UnityEditor;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class Menu{

    [MenuItem("Window/Activ/PuP/Apply dependencies")]
    public static void ApplyDependencies()
    => Manager.ApplyDeps();

    //[MenuItem("Window/Activ/PuP/Update packages")]
    //public static void UpdatePackages()
    //=> Manager.UpdatePackages();

    [MenuItem("Window/Activ/PuP/Edit Project Dependencies")]
    public static void EditProjectDependencies()
    => Manager.EditProjectDeps();

    [MenuItem("Window/Activ/PuP/Edit Personal Packages")]
    public static void EditLocalDependencies()
    => Manager.EditLocalDeps();

    [MenuItem("Window/Activ/PuP/Delete manifest.json")]
    public static void Rebuild()
    => Manager.Rebuild();

    //[MenuItem("Window/Activ/PuP/Scan (find local packages)")]
    //public static void FindLocalPackages()
    //=> Manager.FindLocalPackages();

    //[MenuItem("Window/Activ/PuP/Online help")]
    //public static void DisplayOnlineHelp()
    //=> Log("Display online help");

    [MenuItem("Window/Activ/PuP/Config")]
    static void Init(){
        PuPConfigWindow.ShowWindow();
    }

}}
