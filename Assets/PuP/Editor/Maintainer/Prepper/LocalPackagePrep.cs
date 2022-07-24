using System.IO;
using System.Text;
using ArgEx = System.ArgumentException;
using Ex = System.Exception;

namespace Activ.PuP{
public class LocalPackagePrep{

    public string log => _log.ToString();
    StringBuilder _log = new StringBuilder();

    public string Process(string path, Dependency dep){
        var url = dep.gitURL;
        if(string.IsNullOrEmpty(path)){
            throw new ArgEx($"Null or empty source path for {dep}");
        }
        var fullpath = Path.GetFullPath(path).Replace("\\", "/");
        if(path != fullpath){
            Log($"PuP: Note: '{path}' expands to '{fullpath}'");
        }
        var pathToRepo = PathToRepo(fullpath, url);
        if(!Directory.Exists(pathToRepo))
            CloneRepo(url, pathToRepo);
        if(!Directory.Exists(fullpath)){
            throw new Ex($"Directory does not exist; `git clone` failed? Tried Git URL '{url}'");
        }
        //
        UpdateRepo(pathToRepo);
        //
        return "file:" + fullpath.Replace("\\", "/");
    }

    void CloneRepo(string gitURL, string fullpath){
        Log($"Clone repository:\n{gitURL} => {fullpath}");
        var cmd = $"clone {gitURL} {fullpath}";
        var @out = GitRunner.Cmd(".", cmd);
    }

    string UpdateRepo(string path){
        var @out = GitRunner.Cmd(path, "status");
        Log($"[git status] > {@out}");
        @out = GitRunner.Cmd(path, "reset --hard");
        Log($"[git reset --hard] > {@out}");
        @out = GitRunner.Cmd(path, "clean -fd");
        if(!string.IsNullOrEmpty(@out))
            Log($"[git clean --fd] > {@out}");
        @out = GitRunner.Cmd(path, "pull --force");
        Log($"[git pull --force] > {@out}");
        return @out;
    }

    string PathToRepo(string path, string gitURL){
        if(!gitURL.Contains("?path=")) return path;
        int i = gitURL.IndexOf("?path=");
        var subpath = gitURL.Substring(i + 6);
        Log($"subpath: {subpath}");
        var @out = path.Replace(subpath, "");
        Log($"PATH TO REPO: {@out}");
        return @out;
    }

    void Log(string arg) => _log.Append("\n" + arg);

}}