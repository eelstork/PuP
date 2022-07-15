using System.Collections.Generic;
using UnityEngine;
using ADB = Activ.PuP.AssetDatabaseMethods;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class Manager{

    public const string ProjectRequirements = "Project Dependencies";
    public const string PersonalRequirements = "Personal Packages";
    public const string ProjectRequirementsPath = "Project Dependencies";
    public const string PersonalRequirementsPath = "Local/Personal Packages";

    static List<PackageRef> _localPackages;

    public static void ApplyDeps(){
        var common       = ADB.Req<Requirements>(ProjectRequirementsPath);
        var local        = ADB.Req<Requirements>(PersonalRequirementsPath);
        var requirements = common + local;
        Resolver.Apply(requirements.dependencies);
    }

    public static bool CanEdit(Requirements arg){
        var name = arg.name;
        if(name != ProjectRequirements) return true;
        if(Config.roles.Contains("admin")) return true;
        return false;
    }

    public static void EditLocalDeps()
    => ADB.Edit<Requirements>(PersonalRequirementsPath);

    public static void EditProjectDeps()
    => ADB.Edit<Requirements>(ProjectRequirementsPath);

    public static void FindLocalPackages(){
        var depth = Config.scanDepth;
        List<PackageRef> @out = new List<PackageRef>();
        foreach(var path in Config.scanRootsArray){
            Crawler.FindPackages(path, @out, depth);
        }
        @out.Sort();
        _localPackages = @out;
    }

    public static void RemoveAllDeps(){
        Log("PuP: Sorry but this isn't implemented yet!");
    }

    // NOTE - I don't think this is useful. A 'rebuild' option
    // might still be useful though
    public static void UpdatePackages(){
        System.IO.File.Delete("Packages/packages-lock.json");
        ApplyDeps();
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
