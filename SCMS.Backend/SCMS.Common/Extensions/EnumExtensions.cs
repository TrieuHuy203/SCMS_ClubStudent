    using System;
using System.ComponentModel;
using System.Reflection;

namespace SCMS.Common.Extensions // hoặc namespace bạn đang dùng
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
{
    if (value == null) return string.Empty;

    var type = value.GetType();
    var field = type.GetField(value.ToString());

    if (field == null)
        return value.ToString();

    var attr = field.GetCustomAttribute<DescriptionAttribute>();

    return attr?.Description ?? value.ToString();
}
    }
}


/*

mapping giữa enum và string lưu trong DB 
vì gọi enum trong code [Permission(AppPermisiion. Admin_AuditLog_View)] thì file này giups mapping với string lưu trong DB là "ADMIN_AUDIT_LOG_VIEW" để kiểm tra quyền khi user gọi API có permission này hay không

*/