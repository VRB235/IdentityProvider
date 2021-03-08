using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using IdentityServer.Models;

namespace IdentityServer.Services
{
    public static class BannerRestAPI
    {
        public static string userBanner = Startup._configuration["appSettings:USERBANNERSERVICE"];

        public static string passwordBanner = Startup._configuration["appSettings:PASSWORDBANNERSERVICE"];
        public async static Task<UcabUserAuth> AuthenticateUser(string usuario, string pass)
        {
            try
            {
                UcabUserAuth result = await Startup._configuration["appSettings:URLBANNERRESTAPI"]
                                .AppendPathSegment("v1")
                                .AppendPathSegment("ucab")
                                .AppendPathSegment("users")
                                .AppendPathSegment("auth/")
                                .WithBasicAuth(userBanner, passwordBanner)
                                .PostJsonAsync
                                (
                                    new
                                    {
                                        uid = usuario,
                                        password = pass
                                    }
                                )
                                .ReceiveJson<UcabUserAuth>();
                return result;
            }
            catch (FlurlHttpException e)
            {
                Console.WriteLine("Error de Flurl al validar credenciales de Usuario -> {user}", usuario);
                Console.WriteLine("Error -> {Message}", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al consumir servicio de Banner -> {0}", Startup._configuration["appSettings:URLBANNERRESTAPI"]);
                Console.WriteLine("Error -> {Message}", e.Message);
                return null;
            }
        }

    }
}
