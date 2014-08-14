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
                    logger.Trace("Пользователь прошёл аутетификацию | " + HttpContext.User.Identity.Name + " | " + Request.UserHostAddress);
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }
                else
                {
                    logger.Trace("Неверный пользователь или пароль | " + HttpContext.User.Identity.Name + " | " + Request.UserHostAddress);
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

            logger.Trace("Logout пользователя | " + HttpContext.User.Identity.Name + " | " + Request.UserHostAddress);
            authProvider.Logout();

            return Redirect(Url.Action("Login", "Account"));
        }

    }
}