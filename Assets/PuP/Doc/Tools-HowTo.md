# Making tools and libraries for Unity

## Manager

Many tools have a default configuration through a scriptable object; can be loaded this way

```cs
using UnityEngine;
using ADB = UnityEditor.AssetDatabase;

public static T Load<T>(string path) where T : Object
    => ADB.LoadAssetAtPath<T>($"Assets/{path}.asset");
```

But also you want to auto-create the configuration if
it does not exist.

```cs
using Dir = System.IO.Directory;

var so = ADB.LoadAssetAtPath<T>(FullPath);
if(!so){
    Dir.CreateDirectory(Path);
    so = ScriptableObject.CreateInstance<PersonalSettings>();
    ADB.CreateAsset(so, FullPath);
}
```

## User interface

Put a menu under Editor/UI/Menu.cs; something like

```cs
namespace Acme.Lib{
public static class Menu{

    [MenuItem("Window/Activ/Pup/Apply dependencies")]
    public static void ApplyDependencies(){
        Log("Apply deps");
    }

}}
```

[TODO] often users prefer a menu item to edit SOs they have not created themselves
