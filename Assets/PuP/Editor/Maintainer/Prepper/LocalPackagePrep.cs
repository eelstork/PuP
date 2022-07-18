using System.IO;
using ArgEx = System.ArgumentException;
using Ex = System.Exception;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class LocalPackagePrep{

    public static string Process(string path, Dependency dep){
        var url = dep.gitURL;
        if(string.IsNullOrEmpty(path)){
            throw new ArgEx($"Null or empty source path for {dep}");
        }
        var fullpath = Path.GetFullPath(path);
        if(path != fullpath){
            Log($"PuP: Note: '{path}' expands to '{fullpath}'");
        }
        if(!Directory.Exists(fullpath))
            CloneRepo(url, fullpath);
        if(!Directory.Exists(fullpath)){
            throw new Ex($"Directory does not exist; `git clone` failed? Tried Git URL '{url}'");
        }
        //
        UpdateRepo(GetRoot(path));
        //
        return "file:" + fullpath.Replace("\\", "/");
    }

    public static void CloneRepo(string gitURL, string fullpath){
        Log($"Clone repository:\n{gitURL} => {fullpath}");
        var cmd = $"clone {gitURL} {fullpath}";
        var @out = GitRunner.Cmd(".", cmd);
        //Log(@out);
    }

    static string UpdateRepo(string path){
        var @out = GitRunner.Cmd(path, "reset --hard");
        Log($"[git reset --hard] > {@out}");
        @out = GitRunner.Cmd(path, "clean -fd");
        if(!string.IsNullOrEmpty(@out))
            Log($"[git clean --fd] > {@out}");
        @out = GitRunner.Cmd(path, "pull --force");
        Log($"[git pull --force] > {@out}");
		//GitHelper.stamp = 0;
        return @out;
    }

    static string GetRoot(string path) => null;

}}
