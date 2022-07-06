using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class Resolver{

    public static void Apply(List<Dependency> deps){
        //foreach(var dep in deps){
        //    Log($"dep will be applied {dep.Format()}");
        //}
        UPMClientMethods.AddAndRemove(PackagesToAdd(deps), PackagesToRemove(deps));
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
