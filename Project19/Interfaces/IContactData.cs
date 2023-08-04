using Microsoft.AspNetCore.Http;
using Project19.AuthContactApp;
using Project19.Entitys;
using System.Security.Claims;

namespace Project19.Interfaces
{
    public interface IContactData
    {
        bool CheckToken(HttpContext httpContext);
        IEnumerable<Contact> GetContacts(HttpContext httpContext);
        void AddContacts(Contact contact, HttpContext httpContext);
        void DeleteContact(int id, HttpContext httpContext);
        Task<Contact> FindContactById(int id, HttpContext httpContext);
        void ChangeContact(string name, string surname,
            string fatherName, string telephoneNumber, 
            string residenceAdress, string description, int id
            ,HttpContext httpContext);
        string IsLogin(UserLoginProp model);
        string IsRegister(UserRegistration model);
        bool AdministrationRegister(UserRegistration model);
        string RoleCreate(RoleModel model, HttpContext httpContext);
        string AddRoleToUser(RoleModel model, HttpContext httpContext);
        string RemoveRoleUser(RoleModel model, HttpContext httpContext);
        string UserRemove(RoleModel model, HttpContext httpContext);

        IList<string> GetCurrentRoles(HttpContext httpContext);
        IList<string> GetAllUsers(HttpContext httpContext);
        IList<string> GetAllAdmins(HttpContext httpContext);
    }

}
