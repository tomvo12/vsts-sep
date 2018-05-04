using System;

namespace CreateServiceEndpoint
{
    class Program
    {
        private const string VstsBaseUrl = "https://YOUR-ACCOUNT.visualstudio.com";
        private const string sepName = "SEP-NAME";
        private const string projectName = "YOUR-PROJECT";

        private static void Main()
        {
            var projectId = VstsTasks.GetId(VstsBaseUrl, projectName).Result;

            var exists = VstsTasks.ServiceEndpointExists(VstsBaseUrl, projectId, sepName)
                .Result;
            if (!exists)
            {
                var sepId = VstsTasks.CreateAppCenterServiceEndpoint(VstsBaseUrl, projectId, sepName, "secretValue")
                    .Result;

                Console.WriteLine($"Created service endpoint '{sepName}' with id: {sepId}");
            }
            else
            {
                var sepId = VstsTasks.GetServiceEndpointId(VstsBaseUrl, projectId, sepName).Result;
                VstsTasks.DeleteServiceEndpoint(VstsBaseUrl, projectId, sepId.Value).Wait();
                Console.WriteLine($"Deleted service endpoint '{sepName}' with id: {sepId}");
            }
        }
    }
}
