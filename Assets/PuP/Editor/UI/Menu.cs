using UnityEditor;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class Menu{

    [MenuItem("Window/Activ/PuP/Apply dependencies")]
    public static void ApplyDependencies()
    => Manager.ApplyDeps();

    [MenuItem("Window/Activ/PuP/Update packages")]
    public static void UpdatePackages()
    => Manager.UpdatePackages();

    [MenuItem("Window/Activ/PuP/Edit local dependencies")]
    public static void EditLocalDependencies()
    => Manager.EditLocalDeps();

    [MenuItem("Window/Activ/PuP/Edit project dependencies")]
    public static void EditProjectDependencies()
    => Manager.EditProjectDeps();

    [MenuItem("Window/Activ/PuP/Remove all dependencies")]
    public static void RemoveDependencies()
    => Manager.RemoveAllDeps();

    [MenuItem("Window/Activ/PuP/Scan (find local packages)")]
    public static void FindLocalPackages()
    => Manager.FindLocalPackages();

    [MenuItem("Window/Activ/PuP/Online help")]
    public static void DisplayOnlineHelp()
    => Log("Display online help");

    [MenuItem("Window/Activ/PuP/Config")]
    static void Init(){
        PuPConfigWindow.ShowWindow();
    }

}}
