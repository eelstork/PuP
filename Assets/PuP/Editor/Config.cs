using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Activ.PuP{
public static class Config{

    const string ScanRootsKey = "com.activ.pup.scan.roots";
    const string ScanDepthKey = "com.activ.pup.scan.depth";
    const string ScanKey = "com.activ.pup.scan.enable";
    const string RolesKey = "com.activ.pup.roles";

    // TODO ideally default to user home dir
    public static string scanRoots{
        get => EditorPrefs.GetString(ScanRootsKey, "C:/Users");
        set => EditorPrefs.SetString(ScanRootsKey, value);
    }

    public static int scanDepth{
        get => EditorPrefs.GetInt(ScanDepthKey, 4);
        set => EditorPrefs.SetInt(ScanDepthKey, value);
    }

    public static bool enableScan{
        get => EditorPrefs.GetBool(ScanKey, false);
        set => EditorPrefs.SetBool(ScanKey, value);
    }

    public static string roles{
        get => EditorPrefs.GetString(RolesKey, "");
        set => EditorPrefs.SetString(RolesKey, value);
    }

    public static IEnumerable<string> scanRootsArray
    => from string path in scanRoots.Split(',') select path.Trim();

}}
