using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class Event
{
    public int EventId { get; set; }

    public int OrganizationId { get; set; }

    public int UserId { get; set; }

    public int EventStatusId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal? BasePrice { get; set; }

    public decimal? PromotionalPrice { get; set; }

    public DateTime? PromoStartDate { get; set; }

    public DateTime? PromoEndDate { get; set; }

    public int PaymentMethodId { get; set; }

    public string? PaymentLink { get; set; }

    public string Location { get; set; } = null!;

    public string? Address { get; set; }

    public string Country { get; set; } = null!;

    public string? MainImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? StripeAccountId { get; set; }

    public int? CityId { get; set; }

    public int? ContentTypeId { get; set; }

    public int? Capacity { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? Credits { get; set; }

    public virtual ICollection<ClassAttendance> ClassAttendances { get; set; } = new List<ClassAttendance>();

    public virtual ContentType? ContentType { get; set; }

    public virtual ICollection<EventDanceType> EventDanceTypes { get; set; } = new List<EventDanceType>();

    public virtual ICollection<EventGallery> EventGalleries { get; set; } = new List<EventGallery>();

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<EventPurchase> EventPurchases { get; set; } = new List<EventPurchase>();

    public virtual EventStatus EventStatus { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
