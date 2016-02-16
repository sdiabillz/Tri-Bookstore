using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BookStore.Models;
using BookStore.Controllers;
using System.Web.Helpers;

namespace BookStore.DAL
{
    public class BookStoreInitializer : CreateDatabaseIfNotExists<BookstoreContext>
     {
        protected override void Seed(BookstoreContext context)
        {
            var Laurier = new Institution { Institution_ID = 1, Name = "Wilfred Laurier University" };
            var Conestoga = new Institution { Institution_ID = 2, Name = "Conestoga College" };
            var Waterloo = new Institution { Institution_ID = 3, Name = "University of Waterloo" };
            context.Institutions.Add(Laurier);
            context.Institutions.Add(Conestoga);
            context.Institutions.Add(Waterloo);

            string pass = Crypto.HashPassword("password");
          //  Byte[] a = AccountController.EncryptPassword(pass);

            var user = new User { UserID = 1, pword = pass, Securitylevel = "admin", Email = "sdiaby-cc@conestogac.on.ca" };
            var user2 = new User { UserID = 2, pword = pass, Securitylevel = "general", Email = "mooloo-cc@conestogac.on.ca" };

            context.User.Add(user);
            context.User.Add(user2);
            context.SaveChanges();
            //base.Seed(context);
        }
     }

    //// but while debugging, when you want a clean and seeded DB, use:
    //public class MyDbInitializer : DropCreateDatabaseAlways<BookstoreContext>
    //{
    //    protected override void Seed(BookstoreContext context)
    //    {
    //        // create 1 user to seed the database
    //        context.Users.Add(new User { email = "admin2@mass.com", pword = "1234", level = 1 });

    //        base.Seed(context);
    //    }
    //}
}