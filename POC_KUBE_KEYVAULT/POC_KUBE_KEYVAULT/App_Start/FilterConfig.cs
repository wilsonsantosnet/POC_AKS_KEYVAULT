using System.Web;
using System.Web.Mvc;

namespace POC_KUBE_KEYVAULT
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
