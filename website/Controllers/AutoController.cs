using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace website.Controllers
{
    /// <summary>
    /// Controls the folder based page serving.
    /// </summary>
    public class AutoController : HomeController
    {

        public IActionResult Projects(string slug)
        {
            string actionName = ControllerContext.RouteData.Values["action"].ToString(); //Projects
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString(); //Auto

            ViewBag.Action = actionName;
            ViewBag.Controller = controllerName;
            ViewBag.ProjectName = slug;

            if (slug != null && slug.Length > 0)
            {
                return View(slug);
            }

            //List all files in the directory as clickable links    ↱ /Views/Projects/* -> will remove any <actionName>.cshtml, those are reserved for list pages
            List<string> projectLinks = Directory.GetFiles($"Views/{actionName}/").Where(link => !ActionFromViewPath(link.ToLower()).Equals(actionName.ToLower())).ToList();
            ViewBag.Links = projectLinks;

            //Get all of the filenames from the project links, these are the actions for the the router
            List<string> projectActions = new List<string>(); //List of actions used to 
            for (int i = 0; i < projectLinks.Count; i++)
            {
                projectActions.Add(ActionFromViewPath(projectLinks[i]));
            }
            ViewBag.LinkActions = projectActions;

            return View();
        }

        [NonAction]
        private string ActionFromViewPath(string viewPath) {
            string ret = Path.GetFileNameWithoutExtension(viewPath);
            return ret;
        }
    }
}