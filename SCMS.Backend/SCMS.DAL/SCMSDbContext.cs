using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SCMS.DomainEntities.Entities;

public partial class SCMSDbContext : DbContext
{
    public SCMSDbContext()
    {
    }

    public SCMSDbContext(DbContextOptions<SCMSDbContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<AuthLog> AuthLogs { get; set; }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<ClubFavorite> ClubFavorites { get; set; }

    public virtual DbSet<ClubTag> ClubTags { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventRegistration> EventRegistrations { get; set; }

    public virtual DbSet<EventTag> EventTags { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FileAttachment> FileAttachments { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostTag> PostTags { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Share> Shares { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveyResult> SurveyResults { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=SCMS_DB;User Id=sa;Password=Password.1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.AchievementId).HasName("PK__Achievem__276330C0B4D69166");

            entity.ToTable("Achievement");

            entity.HasIndex(e => e.UserId, "IX_Achievement_UserId");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ReceivedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Achievements)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Achievement_User");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBDDA0EF70E");

            entity.ToTable("AuditLog");

            entity.HasIndex(e => new { e.TableName, e.RecordId }, "IX_AuditLog_Table_Record");

            entity.HasIndex(e => e.UserId, "IX_AuditLog_UserId");

            entity.Property(e => e.ActionTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ActionType).HasMaxLength(20);
            entity.Property(e => e.TableName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLog_User");
        });

        modelBuilder.Entity<AuthLog>(entity =>
        {
            entity.HasKey(e => e.AuthLogId).HasName("PK__AuthLog__FB951214058959CD");

            entity.ToTable("AuthLog");

            entity.HasIndex(e => e.ActionType, "IX_AuthLog_ActionType");

            entity.HasIndex(e => e.UserId, "IX_AuthLog_UserId");

            entity.Property(e => e.ActionTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ActionType).HasMaxLength(30);
            entity.Property(e => e.DeviceInfo).HasMaxLength(200);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.AuthLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuthLog_User");
        });

        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.ClubId).HasName("PK__Club__D35058E7C5514570");

            entity.ToTable("Club");

            entity.Property(e => e.ClubName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Field).HasMaxLength(100);
            entity.Property(e => e.MemberCount).HasDefaultValue(0);
            entity.Property(e => e.School).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
        });

        modelBuilder.Entity<ClubFavorite>(entity =>
        {
            entity.HasKey(e => e.ClubFavoriteId).HasName("PK__ClubFavo__7D83208EE9049892");

            entity.ToTable("ClubFavorite");

            entity.HasIndex(e => e.ClubId, "IX_ClubFavorite_ClubId");

            entity.HasIndex(e => e.UserId, "IX_ClubFavorite_UserId");

            entity.HasIndex(e => new { e.UserId, e.ClubId }, "IX_ClubFavorite_User_Club").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Club).WithMany(p => p.ClubFavorites)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClubFavorite_Club");

            entity.HasOne(d => d.User).WithMany(p => p.ClubFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClubFavorite_User");
        });

        modelBuilder.Entity<ClubTag>(entity =>
        {
            entity.HasKey(e => e.ClubTagId).HasName("PK__ClubTag__FBA7BBE7F1695A96");

            entity.ToTable("ClubTag");

            entity.HasIndex(e => e.ClubId, "IX_ClubTag_ClubId");

            entity.HasIndex(e => new { e.ClubId, e.TagId }, "IX_ClubTag_Club_Tag").IsUnique();

            entity.HasIndex(e => e.TagId, "IX_ClubTag_TagId");

            entity.HasOne(d => d.Club).WithMany(p => p.ClubTags)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClubTag_Club");

            entity.HasOne(d => d.Tag).WithMany(p => p.ClubTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClubTag_Tag");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFCADAD38D89");

            entity.ToTable("Comment");

            entity.HasIndex(e => e.PostId, "IX_Comment_PostId");

            entity.HasIndex(e => e.UserId, "IX_Comment_UserId");

            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C810A68D78D1");

            entity.ToTable("Event");

            entity.HasIndex(e => e.ClubId, "IX_Event_ClubId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EventName).HasMaxLength(100);
            entity.Property(e => e.EventTime).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Upcoming");

            entity.HasOne(d => d.Club).WithMany(p => p.Events)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Club");
        });

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasKey(e => e.EventRegistrationId).HasName("PK__EventReg__83225A920F2DB89D");

            entity.ToTable("EventRegistration");

            entity.HasIndex(e => e.EventId, "IX_EventRegistration_EventId");

            entity.HasIndex(e => e.UserId, "IX_EventRegistration_UserId");

            entity.HasIndex(e => new { e.UserId, e.EventId }, "IX_EventRegistration_User_Event").IsUnique();

            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.RegistrationStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Registered");

            entity.HasOne(d => d.Event).WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventRegistration_Event");

            entity.HasOne(d => d.User).WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventRegistration_User");
        });

        modelBuilder.Entity<EventTag>(entity =>
        {
            entity.HasKey(e => e.EventTagId).HasName("PK__EventTag__1DC94B51F40184B0");

            entity.ToTable("EventTag");

            entity.HasIndex(e => e.EventId, "IX_EventTag_EventId");

            entity.HasIndex(e => new { e.EventId, e.TagId }, "IX_EventTag_Event_Tag").IsUnique();

            entity.HasIndex(e => e.TagId, "IX_EventTag_TagId");

            entity.HasOne(d => d.Event).WithMany(p => p.EventTags)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTag_Event");

            entity.HasOne(d => d.Tag).WithMany(p => p.EventTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTag_Tag");
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.HasKey(e => e.Faqid).HasName("PK__FAQ__4B89D182DC051CB2");

            entity.ToTable("FAQ");

            entity.HasIndex(e => e.CreatedBy, "IX_FAQ_CreatedBy");

            entity.HasIndex(e => e.Type, "IX_FAQ_Type");

            entity.Property(e => e.Faqid).HasColumnName("FAQId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Question).HasMaxLength(500);
            entity.Property(e => e.Type).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Faqs)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_FAQ_CreatedBy");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD618FD4347");

            entity.ToTable("Feedback");

            entity.HasIndex(e => e.Status, "IX_Feedback_Status");

            entity.HasIndex(e => e.UserId, "IX_Feedback_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProcessedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.FeedbackProcessedByNavigations)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK_Feedback_ProcessedBy");

            entity.HasOne(d => d.User).WithMany(p => p.FeedbackUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feedback_User");
        });

        modelBuilder.Entity<FileAttachment>(entity =>
        {
            entity.HasKey(e => e.FileAttachmentId).HasName("PK__FileAtta__5DABAA53B595C6BC");

            entity.ToTable("FileAttachment");

            entity.HasIndex(e => e.EventId, "IX_FileAttachment_EventId");

            entity.HasIndex(e => e.PostId, "IX_FileAttachment_PostId");

            entity.HasIndex(e => e.SurveyId, "IX_FileAttachment_SurveyId");

            entity.HasIndex(e => e.UserId, "IX_FileAttachment_UserId");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.FileAttachments)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_FileAttachment_Event");

            entity.HasOne(d => d.Post).WithMany(p => p.FileAttachments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_FileAttachment_Post");

            entity.HasOne(d => d.Survey).WithMany(p => p.FileAttachments)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK_FileAttachment_Survey");

            entity.HasOne(d => d.User).WithMany(p => p.FileAttachments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_FileAttachment_User");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__History__4D7B4ABD3B3A57AA");

            entity.ToTable("History");

            entity.HasIndex(e => e.ChangedBy, "IX_History_ChangedBy");

            entity.HasIndex(e => new { e.TableName, e.RecordId }, "IX_History_Table_Record");

            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FieldName).HasMaxLength(100);
            entity.Property(e => e.TableName).HasMaxLength(100);

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.Histories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_History_ChangedBy");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Like__A2922C14B3B2653A");

            entity.ToTable("Like");

            entity.HasIndex(e => e.CommentId, "IX_Like_CommentId");

            entity.HasIndex(e => e.PostId, "IX_Like_PostId");

            entity.HasIndex(e => e.UserId, "IX_Like_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Comment).WithMany(p => p.Likes)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_Like_Comment");

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Like_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Like_User");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__Membersh__92A786796F8CBFB2");

            entity.ToTable("Membership");

            entity.HasIndex(e => e.ClubId, "IX_Membership_ClubId");

            entity.HasIndex(e => e.UserId, "IX_Membership_UserId");

            entity.HasIndex(e => new { e.UserId, e.ClubId }, "IX_Membership_User_Club").IsUnique();

            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LeftAt).HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Club).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membership_Club");

            entity.HasOne(d => d.User).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membership_User");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__C87C0C9C52A4F9B5");

            entity.ToTable("Message");

            entity.HasIndex(e => e.GroupId, "IX_Message_GroupId");

            entity.HasIndex(e => e.ReceiverId, "IX_Message_ReceiverId");

            entity.HasIndex(e => e.SenderId, "IX_Message_SenderId");

            entity.HasIndex(e => e.Status, "IX_Message_Status");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Unread");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK_Message_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_Sender");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E122EFDF6FF");

            entity.ToTable("Notification");

            entity.HasIndex(e => e.IsRead, "IX_Notification_IsRead");

            entity.HasIndex(e => e.UserId, "IX_Notification_UserId");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.NotificationType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB2F413810AD");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.PermissionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__AA126018C26B6268");

            entity.ToTable("Post");

            entity.HasIndex(e => e.ClubId, "IX_Post_ClubId");

            entity.HasIndex(e => e.EventId, "IX_Post_EventId");

            entity.HasIndex(e => e.UserId, "IX_Post_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Club).WithMany(p => p.Posts)
                .HasForeignKey(d => d.ClubId)
                .HasConstraintName("FK_Post_Club");

            entity.HasOne(d => d.Event).WithMany(p => p.Posts)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_Post_Event");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_User");
        });

        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.HasKey(e => e.PostTagId).HasName("PK__PostTag__325724FD10819C3F");

            entity.ToTable("PostTag");

            entity.HasIndex(e => e.PostId, "IX_PostTag_PostId");

            entity.HasIndex(e => new { e.PostId, e.TagId }, "IX_PostTag_Post_Tag").IsUnique();

            entity.HasIndex(e => e.TagId, "IX_PostTag_TagId");

            entity.HasOne(d => d.Post).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostTag_Post");

            entity.HasOne(d => d.Tag).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostTag_Tag");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A85C0A892");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK__RolePerm__120F46BA1F382411");

            entity.HasIndex(e => e.PermissionId, "IX_RolePermissions_PermissionId");

            entity.HasIndex(e => e.RoleId, "IX_RolePermissions_RoleId");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "IX_RolePermissions_Role_Permission").IsUnique();

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Role");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__Setting__54372B1D00F756A3");

            entity.ToTable("Setting");

            entity.HasIndex(e => e.Key, "UQ__Setting__C41E02896E67BD4B").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(500);
        });

        modelBuilder.Entity<Share>(entity =>
        {
            entity.HasKey(e => e.ShareId).HasName("PK__Share__D32A3FEE82C866FD");

            entity.ToTable("Share");

            entity.HasIndex(e => e.PostId, "IX_Share_PostId");

            entity.HasIndex(e => e.UserId, "IX_Share_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Shares)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Share_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Shares)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Share_User");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.SurveyId).HasName("PK__Survey__A5481F7D5FCCB3A7");

            entity.ToTable("Survey");

            entity.HasIndex(e => e.ClubId, "IX_Survey_ClubId");

            entity.HasIndex(e => e.EventId, "IX_Survey_EventId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.SurveyType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Club).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.ClubId)
                .HasConstraintName("FK_Survey_Club");

            entity.HasOne(d => d.Event).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_Survey_Event");
        });

        modelBuilder.Entity<SurveyResult>(entity =>
        {
            entity.HasKey(e => e.SurveyResultId).HasName("PK__SurveyRe__E2A335CC31D945CA");

            entity.ToTable("SurveyResult");

            entity.HasIndex(e => e.SurveyId, "IX_SurveyResult_SurveyId");

            entity.HasIndex(e => e.UserId, "IX_SurveyResult_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyResults)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyResult_Survey");

            entity.HasOne(d => d.User).WithMany(p => p.SurveyResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyResult_User");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tag__657CF9ACCC221B85");

            entity.ToTable("Tag");

            entity.HasIndex(e => e.TagType, "IX_Tag_TagType");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TagName).HasMaxLength(100);
            entity.Property(e => e.TagType).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C8B720FDB");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "IX_User_Email").IsUnique();

            entity.HasIndex(e => e.Username, "IX_User_Username").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsDisabled).HasDefaultValue(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.UserPermissionId).HasName("PK__UserPerm__A90F88B201273E8E");

            entity.HasIndex(e => e.PermissionId, "IX_UserPermissions_PermissionId");

            entity.HasIndex(e => e.UserId, "IX_UserPermissions_UserId");

            entity.HasIndex(e => new { e.UserId, e.PermissionId }, "IX_UserPermissions_User_Permission").IsUnique();

            entity.HasOne(d => d.Permission).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermissions_Permission");

            entity.HasOne(d => d.User).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermissions_User");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A3527D1D855");

            entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");

            entity.HasIndex(e => e.UserId, "IX_UserRoles_UserId");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "IX_UserRoles_User_Role").IsUnique();

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
