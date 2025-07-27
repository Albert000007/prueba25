using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baylongo.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BaylongoContext context)
        {
            context.Database.EnsureCreated();

            if (!context.UserTypes.Any())
            {
                var userTypes = new UserType[]
                {
                new UserType { TypeId = 1, TypeName = "Admin", Description = "Administrador del sistema", CanPublishAds = true, CanJoinEvents = true },
                new UserType { TypeId = 2, TypeName = "Organizer", Description = "Organizador de eventos", CanPublishAds = true, CanJoinEvents = true, RequiresOrganization = true },
                new UserType { TypeId = 3, TypeName = "Regular", Description = "Usuario regular", CanJoinEvents = true }
                };

                context.UserTypes.AddRange(userTypes);
                context.SaveChanges();
            }
        }
    }
}
