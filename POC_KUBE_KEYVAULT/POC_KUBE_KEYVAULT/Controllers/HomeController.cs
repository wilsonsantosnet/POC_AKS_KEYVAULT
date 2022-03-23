using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace POC_KUBE_KEYVAULT.Controllers
{
    public class HomeController : Controller
    {

        private static async Task<string> GetConnectionString()
        {

            var kv_clientId = "ea7076c5-1712-4ea3-9c94-36223130eb07";
            var kv_secretId = "L.z7Q~mIN1iEkEc5n.UYG9Yy30igdvKDLTVFZ";
            var kv_url = "https://pockubekeyvault.vault.azure.net/";


            var client2 = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                async (string auth, string resp, string scope) =>
                {

                    var authCtx = new AuthenticationContext(auth);
                    var credencial = new ClientCredential(kv_clientId, kv_secretId);
                    var result = await authCtx.AcquireTokenAsync(resp, credencial);
                    if (result == null)
                        throw new InvalidCastException("Erro");
                    return result.AccessToken;

                }));

            var key2 = await client2.GetSecretAsync(kv_url, "ServiceBusCns");
            return key2.Value;
        }
        public async Task<ActionResult> Index()
        {
            var teste = await GetConnectionString();
            ViewBag.Secret = teste;
            return View();
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
    }
}