﻿using System.Web.Mvc;
using Ninject;

namespace DATS.Controllers
{
    public class BaseController : Controller
    {
      [Inject]
      public IRepository Repository { get; set; }

      protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    }
}
