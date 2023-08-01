using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Project19.AuthContactApp;
using Project19.Entitys;
using Project19.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;

namespace Project19.Data
{
    public class ContactDataApi : IContactData
    {
        private HttpClient httpClient { get; set; }

        public ContactDataApi()
        {
            httpClient = new HttpClient();
        }

         public IEnumerable<Contact> GetContacts(HttpContext httpContext)
            {
                string url = @"https://localhost:7037/api/values";
                string tokenValue = httpContext.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(tokenValue))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
                }
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<IEnumerable<Contact>>(json);
            }

        public IList<string> GetCurrentRoles(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/CurrentRoles";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }

            string json = httpClient.GetStringAsync(url).Result;

            IList<string> list = JsonConvert.DeserializeObject<IList<string>>(json);
            return list;
        }

        public IList<string> GetAllUsers(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/GetUsers";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            string json = httpClient.GetStringAsync(url).Result;
            IList<string> list = JsonConvert.DeserializeObject<IList<string>>(json);
            return list;
        }

        public IList<string> GetAllAdmins(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/GetAdmins";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            string json = httpClient.GetStringAsync(url).Result;
            IList<string> list = JsonConvert.DeserializeObject<IList<string>>(json);
            return list;
        }

        public void AddContacts(Contact contact, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        public void DeleteContact(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/{id}";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }

        public async Task<Contact> FindContactById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/Details/{id}";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<Contact>(json);
            }
            catch
            {
                return null;
            }
        }

        public void ChangeContact(string name, string surname,
            string fatherName, string telephoneNumber, string residenceAdress, string description, int id, HttpContext httpContext)
        {
            Contact contact = new Contact()
            {
                Name = name,
                Surname = surname,
                FatherName = fatherName,
                TelephoneNumber = telephoneNumber,
                ResidenceAdress = residenceAdress,
                Description = description
            };

            string url = $"https://localhost:7037/api/values/ChangeContactById/{id}";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        public string RoleCreate(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/RoleCreate";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
            try
            {
                string result = r.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string AddRoleToUser(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/AddRoleToUser";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
            try
            {
                string result = r.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string RemoveRoleUser(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/RemoveUserRole";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
            try
            {
                string result = r.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string UserRemove(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/RemoveUser";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
            try
            {
                string result = r.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string IsRegister(UserRegistration model)
        {
            string url = $"https://localhost:7037/api/values/Registration/";

            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
            string token = "";
            if (r.IsSuccessStatusCode)
            {
                string json = r.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
                string tokenAuth = tokenResponse.access_token;

                token = tokenAuth;
            }
            else
            {
                token = string.Empty;
            }
            return token;
        }

        public bool AdministationRegister(UserRegistration model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/AdminRegistration/";
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
            bool isAccountCreate = false;
            if (r.IsSuccessStatusCode)
            {
                isAccountCreate = true;
            }
            else
            {
                isAccountCreate = false;
            }
            return true;
        }

        public string IsLogin(UserLoginProp model)
        {
            string url = $"https://localhost:7037/api/values/Authenticate/";

            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;

            string token = "";
            if (r.IsSuccessStatusCode)
            {
                string json = r.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
                string tokenAuth = tokenResponse.access_token;

                token = tokenAuth;
            }
            else
            {
                token = string.Empty;
            }
            return token;
        }

    }
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string username { get; set; }
    }
}
