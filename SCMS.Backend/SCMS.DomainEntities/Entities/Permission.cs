using System;
using System.Collections.Generic;
namespace SCMS.DomainEntities.Entities;
public partial class Permission
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái hoạt động của quyền (true: đang hoạt động, false: không hoạt động)
    /// </summary>
    public bool IsActive { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
