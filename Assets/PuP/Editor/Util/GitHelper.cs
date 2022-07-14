using static UnityEngine.Debug;

namespace Activ.PuP{
public static class GitHelper{

    public static void Clone(string gitURL, string fullpath){
        Log($"Clone repository:\n{gitURL} => {fullpath}");
        var cmd = $"clone {gitURL} {fullpath}";
        var @out = GitRunner.Cmd(".", cmd);
        //Log(@out);
    }

}}
