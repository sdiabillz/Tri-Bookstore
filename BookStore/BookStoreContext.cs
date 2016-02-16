using BookStore.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace BookStore.DAL
{
    public class BookstoreContext : DbContext
    {
        public BookstoreContext()
            : base("BookstoreContext")
        {
            Database.SetInitializer<BookstoreContext>(new MyDbInitializer());
        }

        public DbSet<Book> Books { set; get; }
        public DbSet<User> Users { set; get; }

        internal string GetSecurityLevel(string Email)
        {
            return Users.Where(u => u.email == Email)
            .Select(u => u.Securitylevel.ToString())
            .FirstOrDefault();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().HasKey(x => x.Book_ID);
            modelBuilder.Entity<Book>().Property(x => x.Book_ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Book>().ToTable("Book");
            //make bookid and user id pimary key in posting
            modelBuilder.Entity<Posting>().HasKey(x => new { x.Book_ID, x.user_ID }).ToTable("Posting");
            //create a table to search postings by institution
            modelBuilder.Entity<PostandInstitution>().HasKey(x => new { x.Book_ID, x.user_ID, x.Institution_ID }).ToTable("PostandInstitution");
            // an insttution may have multiple postings
            modelBuilder.Entity<Institution>().HasMany(b => b.PostandInstitutions).WithRequired(p => p.institution);

            modelBuilder.Entity<OfficialBook>().ToTable("OfficialBook");

            modelBuilder.Entity<Institution>().HasKey(x => x.Institution_ID).ToTable("Institution");

        }

        public DbSet<OfficialBook> OfficialBooks { set; get; }
        public DbSet<Institution> Institutions { set; get; }
        public DbSet<Posting> Postings { set; get; }
        //public DbSet<PostandInstitution> PostandInstitutions { set; get; }
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
                Database.SetInitializer<BookstoreContext>(new MyDbInitializer());

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

                   // WebSecurity.InitializeDatabaseConnection(ConfigurationManager.ConnectionStrings["BookStoreContext"].Name, "users", "userID", "email", autoCreateTables: true);
                    var roles = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                    var membership = (SimpleMembershipProvider)Membership.Provider;

                    if (!roles.RoleExists("1"))
                    {
                        roles.CreateRole("1");
                    }
                    if (!roles.RoleExists("0"))
                    {
                        roles.CreateRole("0");
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