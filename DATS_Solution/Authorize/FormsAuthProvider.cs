using System.Web.Security;

namespace DATS
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password)
        {
            bool result = FormsAuthentication.Authenticate(username, password);
            if (result)
            {
                FormsAuthentication.SetAuthCookie(username, false);
            }
            return result;
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}