using System.Collections.Generic;
using System.IO;
using UnauthorizedEx = System.UnauthorizedAccessException;
using static UnityEngine.Debug;
using Dir = System.IO.DirectoryInfo;
using URI = System.Uri;

namespace Activ.PuP{
public static class Crawler{

    public static void FindPackages(string root,
                                    List<PackageRef> refs,
                                    int maxDepth){
        var dir = new Dir(root);
        var exclusion = new Dir(".").FullName;
        FindPackages(dir, exclusion, refs, maxDepth);
    }

    static void FindPackages(Dir root, string exclusion,
                             List<PackageRef> refs,
                             int maxDepth){
        if(IsSameDir(root, exclusion)){
            return;  // no search current proj
        }
        try{
            var files = root.GetFiles("package.json");
            if(files.Length == 1) TryAdd(files[0].FullName, refs);
            if(maxDepth <= 0) return;
            foreach(var child in root.GetDirectories()){
                FindPackages(child, exclusion, refs, maxDepth - 1);
            }
        }catch(UnauthorizedEx){}
    }

    static bool IsSameDir(Dir arg, string path){
        var x = Path.Combine(arg.FullName, "_");
        var y = Path.Combine(path, "_");
        return new URI(x) == new URI(y);
    }

    static void TryAdd(string path, List<PackageRef> refs){
       var hintsPkg = false;
       string pkgName = null, pkgDisplayName = null;
       foreach (string line in File.ReadAllLines(path))
       {
           if(line.Contains("unity")){
               hintsPkg = true;
           }
           if(pkgName == null && line.Contains("name")){
               pkgName = UnreliablyParsePropValue(line);
           }
           if(line.Contains("displayName")){
               pkgDisplayName = UnreliablyParsePropValue(line);
           }
           if(GotPkgInfo(pkgName, pkgDisplayName, hintsPkg)){
               refs.Add( new PackageRef(
                   pkgName,
                   pkgDisplayName,
                   Path.GetDirectoryName(path).Replace('\\', '/')
               ));
               return;
           }
       }
    }

    static bool GotPkgInfo(string arg0, string arg1, bool hint)
    => hint && arg0 != null && arg1 != null;

    static string UnreliablyParsePropValue(string line){
        var i = line.LastIndexOf(':');
        if(i < 0) return null;
        var str = line.Substring(i + 1);
        str = str.Replace("\"", null);
        str = str.Replace(",", null);
        return str.Trim();
    }

}}
