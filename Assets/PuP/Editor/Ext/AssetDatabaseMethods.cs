using System.IO;
using UnityEngine; using UnityEditor;
using ADB = UnityEditor.AssetDatabase;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class AssetDatabaseMethods{

    // Ref: https://tinyurl.com/yahntp3a this and a better solution
    #if UNITY_2018_1_OR_NEWER
    const string inspector = "Window/General/Inspector";
    #else
    const string inspector = "Window/Inspector";
    #endif

    public static void Edit<T>(string path) where T : ScriptableObject{
        ADB.OpenAsset(Req<T>(path));
        // NOTE: ensure inspector open and visible
        EditorApplication.ExecuteMenuItem(inspector);
    }

    public static T Req<T>(string path) where T : ScriptableObject
    => Load<T>(path) ?? Create<T>(path);

    static T Load<T>(string path) where T : Object
    => ADB.LoadAssetAtPath<T>(FullPath(path));

    static T Create<T>(string path) where T : ScriptableObject{
        path = FullPath(path);
        var so = ScriptableObject.CreateInstance<T>();
        var directory = Path.GetDirectoryName(path);
        Directory.CreateDirectory(directory);
        ADB.CreateAsset(so, path);
        return so;
    }

    static string FullPath(string path)
    => $"Assets/{path}.asset";

}}
