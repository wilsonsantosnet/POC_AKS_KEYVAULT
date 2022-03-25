using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace POC_KUBE_KEYVAULT.Controllers
{
    public class HomeController : Controller
    {

        private string kv_clientId;
        private string kv_secretId;
        private string kv_url;
        public HomeController()
        {
            this.kv_clientId = "ea7076c5-1712-4ea3-9c94-36223130eb07";
            this.kv_secretId = "L.z7Q~mIN1iEkEc5n.UYG9Yy30igdvKDLTVFZ";
            this.kv_url = "https://pockubekeyvault.vault.azure.net/";
        }

        public async Task<ActionResult> Index()
        {
            var displayKeyKv = await GetConnectionString(this.kv_clientId, this.kv_secretId, this.kv_url);
            var displayCertIdKv = await GetCertificate(this.kv_clientId, this.kv_secretId, this.kv_url);

            var displayKeyAks = GetSecretFromEnvironment();
            var displayMapsAks = GetMapsFromEnvironment();
            var displayAppSettings = GetMapsFromAppSettings();


            ViewBag.Display = $"{displayKeyKv} | {displayCertIdKv} | {displayKeyAks} | {displayMapsAks} | {displayAppSettings}";
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


        #region Helpers
       

        private static async Task<string> GetConnectionString(string kv_clientId, string kv_secretId, string kv_url)
        {
            var client = GetClientKeyVault(kv_clientId, kv_secretId);
            var key = await client.GetSecretAsync(kv_url, "ServiceBusCns");
            return $"Secret From KV: {key.Value}";
        }

        private static async Task<string> GetCertificate(string kv_clientId, string kv_secretId, string kv_url)
        {
            var client2 = GetClientKeyVault(kv_clientId, kv_secretId);

            var certificate = await client2.GetCertificateAsync(kv_url, "aks");
            return $"Cert From KV: {certificate.Id}";
        }

        private static string GetSecretFromEnvironment()
        {
            return $"Secret From AKS: {Environment.GetEnvironmentVariable("testekey")}";
        }

        private static string GetMapsFromEnvironment()
        {
            return $"Maps From AKS: {Environment.GetEnvironmentVariable("cns")}";
        }

        private static string GetMapsFromAppSettings()
        {
            return $"App Settings: {System.Configuration.ConfigurationManager.AppSettings["cns"]}";
        }

        private static async Task<string> GetConnectionStringEnvironment(string kv_url)
        {
            //AZURE_CLIENT_SECRET
            //AZURE_CLIENT_ID
            //AZURE_TENANT_ID

            var client = new SecretClient(
                vaultUri: new Uri(kv_url),
                credential: new EnvironmentCredential()
            );

            var key = await client.GetSecretAsync("ServiceBusCns");
            return $"Secret From KV: {key.Value}";
        }

        private static KeyVaultClient GetClientKeyVault(string kv_clientId, string kv_secretId)
        {
            return new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                async (string auth, string resp, string scope) =>
                {

                    var authCtx = new AuthenticationContext(auth);
                    var credencial = new ClientCredential(kv_clientId, kv_secretId);
                    var result = await authCtx.AcquireTokenAsync(resp, credencial);
                    if (result == null)
                        throw new InvalidCastException("Erro");

                    return result.AccessToken;

                }));
        }

        #endregion
    }
}