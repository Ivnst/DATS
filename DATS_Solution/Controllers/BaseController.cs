using System.Web.Mvc;
using Ninject;

namespace DATS.Controllers
{
    public class BaseController : Controller
    {
      [Inject]
      public IRepository Repository;

    }
}
