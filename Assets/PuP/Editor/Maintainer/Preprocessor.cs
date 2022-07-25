using SourceStatus = Activ.PuP.Dependency.SourceStatus;

namespace Activ.PuP{
public static class Preprocessor{

    public static string UpdateSource(Dependency dep,
                                      out string message){
        var source = dep.GetSource(out SourceStatus status);
        message = status.ToString();
        switch(status){
        case SourceStatus.Local:
            var prep = new LocalPackagePrep();
            var @out = prep.Process(source, dep);
            dep.log = prep.log;
            return @out;
        case SourceStatus.Remote:
            return RemotePackagePrep.Process(source, dep);
        default:
            return null;
        }
    }

}}
