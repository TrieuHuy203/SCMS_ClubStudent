using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SCMS.DomainEntities.Enums
{
    public enum AppPermission
    {
       //////////////////////////////// Quyền controller admin /////////////////
       

 // AdminAuditLogController - Xem nhật ký hoạt động
 [Description("ADMIN_AUDIT_LOG_VIEW")] Admin_AuditLog_View,  // xong

 // AdminAuthLogController - Xem nhật ký đăng nhập
[Description("ADMIN_AUTH_LOG_VIEW")] Admin_AuthLog_View, 

 // AdminClubFavoriteController - Quản lý yêu thích CLB
 [Description("ADMIN_CLUB_FAVORITE_VIEW_LIST")] Admin_Club_Favorite_View_List, 
[Description("ADMIN_CLUB_FAVORITE_VIEW_DETAIL")] Admin_Club_Favorite_View_Detail,             

// AdminClubStatisticsController - Xem thống kê CLB
[Description("ADMIN_CLUB_STATISTICS_VIEW")] Admin_Club_Statistics_View,  

// AdminCommentController - Quản lý bình luận
[Description("ADMIN_COMMENT_VIEW_LIST")] Admin_Comment_View_List,
[Description("ADMIN_COMMENT_VIEW_DETAIL")] Admin_Comment_View_Detail, 
[Description("ADMIN_COMMENT_UPDATE")] Admin_Comment_Update, 
[Description("ADMIN_COMMENT_DELETE_ANY")] Admin_Comment_Delete_Any, 
[Description("ADMIN_COMMENT_VIEW_BY_CLUB")] Admin_Comment_View_By_Club, 

// AdminDashboardController.cs - Xem dashboard tổng quan admin 
[Description("ADMIN_DASHBOARD_VIEW_OVERVIEW")] Admin_Dashboard_View_Overview, 
[Description("ADMIN_DASHBOARD_VIEW_TODAY_METRICS")] Admin_Dashboard_View_Today_Metrics,
[Description("ADMIN_DASHBOARD_VIEW_STATUS_BREAKDOWN")] Admin_Dashboard_View_Status_Breakdown,
[Description("ADMIN_DASHBOARD_VIEW_GROWTH_CHART")] Admin_Dashboard_View_Growth_Chart,
[Description("ADMIN_DASHBOARD_VIEW_MODERATION")] Admin_Dashboard_View_Moderation,
[Description("ADMIN_DASHBOARD_VIEW_TOP_RANKING")] Admin_Dashboard_View_Top_Ranking,
[Description("ADMIN_DASHBOARD_VIEW_RECENT_ACTIVITY")] Admin_Dashboard_View_Recent_Activity,

// AdminEventRegistrationController - Quản lý đăng ký sự kiện

[Description("ADMIN_EVENT_REGISTRATION_VIEW_DETAIL")] Admin_EventRegistration_View_Detail,
[Description("ADMIN_EVENT_REGISTRATION_VIEW_LIST")] Admin_EventRegistration_View_List,
[Description("ADMIN_EVENT_REGISTRATION_VIEW_ALL")] Admin_EventRegistration_View_All, 
[Description("ADMIN_EVENT_REGISTRATION_SEARCH")] Admin_EventRegistration_Search,
[Description("ADMIN_EVENT_REGISTRATION_VIEW_EVENT_REGISTRATIONS_LIST")] Admin_EventRegistration_View_Event_Registrations_List,
[Description("ADMIN_EVENT_REGISTRATION_VIEW_EVENT_REGISTRATIONS")] Admin_EventRegistration_View_Event_Registrations,
[Description("ADMIN_EVENT_REGISTRATION_SEARCH_EVENT_REGISTRATIONS")] Admin_EventRegistration_Search_Event_Registrations,
[Description("ADMIN_EVENT_REGISTRATION_EXPORT_EVENT_REGISTRATIONS")] Admin_EventRegistration_Export_Event_Registrations,

// AdminFeedbackController.cs - Quản lý phản hồi
[Description("ADMIN_FEEDBACK_VIEW_LIST")] Admin_Feedback_View_List,
[Description("ADMIN_FEEDBACK_VIEW_DETAIL")] Admin_Feedback_View_Detail,
[Description("ADMIN_FEEDBACK_MARK_REVIEWED")] Admin_Feedback_Mark_Reviewed,
[Description("ADMIN_FEEDBACK_MARK_RESOLVED")] Admin_Feedback_Mark_Resolved,
[Description("ADMIN_FEEDBACK_UPDATE")] Admin_Feedback_Update,
[Description("ADMIN_FEEDBACK_DELETE")] Admin_Feedback_Delete,

// AdminMembershipController.cs - Quản lý membership
[Description("ADMIN_MEMBERSHIP_VIEW_LIST")] Admin_Membership_View_List,
[Description("ADMIN_MEMBERSHIP_VIEW_DETAIL")] Admin_Membership_View_Detail,
[Description("ADMIN_MEMBERSHIP_VIEW_CLUB_MEMBERS")] Admin_Membership_View_Club_Members,
[Description("ADMIN_MEMBERSHIP_VIEW_USER_APPLICATIONS")] Admin_Membership_View_User_Applications,
[Description("ADMIN_MEMBERSHIP_VIEW_PENDING_APPLICATIONS")] Admin_Membership_View_Pending_Applications,
[Description("ADMIN_MEMBERSHIP_VIEW_REJECTED_APPLICATIONS")] Admin_Membership_View_Rejected_Applications,
[Description("ADMIN_MEMBERSHIP_APPROVE")] Admin_Membership_Approve,
[Description("ADMIN_MEMBERSHIP_REJECT")] Admin_Membership_Reject,
[Description("ADMIN_MEMBERSHIP_KICK")] Admin_Membership_Kick,

// AdminPermissionController.cs - Quản lý phân quyền
[Description("ADMIN_PERMISSION_VIEW_LIST")] Admin_Permission_View_List,
[Description("ADMIN_PERMISSION_VIEW_DETAIL")] Admin_Permission_View_Detail,
[Description("ADMIN_PERMISSION_CREATE")] Admin_Permission_Create,
[Description("ADMIN_PERMISSION_UPDATE")] Admin_Permission_Update,
[Description("ADMIN_PERMISSION_DELETE")] Admin_Permission_Delete,
[Description("ADMIN_PERMISSION_SEARCH")] Admin_Permission_Search,

// AdminPostController.cs - Quản lý bài viết
[Description("ADMIN_POST_VIEW_LIST")] Admin_Post_View_List,
[Description("ADMIN_POST_VIEW_DETAIL")] Admin_Post_View_Detail,
[Description("ADMIN_POST_CREATE")] Admin_Post_Create,
[Description("ADMIN_POST_UPDATE")] Admin_Post_Update,
[Description("ADMIN_POST_DELETE_ANY")] Admin_Post_Delete_Any,
[Description("ADMIN_POST_APPROVE")] Admin_Post_Approve,
[Description("ADMIN_POST_REJECT")] Admin_Post_Reject,
[Description("ADMIN_POST_VIEW_PENDING_BY_CLUB")] Admin_Post_View_Pending_By_Club,
[Description("ADMIN_POST_VIEW_PENDING")] Admin_Post_View_Pending,
[Description("ADMIN_POST_VIEW_APPROVED")] Admin_Post_View_Approved,
[Description("ADMIN_POST_VIEW_REJECTED")] Admin_Post_View_Rejected,


// AdminPostImageController - Quản lý hình ảnh bài viết
[Description("ADMIN_POST_IMAGE_VIEW_LIST")] Admin_Post_Image_View_List,
[Description("ADMIN_POST_IMAGE_VIEW_DETAIL")] Admin_Post_Image_View_Detail,
[Description("ADMIN_POST_IMAGE_DELETE")] Admin_Post_Image_Delete,
[Description("ADMIN_POST_IMAGE_VIEW_BY_CLUB")] Admin_Post_Image_View_By_Club,


// AdminPostReportController - Quản lý báo cáo bài viết
[Description("ADMIN_POST_REPORT_VIEW_LIST")] Admin_Post_Report_View_List,
[Description("ADMIN_POST_REPORT_VIEW_DETAIL")] Admin_Post_Report_View_Detail,
[Description("ADMIN_POST_REPORT_MARK_REVIEWED")] Admin_Post_Report_Mark_Reviewed,
[Description("ADMIN_POST_REPORT_MARK_RESOLVED")] Admin_Post_Report_Mark_Resolved,

//AdminRoleController - Quản lý vai trò
[Description("ADMIN_ROLE_VIEW_LIST")] Admin_Role_View_List,

// AdminRolePermissionController - Quản lý phân quyền vai trò
[Description("ADMIN_ROLE_PERMISSION_VIEW_LIST")] Admin_Role_Permission_View_List,
[Description("ADMIN_ROLE_PERMISSION_ADD")] Admin_Role_Permission_Add,
[Description("ADMIN_ROLE_PERMISSION_DELETE")] Admin_Role_Permission_Delete,
[Description("ADMIN_ROLE_PERMISSION_SEARCH")] Admin_Role_Permission_Search,


// AdminUserController - Quản lý người dùng
[Description("ADMIN_USER_VIEW_LIST")] Admin_User_View_List,
[Description("ADMIN_USER_VIEW_DETAIL")] Admin_User_View_Detail,
[Description("ADMIN_USER_SEARCH")] Admin_User_Search,
[Description("ADMIN_USER_CREATE")] Admin_User_Create,
[Description("ADMIN_USER_UPDATE")] Admin_User_Update,
[Description("ADMIN_USER_DELETE")] Admin_User_Delete,
[Description("ADMIN_USER_SET_STATUS")] Admin_User_Set_Status,
[Description("ADMIN_USER_RESET_PASSWORD")] Admin_User_Reset_Password,
[Description("ADMIN_USER_AVATAR_VIEW_LIST")] Admin_User_Avatar_View_List,
[Description("ADMIN_USER_AVATAR_VIEW_DETAIL")] Admin_User_Avatar_View_Detail,
[Description("ADMIN_USER_AVATAR_DELETE")] Admin_User_Avatar_Delete,

//AdminUserPermissionController - Quản lý phân quyền người dùng ghi đè
[Description("ADMIN_USER_PERMISSION_VIEW_LIST")] Admin_User_Permission_View_List,
[Description("ADMIN_USER_PERMISSION_ADD")] Admin_User_Permission_Add,
[Description("ADMIN_USER_PERMISSION_DELETE")] Admin_User_Permission_Delete,
[Description("ADMIN_USER_PERMISSION_SEARCH")] Admin_User_Permission_Search,
[Description("ADMIN_USER_PERMISSION_UPDATE")] Admin_User_Permission_Update,


// AdminUserRoleController - Quản lý vai trò người dùng 
[Description("ADMIN_USER_ROLE_VIEW_LIST")] Admin_User_Role_View_List,
[Description("ADMIN_USER_ROLE_ADD")] Admin_User_Role_Add,
[Description("ADMIN_USER_ROLE_DELETE")] Admin_User_Role_Delete,
[Description("ADMIN_USER_ROLE_SEARCH")] Admin_User_Role_Search,

// ClubController.cs - Quản lý CLB (admin)
[Description("ADMIN_CLUB_VIEW_LIST")] Admin_Club_View_List,
[Description("ADMIN_CLUB_VIEW_DETAIL")] Admin_Club_View_Detail,
[Description("ADMIN_CLUB_CREATE")] Admin_Club_Create,
[Description("ADMIN_CLUB_UPDATE")] Admin_Club_Update,
[Description("ADMIN_CLUB_DELETE")] Admin_Club_Delete,
[Description("ADMIN_CLUB_SET_STATUS")] Admin_Club_Set_Status,
[Description("ADMIN_CLUB_VIEW_PAGED")] Admin_Club_View_Paged,
[Description("ADMIN_CLUB_SEARCH")] Admin_Club_Search,
[Description("ADMIN_CLUB_VIEW_PENDING")] Admin_Club_View_Pending,
[Description("ADMIN_CLUB_APPROVE")] Admin_Club_Approve,
[Description("ADMIN_CLUB_REJECT")] Admin_Club_Reject,


// EventController.cs - Quản lý sự kiện (admin)
[Description("ADMIN_EVENT_CREATE")] Admin_Event_Create,
[Description("ADMIN_EVENT_UPDATE")] Admin_Event_Update,
[Description("ADMIN_EVENT_DELETE")] Admin_Event_Delete,
[Description("ADMIN_EVENT_APPROVE")] Admin_Event_Approve,
[Description("ADMIN_EVENT_REJECT")] Admin_Event_Reject,
[Description("ADMIN_EVENT_VIEW_DETAIL")] Admin_Event_View_Detail,
[Description("ADMIN_EVENT_VIEW_LIST")] Admin_Event_View_List,
[Description("ADMIN_EVENT_SEARCH")] Admin_Event_Search,
[Description("ADMIN_EVENT_VIEW_PENDING")] Admin_Event_View_Pending,
[Description("ADMIN_EVENT_VIEW_APPROVED")] Admin_Event_View_Approved,
[Description("ADMIN_EVENT_VIEW_REJECTED")] Admin_Event_View_Rejected,



       /// //////////////////////////////// Quyền controller user /////////////////
       
// AuthController- Đăng ký, đăng nhập, refresh token, thay đổi mật khẩu, quên mật khẩu
// [Description("AUTH_REGISTER")] Auth_Register,
// [Description("AUTH_LOGIN")] Auth_Login,
// [Description("AUTH_CONFIRM_EMAIL")] Auth_Confirm_Email,
// [Description("AUTH_FORGOT_PASSWORD")] Auth_Forgot_Password,
// [Description("AUTH_RESET_PASSWORD")] Auth_Reset_Password,
// [Description("AUTH_LOGOUT")] Auth_Logout,
     
// EventRegistrationController - Đăng ký sự kiện, xem đăng ký của mình, xem chi tiết đăng ký của mình
[Description("EVENT_REGISTRATION_REGISTER")] Event_Registration_Register,
[Description("EVENT_REGISTRATION_VIEW_MY_LIST")] Event_Registration_View_My_List,
[Description("EVENT_REGISTRATION_VIEW_MY_DETAIL")] Event_Registration_View_My_Detail,
[Description("EVENT_REGISTRATION_CANCEL_MY_REGISTRATION")] Event_Registration_Cancel_My_Registration,
[Description("EVENT_REGISTRATION_VIEW_CLUB_EVENT_REGISTRATIONS")] Event_Registration_View_Club_Event_Registrations, // Nhóm trưởng xem danh sách thành viên tham gia sự kiện của CLB mình tổ chức
[Description("EVENT_REGISTRATION_EXPORT_CLUB_EVENT_REGISTRATIONS")] Event_Registration_Export_Club_Event_Registrations,
       
// MembershipCOntroller - Đăng ký tham gia CLB, xem đơn đã đăng ký của mình, xem chi tiết đơn đã đăng ký của mình [Description("MEMBERSHIP_REGISTER")] Membership_Register,
[Description("MEMBERSHIP_REGISTER")] Membership_Register,
[Description("MEMBERSHIP_VIEW_MY_APPLICATIONS")] Membership_View_My_Applications,
[Description("MEMBERSHIP_VIEW_MY_APPLICATION_DETAIL")] Membership_View_My_Application_Detail,
[Description("MEMBERSHIP_CANCEL_MY_APPLICATION")] Membership_Cancel_My_Application,
[Description("MEMBERSHIP_VIEW_MY_CLUBS")] Membership_View_My_Clubs,
[Description("MEMBERSHIP_VIEW_MY_CLUB_MEMBERS")] Membership_View_My_Club_Members,
[Description("MEMBERSHIP_LEAVE_CLUB")] Membership_Leave_Club,  
       
 // ProfileController - Xem và cập nhật thông tin cá nhân, xem và cập nhật avatar
[Description("PROFILE_VIEW_MY_PROFILE")] Profile_View_My_Profile,
[Description("PROFILE_UPDATE_MY_PROFILE")] Profile_Update_My_Profile,
[Description("PROFILE_UPLOAD_AVATAR")] Profile_Upload_Avatar,
[Description("PROFILE_CHANGE_PASSWORD")] Profile_Change_Password,


// UserClubController - Xem danh sách CLB, xem chi tiết CLB, xem sự kiện của CLB, xem thành viên của CLB
[Description("USER_CLUB_REGISTER")] User_Club_Register,
[Description("USER_CLUB_VIEW_LIST")] User_Club_View_List,
[Description("USER_CLUB_SEARCH")] User_Club_Search,
[Description("USER_CLUB_VIEW_DETAIL")] User_Club_View_Detail,

// UserClubFavoriteController - Thêm, xóa CLB yêu thích, xem danh sách CLB yêu thích
 [Description("USER_CLUB_FAVORITE_ADD")] User_Club_Favorite_Add,
[Description("USER_CLUB_FAVORITE_REMOVE")] User_Club_Favorite_Remove,
[Description("USER_CLUB_FAVORITE_VIEW_LIST")] User_Club_Favorite_View_List,
[Description("USER_CLUB_FAVORITE_CHECK_STATUS")] User_Club_Favorite_Check_Status,      
       
// UserCommentController - Thêm, sửa, xóa bình luận của chính mình, xem danh sách bình luận của chính mình
[Description("USER_COMMENT_CREATE")] User_Comment_Create,
[Description("USER_COMMENT_VIEW_LIST")] User_Comment_View_List,
[Description("USER_COMMENT_VIEW_DETAIL")] User_Comment_View_Detail,
[Description("USER_COMMENT_UPDATE")] User_Comment_Update,
[Description("USER_COMMENT_DELETE")] User_Comment_Delete,
[Description("USER_COMMENT_LIKE")] User_Comment_Like,
[Description("USER_COMMENT_UNLIKE")] User_Comment_Unlike,

       
       
// UserEventController - Xem danh sách sự kiện, xem chi tiết sự kiện
[Description("USER_EVENT_VIEW_LIST")] User_Event_View_List,
[Description("USER_EVENT_VIEW_MY_CLUBS_EVENTS")] User_Event_View_My_Clubs_Events,
[Description("USER_EVENT_SEARCH")] User_Event_Search,
[Description("USER_EVENT_VIEW_DETAIL")] User_Event_View_Detail,

// UserFeedbackController - Gửi phản hồi, xem danh sách phản hồi của chính mình, xem chi tiết phản hồi của chính mình
[Description("USER_FEEDBACK_CREATE")] User_Feedback_Create,
[Description("USER_FEEDBACK_VIEW_LIST")] User_Feedback_View_List,
[Description("USER_FEEDBACK_VIEW_DETAIL")] User_Feedback_View_Detail,
[Description("USER_FEEDBACK_UPDATE")] User_Feedback_Update,
[Description("USER_FEEDBACK_DELETE")] User_Feedback_Delete,
// UserPostController - Tạo bài viết, xem danh sách bài viết của chính mình
[Description("USER_POST_CREATE")] User_Post_Create,
[Description("USER_POST_VIEW_MY_LIST")] User_Post_View_My_List,
[Description("USER_POST_VIEW_MY_CLUB_LIST")] User_Post_View_My_Club_List,
[Description("USER_POST_VIEW_PUBLIC_LIST")] User_Post_View_Public_List,
[Description("USER_POST_VIEW_MY_DETAIL")] User_Post_View_My_Detail,
[Description("USER_POST_UPDATE")] User_Post_Update,
[Description("USER_POST_DELETE")] User_Post_Delete,
[Description("USER_POST_LIKE")] User_Post_Like,
[Description("USER_POST_UNLIKE")] User_Post_Unlike,
[Description("USER_POST_REPORT")] User_Post_Report,
[Description("USER_POST_VIEW_LIKED_LIST")] User_Post_View_Liked_List,


// UserPostImageController - Thêm, xóa hình ảnh bài viết của chính mình, xem danh sách hình ảnh bài viết của chính mình
[Description("USER_POST_IMAGE_UPLOAD")] User_Post_Image_Upload,
[Description("USER_POST_IMAGE_VIEW_LIST")] User_Post_Image_View_List,
[Description("USER_POST_IMAGE_DELETE")] User_Post_Image_Delete,
    }

    public static class AppPermissionExtensions
    {
        public static string GetName(this AppPermission permission)
        {
            var field = permission.GetType().GetField(permission.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? permission.ToString();
        }

        public static IEnumerable<string> GetAllPermissionNames()
        {
            return Enum.GetValues(typeof(AppPermission))
                .Cast<AppPermission>()
                .Select(p => p.GetName());
        }

        public static AppPermission? ToAppPermission(this string permissionName)
        {
            foreach (var permission in Enum.GetValues(typeof(AppPermission)).Cast<AppPermission>())
            {
                if (permission.GetName() == permissionName)
                {
                    return permission;
                }
            }
            return null;
        }

        public static IEnumerable<AppPermission> GetAll()
        {
            return Enum.GetValues(typeof(AppPermission)).Cast<AppPermission>();
        }

        public static IEnumerable<AppPermission> GetByPrefix(string prefix)
        {
            return GetAll().Where(p => p.GetName().StartsWith(prefix));
        }

        public static IEnumerable<AppPermission> GetByResource(string resourceName)
        {
            return GetAll().Where(p => p.GetName().Contains(resourceName));
        }
    }
}