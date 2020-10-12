using System;
using System.Collections.Generic;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;

namespace L03
{
    class Program
    {
        private static DriveService _service;
         private static string _token;
        

        static void Main(string[] args)
        {
            Program.init();
        }
        static void init()
        {
            string[] scop=new string[]
            {
                DriveService.Scope.Drive,
                 DriveService.Scope.DriveFile
            };
            var clientID="702973582013-rmhn0q74sidbbk62dmoa7psj9cvubnfl.apps.googleusercontent.com";
            var clientSecret="eqQLpd2f7ks3JR9J-DIXsqio";

             var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                 new ClientSecrets
                 {
                     ClientId=clientID,
                     ClientSecret=clientSecret
                 },
                 scop,
                 Environment.UserName,
                 CancellationToken.None,

                 null
             ).Result;
            _service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            
            });
            _token=credential.Token.AccessToken;
            Console.Write("Token:" + credential.Token.AccessToken);
                 
        }

    }
}
