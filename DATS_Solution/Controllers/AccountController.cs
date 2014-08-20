using System.Web.Mvc;
using DATS;

namespace DATS.Controllers
{
    public class AccountController : BaseController
    {
        IAuthProvider authProvider;
        public AccountController(IAuthProvider auth)
        {
            authProvider = auth;
        }

        [AllowAnonymous]
        public ViewResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.UserName, model.Password))
                {
                    try
                    {
                        logger.Trace("Пользователь прошёл аутетификацию | " + HttpContext.User.Identity.Name + " | " + Request.UserHostAddress);
                    }
                    catch
                    {
                        // не обрабатываем, так как ошибка возникает только в режиме выполнения unit test
                    }
                    
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }
                else
                {
                    try
                    {
                        logger.Trace("Неверный пользователь или пароль | " + HttpContext.User.Identity.Name + " | " + Request.UserHostAddress);
                    }
                    catch
                    {
                        // не обрабатываем, так как ошибка возникает только в режиме выполнения unit test
                    }
                    
                    ModelState.AddModelError("", "Неверный пользователь или пароль.");
                    
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Logout()
        {
            try
            { 
            logger.Trace("Logout пользователя | " + HttpContext.User.Identity.Name + " | " + Request.UserHostAddress);
            }
            catch
            {
                // не обрабатываем, так как ошибка возникает только в режиме выполнения unit test
            }
            
            authProvider.Logout();

            return Redirect(Url.Action("Login", "Account"));
        }

    }
}