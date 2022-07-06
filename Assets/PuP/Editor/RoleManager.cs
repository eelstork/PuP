using System.Linq;
using static UnityEngine.Debug;

namespace Activ.PuP{
public static class RoleManager{

    // Given a list of roles (comma separated) does the local user
    // advertise any of these?
    // If the list does not specify any role, return true
    public static bool Validate(string rolesCommaSep){
        var roles = AsArray(rolesCommaSep);
        if(roles == null) return true;
        if(roles.Length == 0) return true;
        // TODO approximate, would match 'dev' against 'superdev'
        foreach(var role in roles){
            if(Config.roles.Contains(role)) return true;
        }
        return false;
    }

    public static string[] AsArray(string roles)
    => (roles == null) ? (string[])null : (
        from role in roles.Split(',')
        where !string.IsNullOrEmpty(role)
        select role.Trim()
    ).ToArray();

}}
