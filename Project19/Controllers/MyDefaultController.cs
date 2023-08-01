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
        /// с атрибутом Authorize, который говорит, что
        /// он будет доступен только для авторизованного 
        /// пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddContact()
        {
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
        /// Асинхронный метод, принимающий параметр int id. В методе 
        /// происходит перебор базы данных по параметру ID. Если параметр 
        /// совпадает, то данный контакт кешируется в статическую 
        /// переменную concreteContact и возвращается ключевым словом
        /// return для дальнейшего использования в качестве модели. 
        /// При этом происходит сохранение текущего id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Change(int id)
        {
            Contact concreteContact = await contactData.FindContactById(id, HttpContext);
            if (concreteContact != null)
            {
                CheckCookieMethod();
                ViewBag.CurrentId = id;

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

        /// <summary>
        /// Метод изменения контакта, принимающий в себя параметры 
        /// строкового типа. В данном методе происходит перебор базы 
        /// данных методом ChangeContact, где в результате совпадения 
        /// параметра ID с текущим закешированным currentID произойдет замена
        /// параметров текущего экземпляра Contact на указанные во 
        /// View. По окончанию происходит сохранение данных методом
        /// SaveChangesAsync и возврат на стартовую страницу.
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
        /// перебор БД на сравнивнение параметра ID с принимаемым id. 
        /// При совпадении происходит кеширование экземпляра Contact 
        /// в статическую переменную concreteContact и возвращение 
        /// этого экземпляра для использования в качестве модели.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
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
        /// Асинхронный метод, принимающий переменную int id.
        /// Атрибут ActionName("Delete") говорит о том, что
        /// данный метод будет вызван в результате action метода
        /// "Delete" в представлении Index.cshtml.
        /// В данном методе, происходит перебор
        /// базы данных на условие совпадения совпадения 
        /// параметра ID контакта с принимаемым id, после чего
        /// происходит удаление элемента базы данных, сохранение
        /// данных методом SaveChangesAsync и возврат на
        /// стартовую страницу.
        /// в котором происходит 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            contactData.DeleteContact(id, HttpContext);
            return Redirect("~/");
        }

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
    }
}
