using Amazon.ResourceExplorer2;
using Amazon.ResourceExplorer2.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssignment_MDC
{
    public static class ResourceValidator
    {
        private static readonly List<string> validResourceTypes;
        //private static List<string> validResourceTypes = new List<string>
        //{
        //    "AWS::SSM::AssociationCompliance",
        //    "AWS::EC2::Instance",
        //    "AWS::SSM::ManagedInstance",
        //};
        private static readonly ILogger<ResourceValidator> _logger;
        static ResourceValidator()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ResourceValidator>();
        }
        private static AmazonResourceExplorer2Client _client = new AmazonResourceExplorer2Client();
        public static AmazonResourceExplorer2Client Client => _client;

        private static async Task FetchValidResourceTypesAsync()
        {
            try
            {
                var response = await _client.ListSupportedResourceTypesAsync(new ListSupportedResourceTypesRequest());

                foreach (var resourceType in response.ResourceTypes)
                {
                    validResourceTypes.Add(resourceType.ResourceType);
                }
            }
            catch (AmazonServiceException ex)
            {
                Console.WriteLine($"Error fetching resource types: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching resource types: {ex.Message}");
                throw;
            }
        }

        private static async Task<List<string>> GetValidResourceTypesAsync()
        {
            if(validResourceTypes.Count>0)
                return validResourceTypes;

            Console.WriteLine("after");

            await FetchValidResourceTypesAsync();
            return validResourceTypes;
        }

        public static async Task<bool> IsValidResourceTypeAsync(string resourceType)
        {
            try
            {
                var validTypes = await GetValidResourceTypesAsync();
                return validTypes.Contains(resourceType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating resource type '{resourceType}': {ex.Message}");
                return false; // Returns false if we failed to check the validation
            }
        }
    }
}
