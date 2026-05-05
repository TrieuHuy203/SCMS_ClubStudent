using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;
public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsDisabled { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpiry { get; set; }
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiry { get; set; }
    public string? AvatarUrl { get; set; }

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<AuthLog> AuthLogs { get; set; } = new List<AuthLog>();

    public virtual ICollection<ClubFavorite> ClubFavorites { get; set; } = new List<ClubFavorite>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual ICollection<Faq> Faqs { get; set; } = new List<Faq>();

    public virtual ICollection<Feedback> FeedbackProcessedByNavigations { get; set; } = new List<Feedback>();

    public virtual ICollection<Feedback> FeedbackUsers { get; set; } = new List<Feedback>();

    public virtual ICollection<FileAttachment> FileAttachments { get; set; } = new List<FileAttachment>();

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();

    public virtual ICollection<SurveyResult> SurveyResults { get; set; } = new List<SurveyResult>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
