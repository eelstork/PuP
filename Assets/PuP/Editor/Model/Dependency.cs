using System;
using System.Collections.Generic;
using System.Linq;
using Ex = System.Exception;
using User = UnityEngine.SerializeField;
using static UnityEngine.Debug;

namespace Activ.PuP{
[Serializable]
public class Dependency{

    public enum Resolution{ PreferGitURL, PreferFile, DisablePackage }

    [User] public string name, gitURL, file;
    [User] public Resolution resolution;
    [User] public bool runTests;
    [User] public string teamRoles;

    // Functions ---------------------------------------------------

    public void GetRemovableSources(List<string> removing){
        switch (resolution){
            case Resolution.PreferGitURL:
                if(IsValidSource(gitURL) && IsValidSource(file)) removing.Add(file);
                break;
            case Resolution.PreferFile:
                if(IsValidSource(gitURL) && IsValidSource(file)) removing.Add(gitURL);
                break;
            case Resolution.DisablePackage:
                if(IsValidSource(file)) removing.Add(file);
                if(IsValidSource(gitURL)) removing.Add(gitURL);
                break;
            default:
                throw new Ex($"Unknown resolution policy: {resolution}");
        }
    }

    public void Validate(){
        name   = name?.Trim();
        gitURL = gitURL?.Trim();
        file   = file?.Trim();
        teamRoles = teamRoles?.Trim();
        if(string.IsNullOrEmpty(name))
            name = NameFromURL(gitURL) ?? NameFromFile(file);
    }

    override public string ToString() => name;

    public string Format()
    => $"{name}, {gitURL}, {file}, {resolution}, run tests: {runTests}, roles: [{teamRoles}]";

    // Public properties -------------------------------------------

    public bool isExcluded{
        set{
            if(value)
                resolution = Resolution.DisablePackage;
            else if(resolution == Resolution.DisablePackage)
                resolution = Resolution.PreferGitURL;
        }
        get => resolution == Resolution.DisablePackage;
    }

    public bool isRequired{ get{
        if(resolution == Resolution.DisablePackage) return false;
        return RoleManager.Validate(teamRoles);
    }}

    public string source{ get{
        switch (resolution){
            case Resolution.PreferGitURL:
                return EvalSource(gitURL, FileSource(file));
            case Resolution.PreferFile:
                return EvalSource(FileSource(file), gitURL);
            case Resolution.DisablePackage:
                throw new Ex("Package is disabled");
            default:
                throw new Ex($"Unknown resolution policy: {resolution}");
        }
    }}

    // Ops and static ----------------------------------------------

    public static Dependency operator + (Dependency x, Dependency y)
    => new Dependency(){
        name       = y?.name       ?? x?.name,
        gitURL     = y?.gitURL     ?? x?.gitURL,
        file       = y?.file       ?? x?.file,
        resolution = y?.resolution ?? x?.resolution ?? Resolution.PreferGitURL,
        runTests   = y?.runTests   ?? x?.runTests   ?? false,
        teamRoles  = y?.teamRoles  ?? x?.teamRoles  ?? ""
    };

    // PRIVATE =====================================================

    string EvalSource(params string[] sources){
        foreach(var s in sources)
            if(!string.IsNullOrEmpty(s)) return s;
        return null;
    }

    bool IsValidSource(string arg)
    => arg != null
    && !string.IsNullOrEmpty(arg)
    && arg.Trim().Length > 0;

    static string FileSource(string path){
        if(string.IsNullOrEmpty(path)) return path;
        var fullpath = System.IO.Path.GetFullPath(path);
        if(path != fullpath){
            Log($"PuP: Note: '{path}' expands to '{fullpath}'");
        }
        return "file:" + fullpath.Replace("\\", "/");
    }

    static string NameFromURL(string url){
        if(url == null) return null;
        if(string.IsNullOrEmpty(url)) return null;
        var i = url.LastIndexOf("/");
        if(i < 0) i = 0;
        url = url.Substring(i + 1);
        return url.Replace(".git", "");
    }

    static string NameFromFile(string file){
        return NameFromURL(file);
    }

}}
