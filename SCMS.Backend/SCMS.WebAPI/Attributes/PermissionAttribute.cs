using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SCMS.DomainEntities.Enums;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Common.Extensions;


namespace SCMS.WebAPI.Attributes{
public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly AppPermission _permission;

    public PermissionAttribute(AppPermission permission)
    {
        _permission = permission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
{
    var userIdClaim = context.HttpContext.User.FindFirst("userId");

    if (userIdClaim == null ||
        !int.TryParse(userIdClaim.Value, out var userId))
    {
        context.Result = new UnauthorizedResult();
        return;
    }

    var permissionService = context.HttpContext.RequestServices
        .GetRequiredService<IPermissionService>();

    var permissionName = _permission.GetDescription();

    if (string.IsNullOrEmpty(permissionName))
    {
        context.Result = new StatusCodeResult(500);
        return;
    }

    var hasPermission = await permissionService.HasPermissionAsync(userId, permissionName);

    if (!hasPermission)
    {
        context.Result = new ForbidResult();
        return;
    }
}
}}