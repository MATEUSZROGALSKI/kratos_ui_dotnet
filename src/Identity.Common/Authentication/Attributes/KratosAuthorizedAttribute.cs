using Microsoft.AspNetCore.Authorization;

namespace Identity.Common.Authentication.Attributes;

public sealed class KratosAuthorizedAttribute : AuthorizeAttribute
{
    public KratosAuthorizedAttribute() : base() 
    {
        AuthenticationSchemes = EnvironmentVariables.SchemaName;
    }
}
