using S = System.String;

namespace Activ.PuP{
public class XPackageRef{

    public readonly string displayName;
    public readonly string name;
    public readonly string url;
    public readonly string shortDescription;

    override public string ToString() => $"{displayName} ({shortDescription})";

    public XPackageRef(string displayName, string name, string shortDescription, string url){
        this.displayName = displayName;
        this.name = name;
        this.shortDescription = shortDescription;
        this.url = url;
    }

    public static implicit operator XPackageRef (
                      (S displayName, S name, S shortDescription, S url) arg)
    => new XPackageRef(
        arg.displayName, arg.name, arg.shortDescription, arg.url
    );

}}
