﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace BigViews
{
    public class HomeController : Controller
    {
        private static Dictionary<string, User> _users = new Dictionary<string, User>();

        static HomeController()
        {
            // Create a few unique users.
            var jobIndex = 0;
            var pastJobCount = 0;
            for (var i = 0; i < 10; i++)
            {
                var user = new User
                {
                    CurrentJob = BigViews.User.AvailableJobs[jobIndex],
                    FirstName = $"Joey { i }",
                    LastName = "Brown",
                    HomePhone = $"425-555-{ i }{ i }{ i }{ i }",
                    Mobile = $"206-555-{ i }{ i }{ i }{ i }",
                    MailId = $"Joey{ i }@contoso.com",
                };
                jobIndex = (jobIndex + 1) % BigViews.User.AvailableJobs.Count;

                for (var j = 0; j < pastJobCount; j++)
                {
                    user.PastJobs.Add(BigViews.User.AvailableJobs[jobIndex]);
                    jobIndex = (jobIndex + 1) % BigViews.User.AvailableJobs.Count;
                }

                pastJobCount = (pastJobCount + 1) % 3;
                _users.Add(user.MailId, user);
            }
        }

        public IActionResult Index()
        {
            // In a more realistic scenario, may have many more users. But would likely display details of only 10 or
            // so at once.
            return View(_users.Values.ToArray());
        }

        public IActionResult IndexWithStaticOptions()
        {
            // In a more realistic scenario, may have many more users. But would likely display details of only 10 or
            // so at once.
            return View(_users.Values.ToArray());
        }

        public IActionResult IndexWithTagHelpers()
        {
            // In a more realistic scenario, may have many more users. But would likely display details of only 10 or
            // so at once.
            return View(_users.Values.ToArray());
        }

        public IActionResult Edit(string id)
        {
            User user;
            _users.TryGetValue(id, out user);

            return new ContentResult
            {
                Content = user?.FirstName,
            };
        }
    }
}
