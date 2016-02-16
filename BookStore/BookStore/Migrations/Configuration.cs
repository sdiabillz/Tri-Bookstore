namespace BookStore.Migrations
{
    using BookStore.Controllers;
    using BookStore.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<BookStore.DAL.BookstoreContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BookStore.DAL.BookstoreContext context)
        {
            //seed thes values on initial migration
            var schools = new List<Institution>
            {
               new Institution { Institution_ID = 1, Name = "Wilfred Laurier University" },
                new Institution { Institution_ID = 2, Name = "Conestoga College" },
                 new Institution { Institution_ID = 3, Name = "University of Waterloo" }
            };
            schools.ForEach(s => context.Institutions.AddOrUpdate(p => p.Institution_ID, s));
            context.SaveChanges();

            string a = Crypto.HashPassword("password");
            var users = new List<User>
            {
                new User { UserID = 1, pword = a, Securitylevel = "admin", Email = "sdiaby-cc@conestogac.on.ca" },
                new User { UserID = 2, pword = a, Securitylevel = "general", Email = "mooloo-cc@conestogac.on.ca" }
            };
            users.ForEach(s => context.User.AddOrUpdate(p => p.UserID, s));
            context.SaveChanges();

            //var postings = new List<Posting>
            //{
            //   new Posting { Institution_ID = 1, UserID = 1, Posting_Date = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(3), PostTitle = "First Post",  PostDescription=  "A book 1",  price = 20, BookTitle = "Book 1", img = "", author= "Sekou", PostingID = 1 },
            //    new Posting { Institution_ID = 2, UserID = 1,  Posting_Date = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(3), PostTitle = "Second Post", PostDescription=  "A book 2",  price = 21, img = "",BookTitle = "Book 2", author= "Diaby",PostingID = 2 },
            //     new Posting { Institution_ID = 3, UserID = 1,  Posting_Date = DateTime.Now,ExpiryDate = DateTime.Now.AddMonths(3), PostTitle = "Third Post",  PostDescription=  "A book 3",  price = 22, img = "",BookTitle = "Book 2", author= "Sekou", PostingID = 3 }
            //};
          //  postings.ForEach(s => context.Postings.AddOrUpdate(p => p.PostingID, s));
            context.SaveChanges();

        }
    }
}
