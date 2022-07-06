namespace Activ.PuP{
public class PackageRef{

    public readonly string name;
    public readonly string displayName;
    public readonly string path;

    public PackageRef(string name, string displayName, string path){
        this.name = name;
        this.displayName = displayName;
        this.path = path;
    }

    override public string ToString() => $"{displayName} ( {path} )";

}}
