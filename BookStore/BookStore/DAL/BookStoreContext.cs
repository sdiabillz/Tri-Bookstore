using BookStore.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Collections.Generic;
using System.Linq;


namespace BookStore.DAL
{
    public class BookstoreContext : DbContext
    {
        public BookstoreContext()
            : base("BookstoreContext")
        {
            Database.SetInitializer<BookstoreContext>(new BookStoreInitializer());
        }

        public DbSet<User> User { set; get; }
        public DbSet<Audits> Audit { set; get; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //make bookid and user id pimary key in posting
            modelBuilder.Entity<Posting>().HasKey(x => x.PostingID).ToTable("Posting");
            modelBuilder.Entity<Posting>().Property(x => x.PostingID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<UserInstitution>().HasKey(x => new { x.UserID, x.Institution_ID }).ToTable("UserInstitution");
            modelBuilder.Entity<User>().HasMany(b => b.UserInstitutions).WithRequired(p => p.user);
            modelBuilder.Entity<Institution>().HasMany(b => b.UserInstitutions).WithRequired(p => p.institution);

            modelBuilder.Entity<OfficialPosting>().ToTable("OfficialPosting");
            modelBuilder.Entity<OfficialPosting>().Property(x => x.PostingID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Institution>().HasKey(x => x.Institution_ID).ToTable("Institution");

        }

        public DbSet<Institution> Institutions { set; get; }
        public DbSet<Posting> Postings { set; get; }
        public DbSet<OfficialPosting> OfficialPostings { set; get; }
        public DbSet<UserInstitution> UserInstitutions { set; get; }

        internal string GetSecurityLevel(string Email)
        {
            return User.Where(u => u.Email == Email)
            .Select(u => u.Securitylevel)
            .FirstOrDefault();
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<BookstoreContext>(new BookStoreInitializer());

                try
                {
                    using (var context = new BookstoreContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                     WebSecurity.InitializeDatabaseConnection("BookStoreContext", "Users", "userID", "Email", autoCreateTables: true);
                    var roles = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                    var membership = (SimpleMembershipProvider)Membership.Provider;

                    if (!roles.RoleExists("admin"))
                    {
                        roles.CreateRole("admin");
                    }
                    if (!roles.RoleExists("general"))
                    {
                        roles.CreateRole("general");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}