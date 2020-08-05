using System;
using System.Threading;
using RestSharp;

namespace MsGraphScript
{
    class Program
    {
        static string BearerToken = "Use client credential flow to get a token";
        static string TenantName = "contoso";
        static string ROPC_PolicyName = "B2C_1_ROPC";
        static string ROPC_ClientId = "00000000-0000-0000-0000-000000000000";

        static void Main(string[] args)
        {

            Console.WriteLine("********************** start creating users ******************");
            for (int i = 11; i < 20; i++)
            {
                CreateUser(i.ToString());
            }
        }

        static void CreateUser(string i)
        {
            var client = new RestClient("https://graph.microsoft.com/v1.0/users");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("bearerToken", BearerToken);
            request.AddHeader("Authorization", $"Bearer {BearerToken}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n  \"displayName\": \"Test user\",\n  \"identities\": [\n    {\n      \"signInType\": \"emailAddress\",\n      \"issuer\": \"" + TenantName + ".onmicrosoft.com\",\n      \"issuerAssignedId\": \"" + i + "@test.com\"\n    }\n  ],\n  \"passwordProfile\" : {\n    \"password\": \"" + i + "\",\n    \"forceChangePasswordNextSignIn\": false\n  },\n  \"passwordPolicies\": \"DisablePasswordExpiration, DisableStrongPassword\"\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            // Console.WriteLine(response.Content);

            for (int x = 0; x < 10; x++)
            {
                if (ROPC(i))
                    break;
                else
                    Thread.Sleep(1000);
            }

        }

        static bool ROPC(string i)
        {
            var client = new RestClient($"https://{TenantName}.b2clogin.com/{TenantName}.onmicrosoft.com/{ROPC_PolicyName}/oauth2/v2.0/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.AlwaysMultipartFormData = true;
            request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
            request.AddParameter("username", $"{i}@test.com", ParameterType.GetOrPost);
            request.AddParameter("password", i, ParameterType.GetOrPost);
            request.AddParameter("client_id", ROPC_ClientId, ParameterType.GetOrPost);
            request.AddParameter("scope", ROPC_ClientId, ParameterType.GetOrPost);
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);

            if (response.Content.Contains("error"))
            {
                Console.WriteLine(i + " error");
                return false;
            }
            else
            {
                Console.WriteLine(i + " success");
                return true;
            }
        }
    }
}
