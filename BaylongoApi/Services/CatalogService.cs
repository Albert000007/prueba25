using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Catalogs;
using BaylongoApi.DTOs.City;
using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Organizations;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaylongoApi.Services
{
    public class CatalogService(BaylongoContext context) : ICatalogService
    {
        public async Task<IEnumerable<CatalogDto>> GetAllCatalogs()
        {
            var organizationTypes = await GetOrganizationTypes();
            var verificationStatuses = await GetVerificationStatuses();
            var userTypes = await GetUserTypes();
            var eventStatuses = await GetEventStatuses();
            var paymentMethods = await GetPaymentMethods();
            var participantTypes = await GetParticipantTypes();
            var participantRoles = await GetParticipantRoles();
            var danceTypes = await GetDanceTypes(); 
            var cities = await GetCities(); 


            return new List<CatalogDto>
            {
                new CatalogDto {
                    CatalogName = "OrganizationTypes",
                    Items = organizationTypes
                },
                new CatalogDto {
                    CatalogName = "VerificationStatus",
                    Items = verificationStatuses
                },
                new CatalogDto {
                    CatalogName = "UserTypes",
                    Items = userTypes
                },
                new CatalogDto {
                    CatalogName = "EventStatus",
                    Items = eventStatuses
                },
                new CatalogDto {
                    CatalogName = "PaymentMethod",
                    Items = paymentMethods
                },
                new CatalogDto {
                    CatalogName = "ParticipantType",
                    Items = participantTypes
                },
                new CatalogDto {
                    CatalogName = "ParticipantRole",
                    Items = participantRoles
                },
                new CatalogDto {
                    CatalogName = "DanceTypes",
                    Items = danceTypes
                },
                new CatalogDto {
                    CatalogName = "Cities",
                    Items = cities
                }
            };
        }

        public async Task<IEnumerable<OrganizationTypeDto>> GetOrganizationTypes()
        {
            return await context.OrganizationTypes
               .Where(ot => ot.IsActive)
               .Select(ot => new OrganizationTypeDto
               {
                   Id = ot.OrganizationTypeId,
                   Name = ot.TypeName,
                   Description = ot.Description
               })
               .ToListAsync();
        }

        public async Task<IEnumerable<UserTypeDto>> GetUserTypes()
        {
            return await context.UserTypes
              .Where(ut => ut.IsActive == true)
              .Select(ut => new UserTypeDto
              {
                  Id = ut.TypeId,
                  Name = ut.TypeName,
                  Description = ut.Description,
                  CanPublishAds = ut.CanPublishAds,
                  CanJoinEvents = ut.CanJoinEvents,
                  RequiresOrganization = ut.RequiresOrganization
              })
              .ToListAsync();
        }

        public async Task<IEnumerable<EventStatusDto>> GetEventStatuses()
        {
            return await context.EventStatuses
              .Where(ut => ut.IsActive == true)
              .Select(ut => new EventStatusDto
              {
                  EventStatusId = ut.EventStatusId,
                  Description = ut.Description,
                  Name = ut.Name,
              })
              .ToListAsync();
        }
        public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethods()
        {
            return await context.PaymentMethods
              .Where(ut => ut.IsActive == true)
              .Select(ut => new PaymentMethodDto
              {
                  PaymentMethodId = ut.PaymentMethodId,
                  Description = ut.Description,
                  Name = ut.Name,
              })
              .ToListAsync();
        }
        public async Task<IEnumerable<ParticipantTypeDto>> GetParticipantTypes()
        {
            return await context.ParticipantTypes
              .Where(ut => ut.IsActive == true)
              .Select(ut => new ParticipantTypeDto
              {
                  ParticipantTypeId = ut.ParticipantTypeId,
                  Description = ut.Description,
                  Name = ut.Name,
              })
              .ToListAsync();
        }
        public async Task<IEnumerable<ParticipantRoleDto>> GetParticipantRoles()
        {
            return await context.ParticipantRoles
              .Where(ut => ut.IsActive == true)
              .Select(ut => new ParticipantRoleDto
              {
                  RoleId = ut.RoleId,
                  Description = ut.Description,
                  Name = ut.Name,
              })
              .ToListAsync();
        }
        public async Task<IEnumerable<VerificationStatusDto>> GetVerificationStatuses()
        {
            return await context.VerificationStatuses
               .Where(vs => vs.IsActive == true)
               .Select(vs => new VerificationStatusDto
               {
                   Id = vs.StatusId,
                   Name = vs.StatusName,
                   Description = vs.Description
               })
               .ToListAsync();
        }

        public async Task<IEnumerable<DanceTypeDto>> GetDanceTypes()
        {
            return await context.DanceTypes
                .Where(dt => dt.IsActive)
                .Select(dt => new DanceTypeDto
                {
                    DanceTypeId = dt.DanceTypeId,
                    Name = dt.Name,
                    Description = dt.Description,
                    IconUrl = dt.IconUrl
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CityDto>> GetCities()
        {
            return await context.Cities
        .Where(c => c.IsActive)
        .Select(c => new CityDto
        {
            Id = c.CityId,
            Name = c.Name,
            Country = c.Country,
            Default = c.CityDefault
        })
        .ToListAsync();
        }

        public async Task<IEnumerable<DanceTypeWithLevelsDto>> GetDanceTypesWithLevels()
        {
            return null;
            //return await context.DanceTypes
            //      .Include(dt => dt.DanceTypeLevels)
            //          .ThenInclude(dtl => dtl.DanceLevel)
            //      .Where(dt => dt.IsActive)
            //      .Select(dt => new DanceTypeWithLevelsDto
            //      {
            //          DanceTypeId = dt.DanceTypeId,
            //          Name = dt.Name,
            //          Description = dt.Description,
            //          IconUrl = dt.IconUrl,
            //          AvailableLevels = dt.DanceTypeLevels
            //              .Where(dtl => dtl.DanceLevel.IsActive) // Cambiado a propiedad del modelo
            //              .OrderBy(dtl => dtl.DanceLevel.Order)  // Cambiado a propiedad del modelo
            //              .Select(dtl => new DanceLevelDto
            //              {
            //                  LevelId = dtl.DanceLevel.LevelId,  // Cambiado a propiedad del modelo
            //                  Name = dtl.DanceLevel.Name,
            //                  Description = dtl.DanceLevel.Description,
            //                  IsBeginnerFriendly = dtl.IsBeginnerFriendly
            //              })
            //              .ToList()
            //      })
            //      .ToListAsync();
        }
    }
}
