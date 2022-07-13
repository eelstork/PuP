using System;

namespace Activ.PuP{
public class PackageRef : IComparable{

    public readonly string name;
    public readonly string displayName;
    public readonly string path;

    public PackageRef(string name, string displayName, string path){
        this.name = name;
        this.displayName = displayName;
        this.path = path;
    }

    int IComparable.CompareTo(object arg){
        if(arg is PackageRef)
            return displayName.CompareTo(((PackageRef)arg).displayName);
        else
            return name.CompareTo(arg?.ToString());
    }

    override public string ToString() => $"{displayName} ( {path} )";

}}
