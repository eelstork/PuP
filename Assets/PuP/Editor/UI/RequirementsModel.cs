using Dep = Activ.PuP.Dependency;
using Req = Activ.PuP.Requirements;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class RequirementsModel{

    public static void Remove(Dep dep, Req @from){
        @from.dependencies.Remove(dep);
        Log(
              $"NOTE: deleting {dep} does not uninstall the package;\n"
            + "to remove a package from locals, select 'DisablePackage' under resolution."
        );
    }

    public static void AddCommonPackage(int pkgIndex, Req @to){
        if(pkgIndex <= 0) return;
        InternalRegistry.AddPackage(pkgIndex, @to);
        Manager.ApplyDeps();
    }

    public static void AddLocalPackage(int pkgIndex, Req @to){
        if(pkgIndex < 0) return;
        var @ref = Manager.GetLocalPackage(pkgIndex);
        @to.Add( new Dependency(){
            name = @ref.name,
            file = @ref.path
        });
        Manager.ApplyDeps();
    }

}}
