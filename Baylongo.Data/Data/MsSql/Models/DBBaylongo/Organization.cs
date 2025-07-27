using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class Organization
{
    public int OrganizationId { get; set; }

    public int UserId { get; set; }

    public int OrganizationTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    public string? Website { get; set; }

    public string Phone { get; set; } = null!;

    public int VerificationStatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsBaseOrganization { get; set; }

    public int? BaseOrganizationId { get; set; }

    public int? CityId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? GoogleMapsUrl { get; set; }

    public virtual Organization? BaseOrganization { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Organization> InverseBaseOrganization { get; set; } = new List<Organization>();

    public virtual ICollection<OrganizationContentPermission> OrganizationContentPermissions { get; set; } = new List<OrganizationContentPermission>();

    public virtual ICollection<OrganizationInvitation> OrganizationInvitations { get; set; } = new List<OrganizationInvitation>();

    public virtual OrganizationType OrganizationType { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<StripeAccount> StripeAccounts { get; set; } = new List<StripeAccount>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();

    public virtual VerificationStatus VerificationStatus { get; set; } = null!;
}
