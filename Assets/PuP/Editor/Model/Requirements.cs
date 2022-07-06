using System.Collections.Generic;
using UnityEngine;
using ArgEx = System.ArgumentException;

namespace Activ.PuP{
public class Requirements : ScriptableObject{

    public List<Dependency> dependencies = new List<Dependency>();

    public void Add(Dependency arg){
        this[arg.name] = this[arg.name] + arg;
    }

    public static Requirements operator + (Requirements x, Requirements y)
    {
        Requirements z = ScriptableObject.CreateInstance<Requirements>();
        z.name = $"({x.name} ∪ {y.name})";
        if(x != null) foreach(var e in x.deps) z.Add(e);
        if(z != null) foreach(var e in y.deps) z.Add(e);
        return z;
    }

    public Dependency this[string name]{
        get => deps.Find( x => x.name == name);
        set{
            if(this.Contains(name)){
                deps[ IndexOf(name) ] = value;
            }else{
                dependencies.Add(value);
            }
        }
    }

    bool Contains(string name)
    => deps.Find( x => x.name == name) != null;

    int IndexOf(string name){
        var i = deps.FindIndex(x => x.name == name);
        if(i < 0) throw new ArgEx($"Dependency not in {this.name}: {name}");
        return i;
    }

    void OnValidate(){
        foreach(var e in dependencies) e.Validate();
    }

    List<Dependency> deps => dependencies;

}}
