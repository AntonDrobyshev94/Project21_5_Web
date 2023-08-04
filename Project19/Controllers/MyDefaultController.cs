using Microsoft.AspNetCore.Mvc;
using Project19.Entitys;
using Project19.Interfaces;

namespace Project19.Controllers
{
    public class MyDefaultController : Controller
    {
        private readonly IContactData contactData;

        public MyDefaultController(IContactData ContactData)
        {
            this.contactData = ContactData;
        }

        public IActionResult Index()
        {
            CheckCookieMethod();
            return View(contactData.GetContacts(HttpContext));
        }

        /// <summary>
        /// Метод открытия нового View для добавления контакта
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddContact()
        {
            if (!contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            return View();
        }

        /// <summary>
        /// Метод, принимающий в себя параметры строкового
        /// типа. В данном методе происходит добавление в базу данных
        /// нового контакта с параметрами, указанными во View. По
        /// окончанию происходит сохранение данных с помощью метода
        /// SaveChanges и возврат на стартовую страницу.
        /// </summary>
        /// <param name="surname"></param>
        /// <param name="name"></param>
        /// <param name="fatherName"></param>
        /// <param name="telephoneNumber"></param>
        /// <param name="residenceAdress"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetDataFromViewField(string surname, string name,
        string fatherName, string telephoneNumber, string residenceAdress,
        string description)
        {
            if (!contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            contactData.CheckToken(HttpContext);
            var contact = new Contact()
            {
                Surname = surname,
                Name = name,
                FatherName = fatherName,
                TelephoneNumber = telephoneNumber,
                ResidenceAdress = residenceAdress,
                Description = description
            };
            contactData.AddContacts(contact, HttpContext);
            return Redirect("~/");
        }

        /// <summary>
        /// Асинхронный метод, принимающий параметр int id.Происходит
        /// проверка на наличие авторизации по роли админ для
        /// изменения контакта. Далее происходит проверка куки методом
        /// CheckCookieMethod и создание экземпляра контакта, который
        /// определяется с помощью метода FindContactById, который
        /// посылает запрос в API для поиска контакта и возврата
        /// его экземпляра для дальнейшего использования во View.
        /// Также, текущий изменяемый id записывается в файлы куки
        /// для дальнейшего использования в методе 
        /// ChangeDataFromViewField
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Change(int id)
        {
            if (!contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            if (!string.IsNullOrEmpty(Request.Cookies["RoleCookie"]))
            {
                string roleValue = Request.Cookies["RoleCookie"];
                if (roleValue != "Admin")
                {
                    return Redirect("~/");
                }
                CheckCookieMethod();
                Contact concreteContact = await contactData.FindContactById(id, HttpContext);
                if (concreteContact != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true
                    };

                    string currentId = id.ToString();
                    Response.Cookies.Append("CurrentId", currentId, cookieOptions);

                    return View(concreteContact);
                }
                else
                {
                    return Redirect("~/");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        /// <summary>
        /// Метод изменения контакта, принимающий в себя параметры 
        /// строкового типа. В методе происходит проверка на наличие
        /// файла куки с текущим id контакта. Данный id сохраняется
        /// в int переменную currentId посредством приведения из 
        /// строки с помощью метода Parse. Далее происходит запуск
        /// метода ChangeContact, в который передаются измененные
        /// значения контакта. Данный метод передаёт значения в API.
        /// По окончанию происходит редирект на стартовую страницу.
        /// </summary>
        /// <param name="surname"></param>
        /// <param name="name"></param>
        /// <param name="fatherName"></param>
        /// <param name="telephoneNumber"></param>
        /// <param name="residenceAdress"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeDataFromViewField(string surname, string name,
        string fatherName, string telephoneNumber, string residenceAdress,
        string description)
        {
            if (!contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            if (!string.IsNullOrEmpty(Request.Cookies["CurrentId"]))
            {
                string currentIdStr = Request.Cookies["CurrentId"];
                int currentId = int.Parse(currentIdStr);
                contactData.ChangeContact(name, surname,
                             fatherName, telephoneNumber, residenceAdress, description, currentId, HttpContext);
                return Redirect("~/");
            }
            else
            {
                return Redirect("~/");
            }
        }

        /// <summary>
        /// Метод, возвращающий экземпляр класса Contact с указанным id, 
        /// данный метод принимает в себя int id. В методе происходит 
        /// созадние экземпляра Contact, в который записывается
        /// результат отработки метода FindContactById, принимающий id
        /// контакта. Метод FindContactById создает запрос в API и 
        /// принимает ответ в виде экземпляра контакта. По окончанию
        /// происходит вывод экземпляра во View модель и открытие данного
        /// View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            Contact concreteContact = await contactData.FindContactById(id, HttpContext);
            if (concreteContact != null)
            {
                CheckCookieMethod();
                return View(concreteContact);
            }
            else
            {
                return Redirect("~/");
            }

        }

        /// <summary>
        /// Метод, принимающий переменную int id.
        /// Атрибут ActionName("Delete") говорит о том, что
        /// данный метод будет вызван в результате action метода
        /// "Delete" в представлении Index.cshtml.
        /// Данный метод обращается к методу DeleteContact и передает
        /// в него id контакта для удаления. DeleteContact отправляет
        /// запрос в API на удаление контакта.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!contactData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            contactData.DeleteContact(id, HttpContext);
            return Redirect("~/");
        }

        /// <summary>
        /// Метод проверки наличия куки с именем пользователя, ролью и
        /// токеном. Если куки существуют и не равны нулю или пустой 
        /// строке, то происходит их запись в строковые переменные.
        /// Если строка токена не равна нулю или пустой строке, то
        /// происходит запись значение IsAuth во ViewBag равное true.
        /// Значение имени и роли также записываются во ViewBag.
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
