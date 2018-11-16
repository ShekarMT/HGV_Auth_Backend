using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HGVServiceAuth.Helpers
{
    public static class GraphServiceProvider
    {
        public static GraphServiceClient ClientProvider(string accessToken)
        {
            
            //AuthenticationContext authContext = new AuthenticationContext(ConfigurationReader.AppSetting["Authentication:AzureAd:AADInstance"] + ConfigurationReader.AppSetting["Authentication:AzureAd:TenantId"]);
            //var accessToken = authContext.AcquireTokenAsync(ConfigurationReader.AppSetting["Authentication:AzureAd:GraphEndpoint"], new ClientCredential(ConfigurationReader.AppSetting["Authentication:AzureAd:Audience"], ConfigurationReader.AppSetting["Authentication:AzureAd:SecretKey"])).Result.AccessToken;

            if (!string.IsNullOrEmpty(accessToken))
            {
                return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    requestMessage =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken.Split(" ")[1].ToString());

                        return Task.FromResult(0);
                    }));
            }
            else
            {
                return null;
            }
        }
    }
}
