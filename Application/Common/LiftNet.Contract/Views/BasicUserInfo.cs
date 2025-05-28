using LiftNet.Domain.Enums;

public class BasicUserInfo
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Avatar { get; set; }
    public LiftNetRoleEnum Role { get; set; }
} 