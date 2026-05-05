-- 1. User (Sinh viên)
CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,         -- Khóa chính, tự tăng
    Username NVARCHAR(50) NOT NULL,               -- Tên đăng nhập
    PasswordHash NVARCHAR(255) NOT NULL,          -- Mật khẩu đã mã hóa
    FullName NVARCHAR(100) NOT NULL,              -- Họ tên
    Email NVARCHAR(100) NOT NULL,                 -- Email
    Phone NVARCHAR(20),                           -- Số điện thoại
    Status NVARCHAR(20) DEFAULT 'Active',         -- Trạng thái tài khoản
    CreatedAt DATETIME DEFAULT GETDATE()          -- Ngày tạo tài khoản


);
ALTER TABLE [User]
ADD IsDisabled BIT DEFAULT 0; -- 0: hoạt động, 1: bị vô hiệu hóa


-- 2. Club (Câu lạc bộ)
CREATE TABLE Club (
    ClubId INT IDENTITY(1,1) PRIMARY KEY,         -- Khóa chính, tự tăng
    ClubName NVARCHAR(100) NOT NULL,              -- Tên CLB
    Description NVARCHAR(500),                    -- Mô tả CLB
    Field NVARCHAR(100),                          -- Lĩnh vực hoạt động
    School NVARCHAR(100),                         -- Trường trực thuộc
    MemberCount INT DEFAULT 0,                    -- Số lượng thành viên
    Status NVARCHAR(20) DEFAULT 'Active',         -- Trạng thái CLB
    CreatedAt DATETIME DEFAULT GETDATE()          -- Ngày tạo CLB
);

-- 3. Membership (Thành viên CLB)
CREATE TABLE Membership (
    MembershipId INT IDENTITY(1,1) PRIMARY KEY,   -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    ClubId INT NOT NULL,                          -- FK đến Club
    Role NVARCHAR(50),                            -- Vai trò trong CLB
    Status NVARCHAR(20) DEFAULT 'Pending',        -- Trạng thái thành viên
    JoinedAt DATETIME DEFAULT GETDATE(),          -- Ngày tham gia
    LeftAt DATETIME,                              -- Ngày rời CLB (nếu có)
    CONSTRAINT FK_Membership_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_Membership_Club FOREIGN KEY (ClubId) REFERENCES Club(ClubId)
);

-- 4. Event (Sự kiện)
CREATE TABLE Event (
    EventId INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính, tự tăng
    ClubId INT NOT NULL,                          -- FK đến Club
    EventName NVARCHAR(100) NOT NULL,             -- Tên sự kiện
    Description NVARCHAR(500),                    -- Mô tả sự kiện
    EventTime DATETIME NOT NULL,                  -- Thời gian diễn ra
    EndDateTime DATETIME,                         -- Thời gian kết thúc sự kiện
    Location NVARCHAR(200),                       -- Địa điểm
    Status NVARCHAR(20) DEFAULT 'Upcoming',       -- Trạng thái sự kiện
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Ngày tạo sự kiện
    ParticipantCount INT DEFAULT 0,               -- Số người đã đăng ký
    MaxParticipants INT NULL,                     -- Số lượng tối đa (null = không giới hạn)
    CONSTRAINT FK_Event_Club FOREIGN KEY (ClubId) REFERENCES Club(ClubId)
);

-- 5. EventRegistration (Đăng ký sự kiện)
CREATE TABLE EventRegistration (
    EventRegistrationId INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    EventId INT NOT NULL,                         -- FK đến Event
    RegistrationStatus NVARCHAR(20) DEFAULT 'Registered', -- Trạng thái đăng ký
    CheckInTime DATETIME,                         -- Thời gian check-in
    CONSTRAINT FK_EventRegistration_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_EventRegistration_Event FOREIGN KEY (EventId) REFERENCES Event(EventId)
);

-- 6. Post (Bài viết)
CREATE TABLE Post (
    PostId INT IDENTITY(1,1) PRIMARY KEY,         -- Khóa chính, tự tăng
    Title NVARCHAR(200) NOT NULL,                 -- Tiêu đề bài viết
    Content NVARCHAR(MAX) NOT NULL,               -- Nội dung bài viết
    ClubId INT,                                   -- FK đến Club (nếu là bài viết CLB)
    EventId INT,                                  -- FK đến Event (nếu là bài viết sự kiện)
    UserId INT NOT NULL,                          -- FK đến User (người đăng)
    PostType NVARCHAR(50),                        -- Loại bài viết (thông báo, sự kiện, v.v.)
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian đăng bài
    CONSTRAINT FK_Post_Club FOREIGN KEY (ClubId) REFERENCES Club(ClubId),
    CONSTRAINT FK_Post_Event FOREIGN KEY (EventId) REFERENCES Event(EventId),
    CONSTRAINT FK_Post_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 7. Comment (Bình luận)
CREATE TABLE Comment (
    CommentId INT IDENTITY(1,1) PRIMARY KEY,      -- Khóa chính, tự tăng
    PostId INT NOT NULL,                          -- FK đến Post
    UserId INT NOT NULL,                          -- FK đến User
    Content NVARCHAR(1000) NOT NULL,              -- Nội dung bình luận
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian bình luận
    CONSTRAINT FK_Comment_Post FOREIGN KEY (PostId) REFERENCES Post(PostId),
    CONSTRAINT FK_Comment_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 8. Like (Lượt thích)
CREATE TABLE [Like] (
    LikeId INT IDENTITY(1,1) PRIMARY KEY,         -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    PostId INT,                                   -- FK đến Post (nếu like bài viết)
    CommentId INT,                                -- FK đến Comment (nếu like bình luận)
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian like
    CONSTRAINT FK_Like_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_Like_Post FOREIGN KEY (PostId) REFERENCES Post(PostId),
    CONSTRAINT FK_Like_Comment FOREIGN KEY (CommentId) REFERENCES Comment(CommentId)
);

-- 8.1 PostReport (Báo cáo bài viết)
CREATE TABLE PostReport (
    PostReportId INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    UserId INT NOT NULL,
    Reason NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    Status NVARCHAR(20) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_PostReport_Post FOREIGN KEY (PostId) REFERENCES Post(PostId),
    CONSTRAINT FK_PostReport_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- Chặn một user report cùng một bài viết nhiều lần
CREATE UNIQUE INDEX UX_PostReport_User_Post ON PostReport(UserId, PostId);

-- 9. Share (Chia sẻ)
CREATE TABLE Share (
    ShareId INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    PostId INT NOT NULL,                          -- FK đến Post
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian chia sẻ
    CONSTRAINT FK_Share_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_Share_Post FOREIGN KEY (PostId) REFERENCES Post(PostId)
);

-- 10. Notification (Thông báo)
CREATE TABLE Notification (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    Content NVARCHAR(500) NOT NULL,               -- Nội dung thông báo
    NotificationType NVARCHAR(50),                -- Loại thông báo
    IsRead BIT DEFAULT 0,                         -- Đã đọc hay chưa
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian gửi thông báo
    CONSTRAINT FK_Notification_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 11. Survey (Khảo sát/Đánh giá)
CREATE TABLE Survey (
    SurveyId INT IDENTITY(1,1) PRIMARY KEY,       -- Khóa chính, tự tăng
    ClubId INT,                                   -- FK đến Club (nếu khảo sát CLB)
    EventId INT,                                  -- FK đến Event (nếu khảo sát sự kiện)
    Title NVARCHAR(200) NOT NULL,                 -- Tiêu đề khảo sát
    Description NVARCHAR(1000),                   -- Mô tả khảo sát
    SurveyType NVARCHAR(50),                      -- Loại khảo sát
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian tạo khảo sát
    CONSTRAINT FK_Survey_Club FOREIGN KEY (ClubId) REFERENCES Club(ClubId),
    CONSTRAINT FK_Survey_Event FOREIGN KEY (EventId) REFERENCES Event(EventId)
);

-- 12. SurveyResult (Kết quả khảo sát)
CREATE TABLE SurveyResult (
    SurveyResultId INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính, tự tăng
    SurveyId INT NOT NULL,                        -- FK đến Survey
    UserId INT NOT NULL,                          -- FK đến User
    Answer NVARCHAR(MAX),                         -- Câu trả lời khảo sát
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian trả lời
    CONSTRAINT FK_SurveyResult_Survey FOREIGN KEY (SurveyId) REFERENCES Survey(SurveyId),
    CONSTRAINT FK_SurveyResult_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 13. Achievement (Thành tích/Huy hiệu)
CREATE TABLE Achievement (
    AchievementId INT IDENTITY(1,1) PRIMARY KEY,  -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    Title NVARCHAR(200) NOT NULL,                 -- Tên thành tích/huy hiệu
    Description NVARCHAR(500),                    -- Mô tả
    Type NVARCHAR(50),                            -- Loại (huy hiệu, chứng nhận, ...)
    ReceivedAt DATETIME DEFAULT GETDATE(),        -- Thời gian nhận
    CONSTRAINT FK_Achievement_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 14. ClubFavorite (CLB yêu thích)
CREATE TABLE ClubFavorite (
    ClubFavoriteId INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    ClubId INT NOT NULL,                          -- FK đến Club
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian yêu thích
    CONSTRAINT FK_ClubFavorite_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_ClubFavorite_Club FOREIGN KEY (ClubId) REFERENCES Club(ClubId)
);

-- 15. AuditLog (Ghi log sửa đổi)
CREATE TABLE AuditLog (
    AuditLogId INT IDENTITY(1,1) PRIMARY KEY,     -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User (ai thao tác)
    TableName NVARCHAR(100) NOT NULL,             -- Tên bảng bị thao tác
    RecordId INT NOT NULL,                        -- Khóa chính của bản ghi bị thao tác
    ActionType NVARCHAR(20) NOT NULL,             -- Loại thao tác (Create, Update, Delete)
    OldValue NVARCHAR(MAX),                       -- Giá trị cũ (nếu có)
    NewValue NVARCHAR(MAX),                       -- Giá trị mới (nếu có)
    ActionTime DATETIME DEFAULT GETDATE(),        -- Thời gian thao tác
    CONSTRAINT FK_AuditLog_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);


-- 16. AuthLog (Logs đăng ký/đăng nhập)
CREATE TABLE AuthLog (
    AuthLogId INT IDENTITY(1,1) PRIMARY KEY,      -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    ActionType NVARCHAR(30) NOT NULL,             -- Loại thao tác (Login, Logout, Register, ...)
    DeviceInfo NVARCHAR(200),                     -- Thông tin thiết bị
    IPAddress NVARCHAR(50),                       -- Địa chỉ IP
    ActionTime DATETIME DEFAULT GETDATE(),        -- Thời gian thực hiện
    Status NVARCHAR(20),                          -- Thành công/thất bại
    CONSTRAINT FK_AuthLog_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 17. Roles (Vai trò)
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,         -- Khóa chính, tự tăng
    RoleName NVARCHAR(50) NOT NULL,               -- Tên vai trò (Admin, Student, ...)
    Description NVARCHAR(200)                     -- Mô tả vai trò
);

-- 18. Permissions (Quyền)
CREATE TABLE Permissions (
    PermissionId INT IDENTITY(1,1) PRIMARY KEY,   -- Khóa chính, tự tăng
    PermissionName NVARCHAR(100) NOT NULL,        -- Tên quyền (Xem, Thêm, Sửa, ...)
    Description NVARCHAR(200)                     -- Mô tả quyền
);

-- 19. UserRoles (Gán vai trò cho user)
CREATE TABLE UserRoles (
    UserRoleId INT IDENTITY(1,1) PRIMARY KEY,     -- Khóa chính, tự tăng
    UserId INT NOT NULL,                          -- FK đến User
    RoleId INT NOT NULL,                          -- FK đến Roles
    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- 20. RolePermissions (Gán quyền cho vai trò)
CREATE TABLE RolePermissions (
    RolePermissionId INT IDENTITY(1,1) PRIMARY KEY,   -- Khóa chính, tự tăng
    RoleId INT NOT NULL,                              -- FK đến Roles
    PermissionId INT NOT NULL,                        -- FK đến Permissions
    CONSTRAINT FK_RolePermissions_Role FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    CONSTRAINT FK_RolePermissions_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId)
);


-- 21. UserPermissions: Gán quyền đặc biệt cho user
CREATE TABLE UserPermissions (
    UserPermissionId INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính, tự tăng
    UserId INT NOT NULL,                            -- FK: User được gán quyền
    PermissionId INT NOT NULL,                      -- FK: Quyền được gán
    CONSTRAINT FK_UserPermissions_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_UserPermissions_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId)
);

-- 22. FileAttachment: Tệp đính kèm cho nhiều loại đối tượng
CREATE TABLE FileAttachment (
    FileAttachmentId INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính, tự tăng
    FileName NVARCHAR(255) NOT NULL,                -- Tên file gốc
    FilePath NVARCHAR(500) NOT NULL,                -- Đường dẫn lưu file
    FileType NVARCHAR(50) NOT NULL,                 -- Loại file (image, pdf, doc, ...)
    FileSize INT,                                   -- Kích thước file (byte)
    PostId INT NULL,                                -- FK: Đính kèm bài viết (nếu có)
    EventId INT NULL,                               -- FK: Đính kèm sự kiện (nếu có)
    SurveyId INT NULL,                              -- FK: Đính kèm khảo sát (nếu có)
    UserId INT NULL,                                -- FK: Đính kèm user (nếu có)
    UploadedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Thời gian upload
    CONSTRAINT FK_FileAttachment_Post FOREIGN KEY (PostId) REFERENCES Post(PostId),
    CONSTRAINT FK_FileAttachment_Event FOREIGN KEY (EventId) REFERENCES Event(EventId),
    CONSTRAINT FK_FileAttachment_Survey FOREIGN KEY (SurveyId) REFERENCES Survey(SurveyId),
    CONSTRAINT FK_FileAttachment_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 23. FAQ & Guide: Câu hỏi thường gặp và hướng dẫn
CREATE TABLE FAQ (
    FAQId INT IDENTITY(1,1) PRIMARY KEY,            -- Khóa chính, tự tăng
    Question NVARCHAR(500) NOT NULL,                -- Câu hỏi
    Answer NVARCHAR(MAX) NOT NULL,                  -- Câu trả lời
    Type NVARCHAR(20) NOT NULL,                     -- Loại: 'FAQ' hoặc 'Guide'
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),  -- Ngày tạo
    UpdatedAt DATETIME NULL,                        -- Ngày cập nhật
    CreatedBy INT NULL,                             -- FK: Người tạo (User)
    CONSTRAINT FK_FAQ_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES [User](UserId)
);

-- 24. Feedback: Góp ý/phản hồi từ user
CREATE TABLE Feedback (
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,       -- Khóa chính, tự tăng
    UserId INT NOT NULL,                            -- FK: User gửi góp ý
    Content NVARCHAR(MAX) NOT NULL,                 -- Nội dung góp ý
    Status NVARCHAR(20) NOT NULL DEFAULT N'Pending',-- Trạng thái xử lý (Pending/Processed)
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),  -- Ngày gửi
    ProcessedAt DATETIME NULL,                      -- Ngày xử lý
    ProcessedBy INT NULL,                           -- FK: Người xử lý (User)
    CONSTRAINT FK_Feedback_User FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_Feedback_ProcessedBy FOREIGN KEY (ProcessedBy) REFERENCES [User](UserId)
);

-- 25. Tag/Category: Thẻ/Phân loại (bảng chính)
CREATE TABLE Tag (
    TagId INT IDENTITY(1,1) PRIMARY KEY,            -- Khóa chính, tự tăng
    TagName NVARCHAR(100) NOT NULL,                 -- Tên thẻ
    TagType NVARCHAR(20) NOT NULL,                  -- Loại: Club/Event/Post
    Description NVARCHAR(255) NULL                  -- Mô tả
);

-- 25.1. ClubTag: Gắn thẻ cho CLB (bảng phụ nhiều-nhiều)
CREATE TABLE ClubTag (
    ClubTagId INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính, tự tăng
    ClubId INT NOT NULL,                            -- FK: CLB
    TagId INT NOT NULL,                             -- FK: Thẻ
    CONSTRAINT FK_ClubTag_Club FOREIGN KEY (ClubId) REFERENCES Club(ClubId),
    CONSTRAINT FK_ClubTag_Tag FOREIGN KEY (TagId) REFERENCES Tag(TagId)
);

-- 25.2. EventTag: Gắn thẻ cho sự kiện (bảng phụ nhiều-nhiều)
CREATE TABLE EventTag (
    EventTagId INT IDENTITY(1,1) PRIMARY KEY,       -- Khóa chính, tự tăng
    EventId INT NOT NULL,                           -- FK: Sự kiện
    TagId INT NOT NULL,                             -- FK: Thẻ
    CONSTRAINT FK_EventTag_Event FOREIGN KEY (EventId) REFERENCES Event(EventId),
    CONSTRAINT FK_EventTag_Tag FOREIGN KEY (TagId) REFERENCES Tag(TagId)
);

-- 25.3. PostTag: Gắn thẻ cho bài viết (bảng phụ nhiều-nhiều)
CREATE TABLE PostTag (
    PostTagId INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính, tự tăng
    PostId INT NOT NULL,                            -- FK: Bài viết
    TagId INT NOT NULL,                             -- FK: Thẻ
    CONSTRAINT FK_PostTag_Post FOREIGN KEY (PostId) REFERENCES Post(PostId),
    CONSTRAINT FK_PostTag_Tag FOREIGN KEY (TagId) REFERENCES Tag(TagId)
);

-- 26. Message/Chat: Tin nhắn cá nhân/nhóm
CREATE TABLE Message (
    MessageId INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính, tự tăng
    SenderId INT NOT NULL,                          -- FK: Người gửi (User)
    ReceiverId INT NULL,                            -- FK: Người nhận (User, nếu chat cá nhân)
    GroupId INT NULL,                               -- FK: Nhóm chat (nếu có, để mở rộng)
    Content NVARCHAR(MAX) NOT NULL,                 -- Nội dung tin nhắn
    SentAt DATETIME NOT NULL DEFAULT GETDATE(),     -- Thời gian gửi
    Status NVARCHAR(20) NOT NULL DEFAULT N'Unread', -- Trạng thái (Unread/Read)
    CONSTRAINT FK_Message_Sender FOREIGN KEY (SenderId) REFERENCES [User](UserId),
    CONSTRAINT FK_Message_Receiver FOREIGN KEY (ReceiverId) REFERENCES [User](UserId)
    -- GroupId: Nếu có bảng GroupChat thì bổ sung FK sau
);

-- 27. Setting/Config: Cấu hình hệ thống
CREATE TABLE Setting (
    SettingId INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính, tự tăng
    [Key] NVARCHAR(100) NOT NULL UNIQUE,            -- Tên cấu hình (unique)
    [Value] NVARCHAR(500) NOT NULL,                 -- Giá trị cấu hình
    Description NVARCHAR(255) NULL,                 -- Mô tả
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()   -- Ngày cập nhật
);

-- 28. History: Lịch sử thao tác chi tiết
CREATE TABLE History (
    HistoryId INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính, tự tăng
    TableName NVARCHAR(100) NOT NULL,               -- Tên bảng bị thay đổi
    RecordId INT NOT NULL,                          -- ID bản ghi bị thay đổi
    FieldName NVARCHAR(100) NOT NULL,               -- Tên trường bị thay đổi
    OldValue NVARCHAR(MAX) NULL,                    -- Giá trị cũ
    NewValue NVARCHAR(MAX) NULL,                    -- Giá trị mới
    ChangedBy INT NOT NULL,                         -- FK: Người thay đổi (User)
    ChangedAt DATETIME NOT NULL DEFAULT GETDATE(),  -- Thời gian thay đổi
    CONSTRAINT FK_History_ChangedBy FOREIGN KEY (ChangedBy) REFERENCES [User](UserId)
);









-- TỐI ƯU TỐC ĐỘ TRUY VẤN DỮ LIỆU 


-- User: Tìm kiếm nhanh theo Username, Email
CREATE UNIQUE INDEX IX_User_Username ON [User](Username);
CREATE UNIQUE INDEX IX_User_Email ON [User](Email);

-- Membership: Truy vấn nhanh theo UserId, ClubId
CREATE INDEX IX_Membership_UserId ON Membership(UserId);
CREATE INDEX IX_Membership_ClubId ON Membership(ClubId);
CREATE UNIQUE INDEX IX_Membership_User_Club ON Membership(UserId, ClubId);

-- Event: Truy vấn nhanh theo ClubId
CREATE INDEX IX_Event_ClubId ON Event(ClubId);

-- EventRegistration: Truy vấn nhanh theo UserId, EventId
CREATE INDEX IX_EventRegistration_UserId ON EventRegistration(UserId);
CREATE INDEX IX_EventRegistration_EventId ON EventRegistration(EventId);
CREATE UNIQUE INDEX IX_EventRegistration_User_Event ON EventRegistration(UserId, EventId);

-- Post: Truy vấn nhanh theo ClubId, EventId, UserId
CREATE INDEX IX_Post_ClubId ON Post(ClubId);
CREATE INDEX IX_Post_EventId ON Post(EventId);
CREATE INDEX IX_Post_UserId ON Post(UserId);

-- Comment: Truy vấn nhanh theo PostId, UserId
CREATE INDEX IX_Comment_PostId ON Comment(PostId);
CREATE INDEX IX_Comment_UserId ON Comment(UserId);

-- Like: Truy vấn nhanh theo UserId, PostId, CommentId
CREATE INDEX IX_Like_UserId ON [Like](UserId);
CREATE INDEX IX_Like_PostId ON [Like](PostId);
CREATE INDEX IX_Like_CommentId ON [Like](CommentId);

-- Share: Truy vấn nhanh theo UserId, PostId
CREATE INDEX IX_Share_UserId ON Share(UserId);
CREATE INDEX IX_Share_PostId ON Share(PostId);

-- Notification: Truy vấn nhanh theo UserId, IsRead
CREATE INDEX IX_Notification_UserId ON Notification(UserId);
CREATE INDEX IX_Notification_IsRead ON Notification(IsRead);

-- Survey: Truy vấn nhanh theo ClubId, EventId
CREATE INDEX IX_Survey_ClubId ON Survey(ClubId);
CREATE INDEX IX_Survey_EventId ON Survey(EventId);

-- SurveyResult: Truy vấn nhanh theo SurveyId, UserId
CREATE INDEX IX_SurveyResult_SurveyId ON SurveyResult(SurveyId);
CREATE INDEX IX_SurveyResult_UserId ON SurveyResult(UserId);

-- Achievement: Truy vấn nhanh theo UserId
CREATE INDEX IX_Achievement_UserId ON Achievement(UserId);

-- ClubFavorite: Truy vấn nhanh theo UserId, ClubId
CREATE INDEX IX_ClubFavorite_UserId ON ClubFavorite(UserId);
CREATE INDEX IX_ClubFavorite_ClubId ON ClubFavorite(ClubId);
CREATE UNIQUE INDEX IX_ClubFavorite_User_Club ON ClubFavorite(UserId, ClubId);

-- AuditLog: Truy vấn nhanh theo UserId, TableName, RecordId
CREATE INDEX IX_AuditLog_UserId ON AuditLog(UserId);
CREATE INDEX IX_AuditLog_Table_Record ON AuditLog(TableName, RecordId);

-- AuthLog: Truy vấn nhanh theo UserId, ActionType
CREATE INDEX IX_AuthLog_UserId ON AuthLog(UserId);
CREATE INDEX IX_AuthLog_ActionType ON AuthLog(ActionType);

-- UserRoles: Truy vấn nhanh theo UserId, RoleId
CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId);
CREATE INDEX IX_UserRoles_RoleId ON UserRoles(RoleId);
CREATE UNIQUE INDEX IX_UserRoles_User_Role ON UserRoles(UserId, RoleId);

-- RolePermissions: Truy vấn nhanh theo RoleId, PermissionId
CREATE INDEX IX_RolePermissions_RoleId ON RolePermissions(RoleId);
CREATE INDEX IX_RolePermissions_PermissionId ON RolePermissions(PermissionId);
CREATE UNIQUE INDEX IX_RolePermissions_Role_Permission ON RolePermissions(RoleId, PermissionId);

-- UserPermissions: Truy vấn nhanh theo UserId, PermissionId
CREATE INDEX IX_UserPermissions_UserId ON UserPermissions(UserId);
CREATE INDEX IX_UserPermissions_PermissionId ON UserPermissions(PermissionId);
CREATE UNIQUE INDEX IX_UserPermissions_User_Permission ON UserPermissions(UserId, PermissionId);

-- FileAttachment: Truy vấn nhanh theo PostId, EventId, SurveyId, UserId
CREATE INDEX IX_FileAttachment_PostId ON FileAttachment(PostId);
CREATE INDEX IX_FileAttachment_EventId ON FileAttachment(EventId);
CREATE INDEX IX_FileAttachment_SurveyId ON FileAttachment(SurveyId);
CREATE INDEX IX_FileAttachment_UserId ON FileAttachment(UserId);

-- FAQ: Truy vấn nhanh theo Type, CreatedBy
CREATE INDEX IX_FAQ_Type ON FAQ(Type);
CREATE INDEX IX_FAQ_CreatedBy ON FAQ(CreatedBy);

-- Feedback: Truy vấn nhanh theo UserId, Status
CREATE INDEX IX_Feedback_UserId ON Feedback(UserId);
CREATE INDEX IX_Feedback_Status ON Feedback(Status);

-- Tag: Truy vấn nhanh theo TagType
CREATE INDEX IX_Tag_TagType ON Tag(TagType);

-- ClubTag, EventTag, PostTag: Truy vấn nhanh theo FK
CREATE INDEX IX_ClubTag_ClubId ON ClubTag(ClubId);
CREATE INDEX IX_ClubTag_TagId ON ClubTag(TagId);
CREATE UNIQUE INDEX IX_ClubTag_Club_Tag ON ClubTag(ClubId, TagId);

CREATE INDEX IX_EventTag_EventId ON EventTag(EventId);
CREATE INDEX IX_EventTag_TagId ON EventTag(TagId);
CREATE UNIQUE INDEX IX_EventTag_Event_Tag ON EventTag(EventId, TagId);

CREATE INDEX IX_PostTag_PostId ON PostTag(PostId);
CREATE INDEX IX_PostTag_TagId ON PostTag(TagId);
CREATE UNIQUE INDEX IX_PostTag_Post_Tag ON PostTag(PostId, TagId);

-- Message: Truy vấn nhanh theo SenderId, ReceiverId, GroupId, Status
CREATE INDEX IX_Message_SenderId ON Message(SenderId);
CREATE INDEX IX_Message_ReceiverId ON Message(ReceiverId);
CREATE INDEX IX_Message_GroupId ON Message(GroupId);
CREATE INDEX IX_Message_Status ON Message(Status);

-- Setting: Truy vấn nhanh theo Key (đã unique)
-- Không cần thêm index vì đã có UNIQUE

-- History: Truy vấn nhanh theo TableName, RecordId, ChangedBy
CREATE INDEX IX_History_Table_Record ON History(TableName, RecordId);
CREATE INDEX IX_History_ChangedBy ON History(ChangedBy);