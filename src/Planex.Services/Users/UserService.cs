﻿namespace Planex.Services.Users
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Planex.Data.Common;
    using Planex.Data.Models;

    public class UserService : IUserService
    {
        private IRepository<Message, int> messages;

        private RoleManager<IdentityRole> roleManager;

        private UserManager<User> userManager;

        private IRepository<User, string> users;

        public UserService(DbContext context, IRepository<User> users, IRepository<Message, int> messages)
        {
            this.users = users;
            this.messages = messages;
            this.userManager = new UserManager<User>(new UserStore<User>(context));
            this.roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public void Add(User user, string role)
        {
            user.UserName = user.Email;
            user.CreatedOn = DateTime.Now;
            this.userManager.Create(user, "changeme");
            user.IntId = int.Parse(user.Id.GetHashCode().ToString());
            user.ResetPassword = true;
            this.users.Update(user);
            this.userManager.AddToRole(user.Id, role);
        }

        public void Delete(string id)
        {
            this.users.Delete(id);

            var messagesDb = this.messages.All().Where(x => x.FromId == id || x.ToId == id).ToList();
            foreach (var message in messagesDb)
            {
                this.messages.Delete(message);
            }
        }

        public IQueryable<User> GetAll()
        {
            return this.users.All();
        }

        public IQueryable<User> GetAllByRole(string role)
        {
            var r = this.roleManager.FindByName(role);
            var userIds = r.Users.Select(u => u.UserId);
            var res = this.users.All().Where(u => userIds.Contains(u.Id));
            return res;
        }

        public User GetById(string id)
        {
            return this.users.GetById(id);
        }

        public string GetRoleName(User user)
        {
            var result = this.userManager.GetRoles(user.Id).FirstOrDefault();
            return result;
        }

        public string GetRoleNameById(string roleId)
        {
            var result = this.roleManager.Roles.Where(x => x.Id == roleId).Select(x => x.Name).FirstOrDefault();
            return result;
        }

        public IQueryable<IdentityRole> GetRoles()
        {
            var result = this.roleManager.Roles;
            return result.AsQueryable();
        }

        public void SetRoleName(User user, string name)
        {
            var role = this.GetRoleNameById(name);
            this.userManager.RemoveFromRole(user.Id, role);
            this.userManager.AddToRole(user.Id, role);
            this.users.Update(user);
        }

        public void Update(User user)
        {
            this.users.Update(user);
        }

        public void UpdatePassword(User user, string password)
        {
            user.UserName = user.Email;
            this.userManager.ChangePassword(user.Id, "changeme", password);
            this.users.Update(user);
        }
    }
}