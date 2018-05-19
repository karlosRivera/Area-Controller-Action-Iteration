using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;

namespace AreaTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var areaNames = RouteTable.Routes.OfType<Route>()
            //.Where(d => d.DataTokens != null && d.DataTokens.ContainsKey("area"))
            //.Select(r => r.DataTokens["area"]).ToList()
            //.Distinct().ToList();

            //return View();
            string currentarea="";
            if (ControllerContext.RouteData.DataTokens["area"] != null)
                currentarea = ControllerContext.RouteData.DataTokens["area"].ToString();

            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            string controllerName =ControllerContext.RouteData.Values["controller"].ToString();

            // Get all controllers with their actions
            var list = GetSubClasses<Controller>();

            // Get all controllers with their actions
            var getAllcontrollers = (from item in list
                                     let name = item.Name
                                     where !item.Name.StartsWith("Account") && !item.Name.StartsWith("Manage")
                                     select new MyController()
                                     {
                                         Name = splitcontrollerName(name),
                                         Namespace = item.Namespace,
                                         MyActions = GetListOfAction(item)
                                     }).ToList();

            // Now we will get all areas that has been registered in route collection
            var getAllAreas = RouteTable.Routes.OfType<Route>()
                .Where(d => d.DataTokens != null && d.DataTokens.ContainsKey("area"))
                .Select(
                    r =>
                        new MyArea
                        {
                            Name = r.DataTokens["area"].ToString(),
                            Namespace = r.DataTokens["Namespaces"] as IList<string>,
                        }).ToList()
                .Distinct().ToList();


            // Add a new area for default controllers
            getAllAreas.Insert(0, new MyArea()
            {
                Name = "Main",
                Namespace = new List<string>()
            {
                typeof (AreaTest.Controllers.HomeController).Namespace
            }
            });


            foreach (var area in getAllAreas.Where(x => x.Name != "Main"))
            {
                var temp = new List<MyController>();
                foreach (var item in area.Namespace)
                {
                    temp.AddRange(getAllcontrollers.Where(x => x.Namespace.Contains(item.Replace('*', ' ').Trim())).ToList());
                }
                area.MyControllers = temp;
            }

            getAllAreas.RemoveAt(0);// this remove main contoller
            return View(getAllAreas);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private static IEnumerable<Type> GetSubClasses<T>()
        {
            return System.Reflection.Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        private string splitcontrollerName(string myText)
        {
            var searchStr = "Controller";
            int lastIndex = myText.LastIndexOf(searchStr);
            if (lastIndex >= 0)
                myText = myText.Substring(0, lastIndex) + myText.Substring(lastIndex + searchStr.Length);

            return myText;
        }

        private IEnumerable<MyAction> GetListOfAction(Type controller)
        {
            var navItems = new List<MyAction>();

            // Get a descriptor of this controller
            var controllerDesc = new ReflectedControllerDescriptor(controller);

            // Look at each action in the controller
            foreach (ActionDescriptor action in controllerDesc.GetCanonicalActions())
            {
                bool validAction = true;
                bool isHttpPost = false;

                // Get any attributes (filters) on the action
                object[] attributes = action.GetCustomAttributes(false);

                // Look at each attribute
                foreach (object filter in attributes)
                {
                    // Can we navigate to the action?
                    if (filter is ChildActionOnlyAttribute)
                    {
                        validAction = false;
                        break;
                    }
                    if (filter is HttpPostAttribute)
                    {
                        isHttpPost = true;
                    }

                }

                // Add the action to the list if it's "valid"
                if (validAction)
                    navItems.Add(new MyAction()
                    {
                        Name = action.ActionName,
                        IsHttpPost = isHttpPost
                    });
            }
            return navItems;
        }
    }

    public class MyAction
    {
        public string Name { get; set; }

        public bool IsHttpPost { get; set; }
    }

    public class MyController
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public IEnumerable<MyAction> MyActions { get; set; }
    }

    public class MyArea
    {
        public string Name { get; set; }

        public IEnumerable<string> Namespace { get; set; }

        public IEnumerable<MyController> MyControllers { get; set; }
    }
}