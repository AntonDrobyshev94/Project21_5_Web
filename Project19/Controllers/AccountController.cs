using Microsoft.AspNetCore.Mvc;
using Project19.AuthContactApp;
using Project19.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Project19.Controllers
{
    public class AccountController : Controller
    {
        private readonly IContactData _contactData;

        public AccountController(IContactData contactData)
        {
            _contactData = contactData;
        }

        /// <summary>
        /// Асинхронный Get запрос на открытие страницы администратора. 
        /// В результате запроса происходит проверка на наличие роли
        /// администратора в куки. Дале происходит запуск методов
        /// получения текущих ролей пользователя, получения списка 
        /// всех пользователей и списка всех администраторов в виде
        /// коллекции List. Результаты выполнения записываются во ViewBag
        /// и используются в cshtml представлении. Далее происходит
        /// отработка метода проверки текущих куки и по окончанию 
        /// ключевым словом return происходит переход на модель.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AdminWindow()
        {
            if (!_contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }

            if (!string.IsNullOrEmpty(Request.Cookies["RoleCookie"]))
            {
                string roleValue = Request.Cookies["RoleCookie"];
                if (roleValue != "Admin")
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            var userRole = _contactData.GetCurrentRoles(HttpContext);
            if (userRole.Count <= 0) //проверяем количество ролей
            {
                userRole.Add("Роль не определена");
            }

            var users = _contactData.GetAllUsers(HttpContext);
            var adminUsers = _contactData.GetAllAdmins(HttpContext);

            ViewBag.Role = userRole;
            ViewBag.AllUsers = users;
            ViewBag.Admins = adminUsers;

            CheckCookieMethod();

            return View();
        }

        /// <summary>
        /// Асинхронный Post запрос создания новой роли, принимающий 
        /// string переменную roleName (название роли). С помощью 
        /// метода RoleCreate происходит запрос в API на создание
        /// новой роли. В результате в переменную createRoleResponse
        /// записывается ответ об успехе в создании роли. С помощью 
        /// операторов if/else происходит проверка 
        /// ответа createRoleResponse посредством проверки содержимого 
        /// этого ответа (сверяется с указанной строкой). Если ответ 
        /// содержит указанный в условии порядок слов, то
        /// происходит запись сообщений о результатах ввода в 
        /// экземпляр TempData. 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateNewRole(string roleNameString)
        {
            if (!_contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            bool isCreate = false;
            string createRoleResponse = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(roleNameString))
                {
                    RoleModel roleModel = new RoleModel
                    {
                        roleName = roleNameString,
                        userName = ""
                    };

                    createRoleResponse = _contactData.RoleCreate(roleModel, HttpContext);

                    if (createRoleResponse == "Роль успешно добавлена")
                    {
                        TempData["RoleCreateMessage"] = "Роль успешно добавлена!";
                        isCreate = true;
                    }
                    else
                    {
                        TempData["RoleCreateMessage"] = "Роль уже существует";
                        isCreate = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["RoleCreateMessage"] = $"{ex}";
                isCreate = false;
            }
            TempData["IsCreate"] = isCreate;
            return RedirectToAction("AdminWindow", "Account");
        }

        /// <summary>
        /// Асинхронный Post запрос добавления роли указанному
        /// пользователю, принимающий переменные строкового типа
        /// roleName и userName (название роли и имя пользователя).
        /// С помощью метода AddRoleToUser происходит запрос в API
        /// на добавление ролей пользователю, в результате в переменную
        /// addRoleTouserResponse записывается ответ об успехе в 
        /// добавлении роли. С помощью операторов if/else происходит 
        /// проверка ответа addRoleToUserResponse посредством проверки 
        /// содержимого этого ответа методом Contains . Если ответ 
        /// содержит указанный в условиях порядок слов, то происходит 
        /// запись сообщений о результатах ввода в экземпляры TempData.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddUserRole(string roleNameString, string userNameString)
        {
            if (!_contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            string addRoleToUserResponse = string.Empty;
            bool isRoleAvailable = false;
            bool isUserAvailable = false;
            if (!string.IsNullOrEmpty(roleNameString) && !string.IsNullOrEmpty(userNameString))
            {
                RoleModel roleModel = new RoleModel
                {
                    roleName = roleNameString,
                    userName = userNameString
                };
                addRoleToUserResponse = _contactData.AddRoleToUser(roleModel, HttpContext);
                if (addRoleToUserResponse.Contains("Роль успешно добавлена"))
                {
                    TempData["UserMessage"] = "Пользователь указан верно";
                    TempData["SuccessMessage"] = "Роль успешно добавлена";
                    TempData["MessageRole"] = "Роль доступна для добавления";
                    isRoleAvailable = true;
                    isUserAvailable = true;
                }
                else
                {
                    if (addRoleToUserResponse.Contains("Роль доступна для добавления"))
                    {
                        TempData["MessageRole"] = "Роль доступна для добавления";
                        isRoleAvailable = true;
                    }
                    else
                    {
                        TempData["MessageRole"] = "Ошибка: указанная роль не существует";
                        isRoleAvailable = false;
                    }
                    if (addRoleToUserResponse.Contains("Пользователь указан верно"))
                    {
                        TempData["UserMessage"] = "Пользователь указан верно";
                        isUserAvailable = true;
                    }
                    else
                    {
                        TempData["UserMessage"] = "Пользователь отсутствует";
                        isUserAvailable = false;
                    }
                }
            }
            else
            {
                TempData["UserMessage"] = "Ошибка при распозновании";
                isUserAvailable = false;
                TempData["MessageRole"] = "указанных данных";
                isRoleAvailable = false;
            }
            TempData["isRoleAvailable"] = isRoleAvailable;
            TempData["isUserAvailable"] = isUserAvailable;
            return RedirectToAction("AdminWindow", "Account");
        }

        /// <summary>
        /// Асинхронный запрос на удаление роли у указанного
        /// пользователя, принимающий переменные строкового типа
        /// roleName и userName (название роли и имя пользователя).
        /// С помощью метода RemoveRoleUser происходит запрос в API
        /// на удаление ролей у пользователя, в результате в переменную
        /// removeUserRoleResponse записывается ответ об успехе в 
        /// удалении роли. С помощью операторов if/else происходит 
        /// проверка ответа removeUserRoleResponse посредством проверки 
        /// содержимого этого ответа методом Contains. Если ответ 
        /// содержит указанный в условиях порядок слов, то происходит 
        /// запись сообщений о результатах ввода в экземпляры TempData.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveUserRole(string roleNameString, string userNameString)
        {
            if (!_contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            string removeUserRoleResponse = string.Empty;
            bool isRoleAvailable = false;
            bool isUserAvailable = false;
            bool isUserHaveRole = false;
            if (!string.IsNullOrEmpty(roleNameString) && !string.IsNullOrEmpty(userNameString))
            {
                RoleModel roleModel = new RoleModel
                {
                    roleName = roleNameString,
                    userName = userNameString
                };
                removeUserRoleResponse = _contactData.RemoveRoleUser(roleModel, HttpContext);
                if (removeUserRoleResponse.Contains("Роль успешно удалена"))
                {
                    TempData["UserDeleteMessage"] = "Пользователь указан верно";
                    TempData["DeleteMessage"] = "Роль успешно удалена";
                    TempData["MessageDeleteRole"] = "Роль доступна для удаления";
                    isRoleAvailable = true;
                    isUserAvailable = true;
                    isUserHaveRole = true;
                }
                else if (removeUserRoleResponse.Contains("Роль отсутствует у указанного пользователя"))
                {
                    TempData["DeleteMessage"] = "Роль отсутствует у указанного пользователя";
                    isUserHaveRole = false;
                }
                else
                {
                    if (removeUserRoleResponse.Contains("Роль доступна для удаления"))
                    {
                        TempData["MessageDeleteRole"] = "Роль доступна для удаления";
                        isRoleAvailable = true;
                    }
                    else
                    {
                        TempData["MessageDeleteRole"] = "Ошибка: указанная роль не существует";
                        isRoleAvailable = false;
                    }
                    if (removeUserRoleResponse.Contains("Пользователь указан верно"))
                    {
                        TempData["UserDeleteMessage"] = "Пользователь указан верно";
                        isUserAvailable = true;
                    }
                    else
                    {
                        TempData["UserDeleteMessage"] = "Пользователь отсутствует";
                        isUserAvailable = false;
                    }
                }
            }
            else
            {
                TempData["MessageDeleteRole"] = "Ошибка при распозновании";
                isRoleAvailable = false;
                TempData["UserDeleteMessage"] = "указанных данных";
                isUserAvailable = false;
            }
            TempData["isRoleAvailable"] = isRoleAvailable;
            TempData["isUserAvailable"] = isUserAvailable;
            TempData["isUserHaveRole"] = isUserHaveRole;
            return RedirectToAction("AdminWindow", "Account");
        }

        /// <summary>
        /// Асинхронный запрос на удаление пользователя, 
        /// принимающий переменную userName строкового типа. 
        /// С помощью метода UserRemove происходит запрос в API
        /// на удаление пользователя, в результате в переменную
        /// removeUserResponse записывается ответ об успехе в 
        /// удалении пользователя. С помощью операторов if/else происходит 
        /// проверка ответа removeUserResponse посредством проверки 
        /// содержимого этого ответа методом Contains. Если ответ 
        /// содержит указанный в условиях порядок слов, то происходит 
        /// запись сообщений о результатах ввода в экземпляры TempData.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveUser(string userNameString)
        {
            if (!_contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            string removeUserResponse = string.Empty;
            bool isRemoveUser;
            if (!string.IsNullOrEmpty(userNameString))
            {
                RoleModel roleModel = new RoleModel
                {
                    roleName = "",
                    userName = userNameString
                };
                removeUserResponse = _contactData.UserRemove(roleModel, HttpContext);
                if (removeUserResponse.Contains("Пользователь успешно удален"))
                {
                    TempData["DeleteUserMessage"] = "Пользователь успешно удален";
                    isRemoveUser = true;
                }
                else if (removeUserResponse.Contains("Пользователь отсутствует"))
                {
                    TempData["DeleteUserMessage"] = "Пользователь отсутствует";
                    isRemoveUser = false;
                }
                else
                {
                    TempData["DeleteUserMessage"] = "Ошибка";
                    isRemoveUser = false;
                }
            }
            else
            {
                TempData["DeleteUserMessage"] = "Ошибка";
                isRemoveUser = false;
            }
            TempData["IsRemoveUser"] = isRemoveUser;
            return RedirectToAction("AdminWindow", "Account");
        }

        /// <summary>
        /// Get запрос, в результате которого происходит переход
        /// на страницу Login (входа в аккаунт). В результате
        /// запроса происходит запоминание адреса страницы 
        /// при помощи ключевого слова return для дальнейшего
        /// возврата на эту страницу.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Асинхронный метод, принимающий модель UserLogin,
        /// реализованную отдельным классом и возвращающий результат
        /// выполнения данной модели. В методе происходит проверка
        /// принимаемой модели на валидность и если модель валидна,
        /// то создается с помощью метода IsLogin создается запрос
        /// в API для проверки модели и получения токена в случае
        /// успешной авторизации. Если полученный токен равен
        /// пустой строке, то произойдет ошибка авторизации. Если
        /// не равен, то происходит проверка содержимого токена 
        /// на наличие Claims Администратора и имени пользователя,
        /// которые записываются в файлы Куки для дальнейшего
        /// использования.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null && model.Password != null)
                {
                    string loginProp = model.LoginProp;
                    string password = model.Password;
                    UserLoginProp userLogin = new UserLoginProp()
                    {
                        UserName = loginProp,
                        Password = password
                    };
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true
                    };
                    string token = string.Empty;
                    token = _contactData.IsLogin(userLogin);
                    if (token != string.Empty)
                    {
                        Response.Cookies.Append("AuthToken", token, cookieOptions);
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadJwtToken(token);
                        foreach (var item in jwtToken.Claims)
                        {
                            if (item.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                            {
                                if (item.Value == "Admin")
                                {
                                    Response.Cookies.Append("RoleCookie", item.Value, cookieOptions);
                                    break;
                                }
                                else
                                {
                                    Response.Cookies.Append("RoleCookie", item.Value, cookieOptions);
                                }
                            }
                        }
                        foreach (var item in jwtToken.Claims)
                        {
                            if (item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                            {
                                Response.Cookies.Append("UserNameCookie", item.Value, cookieOptions);
                            }
                        }
                        return RedirectToAction("Index", "MyDefault");
                    }
                    else
                    {
                        Response.Cookies.Append("AuthToken","", cookieOptions);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            ModelState.AddModelError("", "Пользователь не найден");
            return View(model);
        }

        /// <summary>
        /// Get запрос на открытие формы регистрации, который
        /// отправляет новый экземпляр UserRegistration в
        /// представление Register в качестве модели.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserRegistration());
        }

        /// <summary>
        /// Асинхронный Post запрос, принимающий модель регистрации,
        /// проверяющий правильность этой модели и на ее основе
        /// с помощью метода IsRegiseter происходит запрос в API
        /// на регистрацию нового пользователя. Если полученный 
        /// токен равен пустой строке, то произойдет ошибка 
        /// авторизации. Если не равен, то происходит запись токена,
        /// имени пользователя и роли User  в куки для дальнейшего
        /// использования. В конце происходит редирект на главную
        /// страницу.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Register(UserRegistration model)
        {
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null)
                {
                    string token = string.Empty;
                    token = _contactData.IsRegister(model);

                    if (token != string.Empty)
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadJwtToken(token);

                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true
                        };

                        Response.Cookies.Append("AuthToken", token, cookieOptions);
                        Response.Cookies.Append("RoleCookie", "User", cookieOptions);

                        foreach (var item in jwtToken.Claims)
                        {
                            if (item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                            {
                                Response.Cookies.Append("UserNameCookie", item.Value, cookieOptions);
                            }
                        }
                        return RedirectToAction("Index", "MyDefault");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ошибка регистрации");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Пароли не совпадают или ошибка формата");
            }
            return View(model);
        }

        /// <summary>
        /// Get запрос на открытие формы регистрации нового пользователя
        /// от имени Администратора
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AdminRegister()
        {
            if (!_contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            return View(new UserRegistration());
        }

        /// <summary>
        /// Асинхронный метод, принимающий модель регистрации,
        /// проверяющий правильность этой модели и на ее основе 
        /// создающий нового пользователя и добавляющий в базу 
        /// данных. Если модель не корректная, то выдается
        /// сообщение об ошибке. Если действие выполнено, то 
        /// выдается сообщение об успешном выполнении.
        /// Далее происходит отработка метода AdministrationRegister,
        /// который принимает в себя модель регистрации 
        /// UserRegistration и текущий Http-контекст. Если результат
        /// отработки метода - true (т.е. аккаунт зарегистрирован),
        /// то выдает сообщение об успешной регистрации. При false -
        /// выдает ошибку.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> AdminRegister(UserRegistration model)
        {
            if (!_contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            if (!string.IsNullOrEmpty(Request.Cookies["RoleCookie"]))
            {
                string roleValue = Request.Cookies["RoleCookie"];
                if (roleValue != "Admin")
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            CheckCookieMethod();
            bool isSucceed = false;
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null)
                {
                    isSucceed = _contactData.AdministrationRegister(model);

                    if (isSucceed)
                    {

                    }
                    else
                    {
                        ModelState.AddModelError("", "Ошибка регистрации");
                    }
                }
            }
            else
            {
                isSucceed = false;
                ModelState.AddModelError("", "Пароли не совпадают или неверный формат");
            }
            ViewBag.IsSuccess = isSucceed;
            TempData["UserCreateMessage"] = isSucceed ? "Пользовательский аккаунт создан" : "Ошибка при создании";
            return View(model);
        }

        /// <summary>
        /// Метод, осуществляющий переход на страницу входа Account
        /// Login.
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            LogoutMethod();

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Метод проверки куки, в котором происходит проверка наличия
        /// куки с именем пользователя, ролью и токеном. После проверки
        /// куки сохраняются в строковые переменные, а далее сохраняются
        /// во ViewBag для дальнейшего использования во View модели.
        /// </summary>
        private void CheckCookieMethod()
        {
            if (!string.IsNullOrEmpty(Request.Cookies["UserNameCookie"]) &&
                !string.IsNullOrEmpty(Request.Cookies["RoleCookie"]) &&
                !string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
            {
                string nameValue = Request.Cookies["UserNameCookie"];
                string roleValue = Request.Cookies["RoleCookie"];
                string tokenValue = Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(tokenValue))
                {
                    ViewBag.IsAuth = true;
                }
                else
                {
                    ViewBag.IsAuth = false;
                }
                ViewBag.UserName = nameValue;
                ViewBag.RoleName = roleValue;
            }
        }

        /// Метод выхода из учетной записи. В данном
        /// методе происходит перезапись текущих куки на новые
        /// значения, которые равны пустым строкам (т.е. обнуление
        /// аутентификационных данных пользователя, хранящихся в куки).
        public void LogoutMethod()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };

            Response.Cookies.Append("AuthToken", string.Empty, cookieOptions);
            Response.Cookies.Append("RoleCookie", string.Empty, cookieOptions);
            Response.Cookies.Append("UserNameCookie", string.Empty, cookieOptions);
        }
    }
}
