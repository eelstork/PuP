using System.IO;
using UnityEngine;
using ADB = UnityEditor.AssetDatabase;

namespace Activ.PuP{
public static class AssetDatabaseMethods{

    public static void Edit<T>(string path) where T : ScriptableObject
    => ADB.OpenAsset(Req<T>(path));

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
