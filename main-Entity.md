1. User (Sinh viên)
        Chức năng: Quản lý tài khoản, đăng nhập, profile, lịch sử hoạt động.
        Lưu: Thông tin cá nhân, tài khoản, mật khẩu, trạng thái, vai trò.
        Liên kết:
        Khóa chính: UserId
        Khóa phụ: Không
        Liên kết: Membership, Club, Event, Post, Comment, Notification, SurveyResult, Achievement


2. Club (Câu lạc bộ)
        Chức năng: Quản lý danh sách CLB, thông tin, thành viên, bài viết.
        Lưu: Tên, mô tả, lĩnh vực, trường, số lượng thành viên, trạng thái.
        Liên kết:
        Khóa chính: ClubId
        Khóa phụ: Không
        Liên kết: Membership, Event, Post, Survey, ClubFavorite


3. Membership (Thành viên CLB)
        Chức năng: Quản lý thành viên, phê duyệt, lịch sử tham gia CLB.
        Lưu: UserId, ClubId, vai trò, trạng thái, ngày tham gia, ngày rời.
        Liên kết:
        Khóa chính: MembershipId
        Khóa phụ: UserId (User), ClubId (Club)
        Liên kết: User, Club


4. Event (Sự kiện)
        Chức năng: Quản lý sự kiện, đăng ký, check-in, lịch sử tham gia.
        Lưu: Tên, mô tả, thời gian, địa điểm, ClubId, trạng thái.
        Liên kết:
        Khóa chính: EventId
        Khóa phụ: ClubId (Club)
        Liên kết: EventRegistration, Survey, Post


5. EventRegistration (Đăng ký sự kiện)
        Chức năng: Quản lý đăng ký, check-in, lịch sử tham gia sự kiện.
        Lưu: UserId, EventId, trạng thái đăng ký, thời gian check-in.
        Liên kết:
        Khóa chính: EventRegistrationId
        Khóa phụ: UserId (User), EventId (Event)
        Liên kết: User, Event


6. Post (Bài viết)
        Chức năng: Quản lý bài viết, thông báo, tương tác (like, comment, share).
        Lưu: Tiêu đề, nội dung, ClubId/EventId, UserId (người đăng), loại bài viết, thời gian.
        Liên kết:
        Khóa chính: PostId
        Khóa phụ: ClubId (Club), EventId (Event, nếu là bài viết sự kiện), UserId (User)
        Liên kết: Comment, Like, Share


7. Comment (Bình luận)
        Chức năng: Bình luận bài viết, tương tác.
        Lưu: Nội dung, PostId, UserId, thời gian.
        Liên kết:
        Khóa chính: CommentId
        Khóa phụ: PostId (Post), UserId (User)
        Liên kết: Post, User


8. Like (Lượt thích)
        Chức năng: Like bài viết, bình luận.
        Lưu: UserId, PostId/CommentId, thời gian.
        Liên kết:
        Khóa chính: LikeId
        Khóa phụ: UserId (User), PostId (Post) hoặc CommentId (Comment)
        Liên kết: User, Post/Comment


9. Share (Chia sẻ)
        Chức năng: Chia sẻ bài viết.
        Lưu: UserId, PostId, thời gian.
        Liên kết:
        Khóa chính: ShareId
        Khóa phụ: UserId (User), PostId (Post)
        Liên kết: User, Post


10. Notification (Thông báo)
        Chức năng: Gửi thông báo cho user về CLB, sự kiện, hệ thống.
        Lưu: UserId, nội dung, loại thông báo, trạng thái đã đọc, thời gian.
        Liên kết:
        Khóa chính: NotificationId
        Khóa phụ: UserId (User)
        Liên kết: User


11. Survey (Khảo sát/Đánh giá)
        Chức năng: Quản lý khảo sát, đánh giá CLB/sự kiện.
        Lưu: ClubId/EventId, tiêu đề, mô tả, thời gian, loại khảo sát.
        Liên kết:
        Khóa chính: SurveyId
        Khóa phụ: ClubId (Club), EventId (Event)
        Liên kết: SurveyResult


12. SurveyResult (Kết quả khảo sát)
        Chức năng: Lưu kết quả khảo sát của user.
        Lưu: SurveyId, UserId, câu trả lời, thời gian.
        Liên kết:
        Khóa chính: SurveyResultId
        Khóa phụ: SurveyId (Survey), UserId (User)
        Liên kết: Survey, User


13. Achievement (Thành tích/Huy hiệu)
        Chức năng: Quản lý thành tích, huy hiệu, chứng nhận của user.
        Lưu: UserId, tên thành tích, mô tả, loại, thời gian nhận.
        Liên kết:
        Khóa chính: AchievementId
        Khóa phụ: UserId (User)
        Liên kết: User


14. ClubFavorite (CLB yêu thích)
        Chức năng: User theo dõi/yêu thích CLB.
        Lưu: UserId, ClubId, thời gian.
        Liên kết:
        Khóa chính: ClubFavoriteId
        Khóa phụ: UserId (User), ClubId (Club)
        Liên kết: User, Club


 15. AuditLog (Ghi log sửa đổi)
        Chức năng: Ghi lại mọi thao tác thêm, sửa, xoá dữ liệu trên các bảng quan trọng (User, Club, Event, Post, v.v.).
        Lưu:
        AuditLogId (PK)
        UserId (FK, User) — ai thực hiện thao tác
        TableName — tên bảng bị thao tác
        RecordId — khoá chính của bản ghi bị thao tác
        ActionType — loại thao tác (Create, Update, Delete)
        OldValue — giá trị cũ (nếu có)
        NewValue — giá trị mới (nếu có)
        ActionTime — thời gian thực hiện
        Liên kết:
        UserId (User)
        Có thể liên kết logic với mọi bảng nghiệp vụ qua TableName + RecordId


16. AuthLog (Logs đăng ký/đăng nhập)
        Chức năng: Ghi lại lịch sử đăng ký, đăng nhập, đăng xuất, quên mật khẩu, đổi mật khẩu.
        Lưu:
        AuthLogId (PK)
        UserId (FK, User)
        ActionType — loại thao tác (Login, Logout, Register, ForgotPassword, ChangePassword)
        DeviceInfo — thông tin thiết bị
        IPAddress — địa chỉ IP
        ActionTime — thời gian thực hiện
        Status — thành công/thất bại
        Liên kết:
        UserId (User)



17. Roles (Vai trò)
    Chức năng: Quản lý các vai trò trong hệ thống (Admin, Student, ClubLeader, v.v.)
    Lưu:
    RoleId (PK)
    RoleName
    Description


18. Permissions (Quyền)
    Chức năng: Quản lý các quyền thao tác (Xem, Thêm, Sửa, Xoá, Phê duyệt, v.v.)
    Lưu:
    PermissionId (PK)
    PermissionName
    Description


19. UserRoles (Gán vai trò cho user)
    Chức năng: Gán nhiều vai trò cho một user.
    Lưu:
    UserRoleId (PK)
    UserId (FK, User)
    RoleId (FK, Roles)


20. RolePermissions (Gán quyền cho vai trò)
    Chức năng: Gán nhiều quyền cho một vai trò.
    Lưu:
    RolePermissionId (PK)
    RoleId (FK, Roles)
    PermissionId (FK, Permissions)


21. UserPermissions (Gán quyền đặc biệt cho user)
    Chức năng: Gán quyền trực tiếp cho user (ngoài quyền theo vai trò).
    Lưu:
    UserPermissionId (PK)
    UserId (FK, User)
    PermissionId (FK, Permissions)


    PHẦN MƯỞ RỘNG 


22.FileAttachment (Tệp đính kèm):
        Lưu trữ file (ảnh, tài liệu) cho bài viết, sự kiện, khảo sát, v.v.
        Liên kết: PostId, EventId, SurveyId, UserId (tuỳ loại file).


23. FAQ (Câu hỏi thường gặp) & Guide (Hướng dẫn):
        Quản lý nội dung hỗ trợ, tài liệu hướng dẫn cho user.

24. Feedback (Góp ý/phản hồi):
        Lưu góp ý, phản hồi từ user gửi cho hệ thống hoặc ban quản trị.


25. Tag/Category (Thẻ/Phân loại):
        Gắn thẻ cho CLB, sự kiện, bài viết để tìm kiếm/lọc nâng cao.


26. Message/Chat (Tin nhắn):
        Nếu muốn phát triển tính năng nhắn tin trực tiếp giữa user hoặc nhóm.


27. Setting/Config (Cấu hình hệ thống):
        Lưu các thiết lập động cho hệ thống (ví dụ: cấu hình email, thông báo, v.v.).


28. History (Lịch sử thao tác chi tiết):
        Nếu muốn lưu lịch sử thay đổi chi tiết cho từng bảng nghiệp vụ (ngoài AuditLog tổng quát).






       relationship
1. FK
Membership: UserId → Users, ClubId → Club
Event: ClubId → Club
EventRegistration: UserId → Users, EventId → Event
Post: ClubId → Club, EventId → Event, UserId → Users
Comment: PostId → Post, UserId → Users
Like: UserId → Users, PostId → Post, CommentId → Comment
Share: UserId → Users, PostId → Post
Notification: UserId → Users
Survey: ClubId → Club, EventId → Event
SurveyResult: SurveyId → Survey, UserId → Users
Achievement: UserId → Users
ClubFavorite: UserId → Users, ClubId → Club
AuditLog: UserId → Users
AuthLog: UserId → Users
UserRoles: UserId → Users, RoleId → Roles
RolePermissions: RoleId → Roles, PermissionId → Permissions
UserPermissions: UserId → Users, PermissionId → Permissions
FileAttachment: PostId → Post, EventId → Event, SurveyId → Survey, UserId → Users
FAQ: CreatedBy → Users
Feedback: UserId → Users, ProcessedBy → Users
ClubTag: ClubId → Club, TagId → Tag
EventTag: EventId → Event, TagId → Tag
PostTag: PostId → Post, TagId → Tag
Message: SenderId → Users, ReceiverId → Users
History: ChangedBy → Users


2. Các bảng không cần khóa phụ (bảng chính/master):
Users: Bảng gốc, không cần FK
Club: Bảng gốc, không cần FK
Roles: Bảng gốc, không cần FK
Permissions: Bảng gốc, không cần FK
Tag: Bảng gốc, không cần FK
Setting: Bảng cấu hình, không cần FK