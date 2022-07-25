using UnityEngine;
using Ed = UnityEditor.EditorApplication;
using GL = UnityEngine.GUILayout;
using EGL = UnityEditor.EditorGUILayout;
using static UnityEditor.EditorGUILayout;

namespace Activ.PuP{
public static class UpdateLogDrawer{

    public static void DrawLog(string text, ref Vector2? scroll, int? height = null){
        if(scroll.HasValue){
            if(height.HasValue){
                scroll = BeginScrollView(scroll.Value, GL.Height(height.Value));
            }else{
                scroll = BeginScrollView(scroll.Value);
            }
        }
        GUI.backgroundColor = Color.black;
        ConfigTextAreaStyle();
        GL.TextArea(text, GL.ExpandHeight(true));
        if(scroll.HasValue) EndScrollView();
        GUI.backgroundColor = Color.white;
    }

    static void ConfigTextAreaStyle(){
        //var f = monofont;
        //if(f == null) Debug.LogError("font not available");
        var style = GUI.skin.button;
        //style.font = f;
        //style.fontSize = FontSize;
        style.normal.textColor  = Color.white * 0.9f;
        style.focused.textColor = Color.white;
        style.focused.textColor = Color.white;
    }

}}
