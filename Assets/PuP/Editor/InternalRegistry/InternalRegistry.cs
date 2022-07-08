using System.Linq;
using ArgEx = System.ArgumentException;

namespace Activ.PuP{
public static partial class InternalRegistry{

    static string[] names;

    public static string[] avail{ get{
        if(names == null) names =
            (from e in packages select Desc(e)).ToArray();
        return names;
    }}

    // TODO for safety if already added just provided a notice?
    // But we do need to "restore" previously deleted packages
    public static void AddPackage(int index, Requirements to){
        if(index == 0) throw new ArgEx("zero indicates no package");
        var src = packages[index];
        Dependency dep = new Dependency(){
            name = src.name,
            //displayName = src.displayName,
            gitURL = src.url,
            resolution = Dependency.Resolution.PreferGitURL
        };
        to[src.name] = dep;
    }

    static string Desc(XPackageRef arg)
    => arg == null ? "..."
                   : $"{arg.displayName} ({arg.shortDescription})";

}}
