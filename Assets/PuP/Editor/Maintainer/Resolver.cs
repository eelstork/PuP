using System.Collections.Generic;
using System.Linq;
using InvOp = System.InvalidOperationException;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class Resolver{

    static ProcessingQueue queue;

    public static void Apply(List<Dependency> deps){
        if(queue?.hasPendingJobs ?? false){
            throw new InvOp("UPM client is busy");
        }
        queue = new ProcessingQueue();
        queue.StartProcessing(deps);
    }

    public static void Stop(){
        queue?.StopProcessing();
    }

}}
