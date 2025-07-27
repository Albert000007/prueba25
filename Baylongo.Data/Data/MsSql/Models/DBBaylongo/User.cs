using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public int UserTypeId { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool? IsActive { get; set; }

    public string? VerificationToken { get; set; }

    public bool? IsVerified { get; set; }

    public int? CityId { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<ClassAttendance> ClassAttendances { get; set; } = new List<ClassAttendance>();

    public virtual ICollection<ErrorLog> ErrorLogs { get; set; } = new List<ErrorLog>();

    public virtual ICollection<EventPurchase> EventPurchases { get; set; } = new List<EventPurchase>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<OrganizationInvitation> OrganizationInvitationInvitedUsers { get; set; } = new List<OrganizationInvitation>();

    public virtual ICollection<OrganizationInvitation> OrganizationInvitationInvitingUsers { get; set; } = new List<OrganizationInvitation>();

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<StripeAccount> StripeAccounts { get; set; } = new List<StripeAccount>();

    public virtual ICollection<SystemMaintenance> SystemMaintenances { get; set; } = new List<SystemMaintenance>();

    public virtual ICollection<UserDancePreference> UserDancePreferences { get; set; } = new List<UserDancePreference>();

    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();

    public virtual UserType UserType { get; set; } = null!;
}
