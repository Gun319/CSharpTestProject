using Microsoft.AspNetCore.Authorization;

namespace Jwt.Commons
{
    public class PermissionAuthorizationRequirement: IAuthorizationRequirement
    {
        public PermissionAuthorizationRequirement(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
