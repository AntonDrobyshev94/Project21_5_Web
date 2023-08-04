using Newtonsoft.Json;
using Project19.AuthContactApp;
using Project19.Entitys;
using Project19.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace Project19.Data
{
    public class ContactDataApi : IContactData
    {
        private HttpClient httpClient { get; set; }

        public ContactDataApi()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Запрос для проверки валидности токена. Запрос пытается
        /// обратиться к API и если нет ошибки в блоке try (запрос 
        /// удался), то токен валидный. Если возникает ошибка, то 
        /// происходит переход в catch (токен не валидный).
        /// </summary>
        /// <returns></returns>
        public bool CheckToken(HttpContext httpContext)
        {
            string response = string.Empty;
            string url = @"https://localhost:7037/api/values/CheckToken";
            try
            {
                AddTokenHeaderMethod(httpContext);
                response = httpClient.GetStringAsync(url).Result;
                if (response == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Запрос на получение всех контактов, передающийся на API 
        /// сервер. Данный запрос принимает текущий Http-контекст, 
        /// который позволяет обратиться к куки, в которых хранится 
        /// токен. Запрос возвращает результат в виде коллекции
        /// объектов типа Contact.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public IEnumerable<Contact> GetContacts(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values";
            AddTokenHeaderMethod(httpContext);
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<IEnumerable<Contact>>(json);
        }

        /// <summary>
        /// Запрос на получение списка всех ролей текущего пользователя, 
        /// передающийся на API сервер. Данный запрос принимает текущий 
        /// Http-контекст, который позволяет обратиться к куки, в которых 
        /// хранится токен. Запрос возвращает коллекцию IList 
        /// параметаризированную строкой,в которой содержится список 
        /// пользователей.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public IList<string> GetCurrentRoles(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/CurrentRoles";
            AddTokenHeaderMethod(httpContext);
            string json = httpClient.GetStringAsync(url).Result;

            IList<string> list = JsonConvert.DeserializeObject<IList<string>>(json);
            return list;
        }

        /// <summary>
        /// Запрос на получение списка всех пользователей, передающийся 
        /// на API сервер. Данный запрос принимает текущий Http-контекст, 
        /// который позволяет обратиться к куки, в которых хранится токен. 
        /// Запрос возвращает коллекцию IList параметаризированную строкой,
        /// в которой содержится список пользователей.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public IList<string> GetAllUsers(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/GetUsers";
            AddTokenHeaderMethod(httpContext);
            string json = httpClient.GetStringAsync(url).Result;
            IList<string> list = JsonConvert.DeserializeObject<IList<string>>(json);
            return list;
        }

        /// <summary>
        /// Запрос на получение списка всех администраторов, передающийся 
        /// на API сервер. Данный запрос принимает текущий Http-контекст, 
        /// который позволяет обратиться к куки, в которых хранится токен. 
        /// Запрос возвращает коллекцию IList параметаризированную строкой,
        /// в которой содержится список администрации.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public IList<string> GetAllAdmins(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/GetAdmins";
            AddTokenHeaderMethod(httpContext);
            string json = httpClient.GetStringAsync(url).Result;
            IList<string> list = JsonConvert.DeserializeObject<IList<string>>(json);
            return list;
        }




        /// <summary>
        /// Запрос на создание нового контакта, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр контакта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="httpContext"></param>
        public void AddContacts(Contact contact, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на удаление контакта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id контакта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void DeleteContact(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/{id}";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }

        /// <summary>
        /// Запрос на поиск контакта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id контакта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра Contact.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<Contact> FindContactById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/Details/{id}";
            AddTokenHeaderMethod(httpContext);
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

        /// <summary>
        /// Запрос на изменение контакта, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра Contact
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Contact. Данный метод является невозвратным.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="fatherName"></param>
        /// <param name="telephoneNumber"></param>
        /// <param name="residenceAdress"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
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
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на создание новой роли , передающийся 
        /// на API сервер. Данный запрос принимает модель роли, 
        /// которая включает в себя имя пользователя и название роли 
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде строки с ответом о результате создания роли.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string RoleCreate(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/RoleCreate";
            AddTokenHeaderMethod(httpContext);
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

        /// <summary>
        /// Запрос на добавление роли пользователю, передающийся 
        /// на API сервер. Данный запрос принимает модель роли, 
        /// которая включает в себя имя пользователя и название роли 
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде строки с ответом о результате добавления.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string AddRoleToUser(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/AddRoleToUser";
            AddTokenHeaderMethod(httpContext);
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

        /// <summary>
        /// Запрос на удаление роли у пользователя, передающийся 
        /// на API сервер. Данный запрос принимает модель роли, 
        /// которая включает в себя имя пользователя и название роли 
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде строки с ответом о результате удаления.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string RemoveRoleUser(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/RemoveUserRole";
            AddTokenHeaderMethod(httpContext);
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

        /// <summary>
        /// Запрос на удаление пользователя, передающийся в API сервер.
        /// Данный запрос принимает модель роли, которая включает
        /// в себя имя пользователя и название роли и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен.
        /// Запрос возвращает результат в виде строки с ответом
        /// о результате удаления.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string UserRemove(RoleModel model, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/RemoveUser";
            AddTokenHeaderMethod(httpContext);
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

        /// <summary>
        /// Запрос на регистрацию, передающийся в API сервер.
        /// Данный запрос принимает модель регистрации и возвращает
        /// результат запроса в виде строки, содержащей токен.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Запрос на регистрацию из окна администрации, передающийся
        /// в API сервер. Данный запрос принимает модель регистрации 
        /// UserRegistration и возвращающает логическую переменную, 
        /// означающую результат создания аккаунта.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public bool AdministrationRegister(UserRegistration model)
        {
            string url = $"https://localhost:7037/api/values/Registration/";
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

        /// <summary>
        /// Запрос на вход пользователя, передающийся в API
        /// Данный запрос принимает модель UserLoginProp
        /// и возвращает строковую переменную с ответом,
        /// который будет включать в себя токен при удачном
        /// входе или пустую строку при неудачном входе.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Метод добавления токена в заголовок запроса. Данный метод
        /// принимает в себя текущий HTTP-контекст (то есть текущий
        /// запрос), что в свою очередь позволяет обратиться к методу
        /// Request для вызова токена, сохраненного в куки.
        /// В методе происходит запись токена из куки и дальнейшее
        /// добавление токена в заголовок запроса с помощью 
        /// создания нового экземпляра заголовка аутентификации
        /// AuthenticationHeaderValue.
        /// </summary>
        /// <param name="httpContext"></param>
        private void AddTokenHeaderMethod(HttpContext httpContext)
        {
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
                Console.WriteLine($"{tokenValue}");
            }
        }

    }
}
