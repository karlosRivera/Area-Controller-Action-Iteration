using System.Web.Mvc;

namespace AreaTest.Areas.CSR
{
    public class CSRAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CSR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CSR_default",
                "CSR/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                , namespaces: new[] { "AreaTest.Areas.CSR.Controllers" }
            );
        }
    }
}