using System.Collections.Generic;
using System.Linq;
using InvOp = System.InvalidOperationException;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class Resolver{

    static UPMAgent agent;

    public static void Apply(List<Dependency> deps){
        if(agent?.hasPendingJobs ?? false){
            throw new InvOp("UPM client is busy");
        }
        agent = new UPMAgent();
        agent.StartProcessing(deps);
    }

    public static void Stop(){
        agent?.StopProcessing();
    }

    static (string name, string source)[] PackagesToAdd(List<Dependency> deps)
    => (from e in deps where e.isRequired
        select (e.name, e.source)).ToArray();

    static string[] PackagesToRemove(List<Dependency> deps){
        List<string> @out = new List<string>(deps.Count);
        foreach(var dep in deps)
            dep.GetRemovableSources(@out);
        return @out.ToArray();
    }


}}
