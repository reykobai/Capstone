using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCollaborationSystem.Controllers
{
	public class ChatController : Controller
	{
		//
		// GET: /Chat/
		public ActionResult Index()
		{
			string username = "puka";
			string group = "koala";



			if(Request.QueryString["username"] != null)
			{
				username = Request.QueryString["username"] as string;
			}

			if (Request.QueryString["group"] != null)
			{
				group = Request.QueryString["group"] as string;
			}

			ViewBag.Username = username;
			ViewBag.Group = group;

			return View();
		}
	}
}