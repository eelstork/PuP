namespace Activ.PuP{
public static class StringExt{

    public static string Clean(this string arg){
        arg = arg.Trim();
        return arg.Length == 0 ? null : arg;
    }

}}
