// SCMS.DomainEntities/Enums/StatusEnums.cs
namespace SCMS.DomainEntities.Enums
{
    public enum ClubStatus
    {
        Active,
        Disabled,
        Pending
    }

    public enum EventStatus
    {
        Upcoming,
        Ongoing,
        Finished,
        Cancelled,
        Pending
    }

    public enum UserStatus
    {
        Active,
        Locked
    }

    public enum PostStatus
    {
        Pending,
        Approved,
        Rejected
    }
}