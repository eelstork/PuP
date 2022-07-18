using SourceStatus = Activ.PuP.Dependency.SourceStatus;

namespace Activ.PuP{
public static class Preprocessor{

    public static string UpdateSource(Dependency dep){
        var source = dep.GetSource(out SourceStatus status);
        switch(status){
        case SourceStatus.Local:
            return LocalPackagePrep.Process(source, dep);
        case SourceStatus.Remote:
            return RemotePackagePrep.Process(source, dep);
        default:
            return null;
        }
    }

}}
