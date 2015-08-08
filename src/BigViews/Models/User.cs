using System.Collections.Generic;
using Microsoft.AspNet.Mvc.Rendering;

namespace BigViews
{
    public class User
    {
        public static readonly List<User> StaticUsers;

        static User()
        {
            var pastJobs = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                pastJobs.Add("hello");
                pastJobs.Add("world");
            }

            StaticUsers = new List<User>();
            for (var i = 0; i < 50; i++)
            {
                StaticUsers.Add(new User
                {
                    FirstName = "Simple",
                    LastName = "User",
                    HomePhone = "81275-912854-1",
                    Mobile = "18295710923",
                    MailId = "something@something.com",
                    CurrentJob = "Some Random Job",
                    PastJobs = new SelectList(pastJobs)
                });
            }
        }

        public User()
        {
            PastJobs = new List<SelectListItem>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string HomePhone { get; set; }

        public string Mobile { get; set; }

        public string MailId { get; set; }

        public string CurrentJob { get; set; }

        public IEnumerable<SelectListItem> PastJobs { get; set; }
    }
}
