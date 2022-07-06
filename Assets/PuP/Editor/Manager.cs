using System.Collections.Generic;
using UnityEngine;
using ADB = Activ.PuP.AssetDatabaseMethods;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class Manager{

    static List<PackageRef> _localPackages;

    public static void ApplyDeps(){
        var common       = ADB.Req<Requirements>("dependencies");
        var local        = ADB.Req<Requirements>("Local/dependencies");
        var requirements = common + local;
        Resolver.Apply(requirements.dependencies);
    }

    public static void EditLocalDeps()
    => ADB.Edit<Requirements>("Local/dependencies");

    public static void EditProjectDeps()
    => ADB.Edit<Requirements>("dependencies");

    public static void FindLocalPackages(){
        var depth = Config.scanDepth;
        List<PackageRef> @out = new List<PackageRef>();
        foreach(var path in Config.scanRootsArray){
            Crawler.FindPackages(path, @out, depth);
        }
        _localPackages = @out;
    }

    public static void RemoveAllDeps(){
        Log("PuP: Sorry but this isn't implemented yet!");
    }

    public static void UpdatePackages(){
        System.IO.File.Delete("Packages/packages-lock.json");
        UPMClientMethods.UpdateAllPackages();
    }

    public static int PackageIndex(string name, string path)
    => localPackages.FindIndex(
        x => x.name == name && x.path == path
    );

    public static PackageRef GetLocalPackage(int index)
    => _localPackages[index];

    public static List<PackageRef> localPackages{ get{
        if(_localPackages == null && Config.enableScan){
            FindLocalPackages();
        }
        return _localPackages;
    }}

}}
