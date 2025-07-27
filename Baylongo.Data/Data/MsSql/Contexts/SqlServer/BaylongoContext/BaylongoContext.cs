using System;
using System.Collections.Generic;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using Microsoft.EntityFrameworkCore;

namespace Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;

public partial class BaylongoContext : DbContext
{
    public BaylongoContext()
    {
    }

    public BaylongoContext(DbContextOptions<BaylongoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<ClassAttendance> ClassAttendances { get; set; }

    public virtual DbSet<ContentType> ContentTypes { get; set; }

    public virtual DbSet<DanceCategory> DanceCategories { get; set; }

    public virtual DbSet<DanceLevel> DanceLevels { get; set; }

    public virtual DbSet<DanceType> DanceTypes { get; set; }

    public virtual DbSet<DanceTypeLevel> DanceTypeLevels { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventDanceType> EventDanceTypes { get; set; }

    public virtual DbSet<EventGallery> EventGalleries { get; set; }

    public virtual DbSet<EventParticipant> EventParticipants { get; set; }

    public virtual DbSet<EventPurchase> EventPurchases { get; set; }

    public virtual DbSet<EventStatus> EventStatuses { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OrganizationContentPermission> OrganizationContentPermissions { get; set; }

    public virtual DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }

    public virtual DbSet<OrganizationRole> OrganizationRoles { get; set; }

    public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<ParticipantRole> ParticipantRoles { get; set; }

    public virtual DbSet<ParticipantType> ParticipantTypes { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PurchaseStatus> PurchaseStatuses { get; set; }

    public virtual DbSet<PurchaseToken> PurchaseTokens { get; set; }

    public virtual DbSet<StripeAccount> StripeAccounts { get; set; }

    public virtual DbSet<StripeEvent> StripeEvents { get; set; }

    public virtual DbSet<SystemMaintenance> SystemMaintenances { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDancePreference> UserDancePreferences { get; set; }

    public virtual DbSet<UserOrganization> UserOrganizations { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<VerificationStatus> VerificationStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__Cities__031491A81D0A3700");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityDefault).HasColumnName("city_default");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ClassAttendance>(entity =>
        {
            entity.HasKey(e => e.ClassAttendanceId).HasName("PK__ClassAtt__43514838CDAD8356");

            entity.ToTable("ClassAttendance");

            entity.HasIndex(e => new { e.EventId, e.UserId }, "IX_ClassAttendance_EventId_UserId");

            entity.Property(e => e.AttendanceDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreditsUsed).HasDefaultValue(1);

            entity.HasOne(d => d.Event).WithMany(p => p.ClassAttendances)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassAttendance_Events");

            entity.HasOne(d => d.User).WithMany(p => p.ClassAttendances)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassAttendance_Users");
        });

        modelBuilder.Entity<ContentType>(entity =>
        {
            entity.HasKey(e => e.ContentTypeId).HasName("PK__ContentT__2026064A9B166C41");

            entity.HasIndex(e => e.Name, "UQ_ContentTypes_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<DanceCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__DanceCat__D54EE9B41CFCB777");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Order).HasColumnName("order");
        });

        modelBuilder.Entity<DanceLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__DanceLev__03461643AF53D8A1");

            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Order).HasColumnName("order");
        });

        modelBuilder.Entity<DanceType>(entity =>
        {
            entity.HasKey(e => e.DanceTypeId).HasName("PK__DanceTyp__2769138AD2931F0C");

            entity.Property(e => e.DanceTypeId).HasColumnName("dance_type_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IconUrl)
                .HasMaxLength(255)
                .HasColumnName("icon_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Order).HasColumnName("order");

            entity.HasOne(d => d.Category).WithMany(p => p.DanceTypes)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_DanceTypes_DanceCategories");
        });

        modelBuilder.Entity<DanceTypeLevel>(entity =>
        {
            entity.HasKey(e => new { e.DanceTypeId, e.LevelId }).HasName("PK__DanceTyp__075D72EE7A098A0F");

            entity.Property(e => e.DanceTypeId).HasColumnName("dance_type_id");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.IsBeginnerFriendly).HasColumnName("is_beginner_friendly");

            entity.HasOne(d => d.DanceType).WithMany(p => p.DanceTypeLevels)
                .HasForeignKey(d => d.DanceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanceTypeLevels_DanceTypes");

            entity.HasOne(d => d.Level).WithMany(p => p.DanceTypeLevels)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanceTypeLevels_DanceLevels");
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorLogId).HasName("PK__ErrorLog__D65247C22D12E3C2");

            entity.HasIndex(e => e.Email, "IX_ErrorLogs_Email");

            entity.HasIndex(e => e.ErrorLevel, "IX_ErrorLogs_ErrorLevel");

            entity.HasIndex(e => e.ErrorTime, "IX_ErrorLogs_ErrorTime");

            entity.HasIndex(e => e.Source, "IX_ErrorLogs_Source");

            entity.HasIndex(e => e.UserId, "IX_ErrorLogs_UserId");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ErrorLevel)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ErrorTime).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ExceptionType)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.MachineName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RequestUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StripeAccountId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StripeEventId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.ErrorLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ErrorLogs_Users");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__2370F727B38E1E3C");

            entity.HasIndex(e => e.ContentTypeId, "IX_Events_ContentTypeId");

            entity.HasIndex(e => e.ExpirationDate, "IX_Events_ExpirationDate");

            entity.HasIndex(e => e.StripeAccountId, "IX_Events_StripeAccountId");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("base_price");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.EventStatusId).HasColumnName("event_status_id");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.MainImageUrl)
                .HasMaxLength(255)
                .HasColumnName("main_image_url");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.PaymentLink)
                .HasMaxLength(255)
                .HasColumnName("payment_link");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.PromoEndDate)
                .HasColumnType("datetime")
                .HasColumnName("promo_end_date");
            entity.Property(e => e.PromoStartDate)
                .HasColumnType("datetime")
                .HasColumnName("promo_start_date");
            entity.Property(e => e.PromotionalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("promotional_price");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.StripeAccountId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ContentType).WithMany(p => p.Events)
                .HasForeignKey(d => d.ContentTypeId)
                .HasConstraintName("FK_Events_ContentTypes");

            entity.HasOne(d => d.EventStatus).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_EventStatuses");

            entity.HasOne(d => d.Organization).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Organizations");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Events)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_PaymentMethods");

            entity.HasOne(d => d.User).WithMany(p => p.Events)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Users");
        });

        modelBuilder.Entity<EventDanceType>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.DanceTypeId }).HasName("PK__EventDan__9106661F12F0CD20");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.DanceTypeId).HasColumnName("dance_type_id");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");

            entity.HasOne(d => d.DanceType).WithMany(p => p.EventDanceTypes)
                .HasForeignKey(d => d.DanceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventDanceTypes_DanceTypes");

            entity.HasOne(d => d.Event).WithMany(p => p.EventDanceTypes)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventDanceTypes_Events");
        });

        modelBuilder.Entity<EventGallery>(entity =>
        {
            entity.HasKey(e => e.GalleryId).HasName("PK__EventGal__43D54A71D33ED8E2");

            entity.ToTable("EventGallery");

            entity.Property(e => e.GalleryId).HasColumnName("gallery_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");

            entity.HasOne(d => d.Event).WithMany(p => p.EventGalleries)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventGallery_Events");
        });

        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => e.EventParticipantId).HasName("PK__EventPar__BEDAC2ECBC8F505D");

            entity.Property(e => e.EventParticipantId).HasColumnName("event_participant_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventParticipants_Events");

            entity.HasOne(d => d.Participant).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.ParticipantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventParticipants_Participants");

            entity.HasOne(d => d.Role).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventParticipants_ParticipantRoles");
        });

        modelBuilder.Entity<EventPurchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__EventPur__6B0A6BBEA789684C");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PurchaseDate).HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.EventPurchases)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventPurc__Event__56B3DD81");

            entity.HasOne(d => d.Status).WithMany(p => p.EventPurchases)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventPurc__Statu__589C25F3");

            entity.HasOne(d => d.User).WithMany(p => p.EventPurchases)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventPurc__UserI__57A801BA");
        });

        modelBuilder.Entity<EventStatus>(entity =>
        {
            entity.HasKey(e => e.EventStatusId).HasName("PK__EventSta__A639FDCDC6D78A4B");

            entity.Property(e => e.EventStatusId).HasColumnName("event_status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PK__Organiza__C0B2F432B56192D1");

            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GoogleMapsUrl).HasMaxLength(500);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("logo_url");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.OrganizationTypeId).HasColumnName("organization_type_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerificationStatusId).HasColumnName("verification_status_id");
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("website");

            entity.HasOne(d => d.BaseOrganization).WithMany(p => p.InverseBaseOrganization)
                .HasForeignKey(d => d.BaseOrganizationId)
                .HasConstraintName("FK_Organizations_BaseOrganization");

            entity.HasOne(d => d.City).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Organizations_Cities");

            entity.HasOne(d => d.OrganizationType).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.OrganizationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Organizations_OrganizationTypes");

            entity.HasOne(d => d.User).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Organizat__user___571DF1D5");

            entity.HasOne(d => d.VerificationStatus).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.VerificationStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Organizat__verif__5812160E");
        });

        modelBuilder.Entity<OrganizationContentPermission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Organiza__EFA6FB2F1248D382");

            entity.HasIndex(e => new { e.OrganizationId, e.ContentTypeId }, "UQ_OrganizationContentType").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ContentType).WithMany(p => p.OrganizationContentPermissions)
                .HasForeignKey(d => d.ContentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrganizationContentPermissions_ContentTypes");

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationContentPermissions)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrganizationContentPermissions_Organizations");
        });

        modelBuilder.Entity<OrganizationInvitation>(entity =>
        {
            entity.HasKey(e => e.InvitationId).HasName("PK__Organiza__033C8DCF0C3210B6");

            entity.ToTable("OrganizationInvitation");

            entity.HasIndex(e => e.InvitedUserId, "IX_OrganizationInvitation_InvitedUser");

            entity.HasIndex(e => e.InvitationToken, "IX_OrganizationInvitation_Token");

            entity.Property(e => e.InvitationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.InvitationToken).HasMaxLength(128);
            entity.Property(e => e.InvitedUserEmail)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Purpose)
                .HasMaxLength(50)
                .HasDefaultValue("OrganizationManagement");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.InvitedUser).WithMany(p => p.OrganizationInvitationInvitedUsers)
                .HasForeignKey(d => d.InvitedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrganizationInvitation_InvitedUser");

            entity.HasOne(d => d.InvitingUser).WithMany(p => p.OrganizationInvitationInvitingUsers)
                .HasForeignKey(d => d.InvitingUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrganizationInvitation_InvitingUser");

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationInvitations)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrganizationInvitation_Organization");

            entity.HasOne(d => d.Role).WithMany(p => p.OrganizationInvitations)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrganizationInvitation_Role");
        });

        modelBuilder.Entity<OrganizationRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Organiza__760965CC5A19A929");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CanCreateEvents)
                .HasDefaultValue(false)
                .HasColumnName("can_create_events");
            entity.Property(e => e.CanCurses).HasColumnName("can_curses");
            entity.Property(e => e.CanInviteMembers)
                .HasDefaultValue(false)
                .HasColumnName("can_invite_members");
            entity.Property(e => e.CanManageOrganization)
                .HasDefaultValue(false)
                .HasColumnName("can_manage_organization");
            entity.Property(e => e.CanWorkshops).HasColumnName("can_workshops");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<OrganizationType>(entity =>
        {
            entity.HasIndex(e => e.TypeName, "IX_OrganizationTypes_TypeName").IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.ParticipantId).HasName("PK__Particip__4E037806D48FAB24");

            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .HasColumnName("logo_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ParticipantTypeId).HasColumnName("participant_type_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.SocialMedia)
                .HasMaxLength(255)
                .HasColumnName("social_media");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");

            entity.HasOne(d => d.ParticipantType).WithMany(p => p.Participants)
                .HasForeignKey(d => d.ParticipantTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Participants_ParticipantTypes");
        });

        modelBuilder.Entity<ParticipantRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Particip__760965CCB5C9B25E");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ParticipantType>(entity =>
        {
            entity.HasKey(e => e.ParticipantTypeId).HasName("PK__Particip__A94DCC8B339E5CD3");

            entity.Property(e => e.ParticipantTypeId).HasColumnName("participant_type_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.HasIndex(e => e.ResetCode, "IX_PasswordResetTokens_ResetCode");

            entity.HasIndex(e => e.UserId, "IX_PasswordResetTokens_UserId");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.ResetCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("reset_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PasswordResetTokens_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83F18BEA2E0");

            entity.ToTable("payments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasColumnName("currency");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.ReceiptUrl)
                .HasMaxLength(500)
                .HasColumnName("receipt_url");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.StripePaymentIntentId)
                .HasMaxLength(255)
                .HasColumnName("stripe_payment_intent_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_event");

            entity.HasOne(d => d.Organization).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_org");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_user");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__8A3EA9EB4AF12B94");

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.RequiresOnlinePayment).HasColumnName("requires_online_payment");
        });

        modelBuilder.Entity<PurchaseStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Purchase__C8EE206392D56F33");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PurchaseToken>(entity =>
        {
            entity.HasKey(e => e.PurchaseTokenId).HasName("PK__Purchase__436C0D0715774273");

            entity.HasIndex(e => e.EventPurchaseId, "IX_PurchaseTokens_EventPurchaseId");

            entity.HasIndex(e => e.Token, "IX_PurchaseTokens_Token");

            entity.HasIndex(e => e.Token, "UQ__Purchase__1EB4F8176C56E9A1").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Token).HasMaxLength(50);

            entity.HasOne(d => d.EventPurchase).WithMany(p => p.PurchaseTokens)
                .HasForeignKey(d => d.EventPurchaseId)
                .HasConstraintName("FK_PurchaseTokens_EventPurchases");
        });

        modelBuilder.Entity<StripeAccount>(entity =>
        {
            entity.Property(e => e.AccountType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.OnboardingStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StripeAccountId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Organization).WithMany(p => p.StripeAccounts)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_StripeAccounts_Organizations");

            entity.HasOne(d => d.User).WithMany(p => p.StripeAccounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StripeAccounts_Users");
        });

        modelBuilder.Entity<StripeEvent>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StripeEventId)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SystemMaintenance>(entity =>
        {
            entity.HasKey(e => e.MaintenanceId).HasName("PK__SystemMa__9D754BEA68EC4ADC");

            entity.ToTable("SystemMaintenance");

            entity.Property(e => e.MaintenanceId).HasColumnName("maintenance_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .HasColumnName("message");
            entity.Property(e => e.StartTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("start_time");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SystemMaintenances)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_SystemMaintenance_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F5C9B1B17");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E61648235D7EB").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC5726AB4216A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("profile_picture_url");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registration_date");
            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.VerificationToken)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("verification_token");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Users_Cities");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__user_type__5CD6CB2B");
        });

        modelBuilder.Entity<UserDancePreference>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.DanceTypeId }).HasName("PK__UserDanc__0BC8A637E774E545");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DanceTypeId).HasColumnName("dance_type_id");
            entity.Property(e => e.PreferenceLevel)
                .HasDefaultValue(1)
                .HasColumnName("preference_level");

            entity.HasOne(d => d.DanceType).WithMany(p => p.UserDancePreferences)
                .HasForeignKey(d => d.DanceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDancePreferences_DanceTypes");

            entity.HasOne(d => d.User).WithMany(p => p.UserDancePreferences)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDancePreferences_Users");
        });

        modelBuilder.Entity<UserOrganization>(entity =>
        {
            entity.HasKey(e => e.UserOrgId).HasName("PK__UserOrga__252F28ABB3B3D5B7");

            entity.Property(e => e.UserOrgId).HasColumnName("user_org_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("join_date");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Organization).WithMany(p => p.UserOrganizations)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOrgan__organ__59FA5E80");

            entity.HasOne(d => d.Role).WithMany(p => p.UserOrganizations)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserOrganizations_OrganizationRoles");

            entity.HasOne(d => d.User).WithMany(p => p.UserOrganizations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserOrgan__user___5AEE82B9");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__UserType__2C0005989CA42170");

            entity.HasIndex(e => e.TypeName, "UQ__UserType__543C4FD974058ED5").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.CanJoinEvents)
                .HasDefaultValue(false)
                .HasColumnName("can_join_events");
            entity.Property(e => e.CanPublishAds)
                .HasDefaultValue(false)
                .HasColumnName("can_publish_ads");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.RequiresOrganization)
                .HasDefaultValue(false)
                .HasColumnName("requires_organization");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<VerificationStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Verifica__3683B53113D0DDC0");

            entity.ToTable("VerificationStatus");

            entity.HasIndex(e => e.StatusName, "UQ__Verifica__501B3753A83F0AA6").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
