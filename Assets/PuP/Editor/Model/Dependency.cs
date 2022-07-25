using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Ex = System.Exception;
using User = UnityEngine.SerializeField;
using static UnityEngine.Debug;

namespace Activ.PuP{
[Serializable]
public class Dependency{

    public enum Resolution{ PreferGitURL, PreferFile, DisablePackage }
    public enum SourceStatus{ Local, Remote, Invalid, Disabled }

    [User] public string name, gitURL, file;
    [User] public Resolution resolution;
    [User] public bool runTests;
    [User] public bool skipUpdates;
    [User] public string teamRoles;
    public string log;

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

    // Ops and static ----------------------------------------------

    public static Dependency operator + (Dependency x, Dependency y)
    => new Dependency(){
        name        = y?.name        ?? x?.name,
        gitURL      = y?.gitURL      ?? x?.gitURL,
        file        = y?.file        ?? x?.file,
        resolution  = y?.resolution  ?? x?.resolution  ?? Resolution.PreferGitURL,
        runTests    = y?.runTests    ?? x?.runTests    ?? false,
        teamRoles   = y?.teamRoles   ?? x?.teamRoles   ?? "",
        skipUpdates = y?.skipUpdates ?? x?.skipUpdates ?? false
    };

    // PRIVATE =====================================================

    string EvalSource(params string[] sources){
        foreach(var s in sources)
            if(!string.IsNullOrEmpty(s)) return s;
        return null;
    }

    public string GetSource(out SourceStatus status){
        switch (resolution){
            case Resolution.PreferGitURL:
                if(hasValidRemote){
                    status = SourceStatus.Remote;
                    return gitURL;
                }else if(hasValidLocal){
                    status = SourceStatus.Local;
                    return file;
                }
                break;
            case Resolution.PreferFile:
                if(hasValidLocal){
                    status = SourceStatus.Local;
                    return file;
                }else if(hasValidRemote){
                    status = SourceStatus.Remote;
                    return gitURL;
                }
                break;
            case Resolution.DisablePackage:
                status = SourceStatus.Disabled;
                return null;
            default:
                throw new Ex($"Unknown resolution policy: {resolution}");
        }
        status = SourceStatus.Invalid;
        return null;
    }

    bool IsValidSource(string arg)
    => arg != null
    && !string.IsNullOrEmpty(arg)
    && arg.Trim().Length > 0;

    static string FileSource(string path){
        if(string.IsNullOrEmpty(path)) return path;
        var fullpath = Path.GetFullPath(path);
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

    // Private properties ==========================================

    bool hasValidLocal => IsValidSource(file);

    bool hasValidRemote => IsValidSource(gitURL);

}}
