using System;
using System.Web.Security;
using SistemRecrutare.Models;
using System.Data;
using System.Linq;

namespace SistemRecrutare.MyRoleProvider
{
    // suprascrierea clasei abstracte RoleProvider
    public class SiteRole : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        // roluri utilizatori
        public override string[] GetRolesForUser(string email)
        {
            using(var db = new DBrecrutare())
            {
                var rezultat = (from utilizator in db.utilizators
                                join rol in db.rols on utilizator.id_rol equals
                                rol.id_rol
                                where utilizator.email == email
                                select rol.denumire_rol).ToArray();
                return rezultat;
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
    }
}