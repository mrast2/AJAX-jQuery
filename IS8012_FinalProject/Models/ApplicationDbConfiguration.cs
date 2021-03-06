﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IS8012_FinalProject.Models
{
    public class ApplicationDbConfiguration : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!RoleManager.RoleExists("Manager"))
            {
                RoleManager.Create(new IdentityRole("Manager"));
            }

            if (!RoleManager.RoleExists("Employee"))
            {
                RoleManager.Create(new IdentityRole("Employee"));
            }

            CreateUserAndRole("manager@example.com", "Manager", "P@ssw0rd", UserManager, RoleManager);
            CreateUserAndRole("employee1@example.com", "Employee", "P@ssw0rd", UserManager, RoleManager);
            CreateUserAndRole("employee2@example.com", "Employee", "P@ssw0rd", UserManager, RoleManager);
            //TODO: CREATE ADDITIONAL USERS / ROLES HERE

            base.Seed(context);
        }

        private void CreateUserAndRole(string email, string role, string password,
                        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExists(role))
            {
                roleManager.Create(new IdentityRole(role));
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            if (userManager.Create(user, password) == IdentityResult.Success)
            {
                userManager.AddToRole(user.Id, role);
            }
        }
    }
}