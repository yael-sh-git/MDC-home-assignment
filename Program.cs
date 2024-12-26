using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Security.Cryptography.X509Certificates;

namespace HomeAssignment_MDC
{
    internal class Program
    {
        /// <summary>
        /// checkes whether an Amazon EC2 instance is compliant with the "SSM.3" recommendation 
        /// (the EC2 instance managed by AWS Systems Manager should have an association compliance status of COMPLIANT).
        /// </summary>
        /// <param name="resource"> resource to evaluate </param>
        /// <returns>True if healthy and False otherwise</returns>
        public static async Task<bool> CheckSSMCompliance(Resource resource)
        {
            try
            {
                var ssmClient = new AmazonSimpleSystemsManagementClient();

                var request = new ListComplianceItemsRequest
                {
                    ResourceIds = { resource.Id },
                    ResourceTypes = { "ManagedInstance" },
                    Filters = new List<ComplianceStringFilter>
                {
                    new ComplianceStringFilter { Key = "ComplianceType", Values = new List<string> { "Association" } }
                }
                };

                var response = await ssmClient.ListComplianceItemsAsync(request);

                if (response.ComplianceItems == null || !response.ComplianceItems.Any())
                {
                    Console.WriteLine($"No associations found for instance {resource.Id}. Assuming healthy.");
                    return true; // No associations to check
                }

                foreach (var item in response.ComplianceItems)
                {
                    if (item.ComplianceType == "Association")
                    {
                        if (item.Status != ComplianceStatus.COMPLIANT)
                        {
                            Console.WriteLine($"Instance {resource.Id} has a non-compliant association. Status: {item.Status}");
                            return false;
                        }
                    }
                }

                Console.WriteLine($"All associations for instance {resource.Id} are compliant.");
                return true; // All associations are compliant
            }
            catch (AmazonSimpleSystemsManagementException ex)
            {
                Console.WriteLine($"Error checking compliance for instance {resource.Id}: {ex.Message}");
                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return false;
            }
        }
        static async Task Main(string[] args)
        {
            //Data Collector Implementation for the resource type in the first answer:

            //Adding the [SSM.3] recommendation
            Recommendation recommendation = await Recommendation.CreateRecommendation(
                "SSM.3",
                "AWS::SSM::AssociationCompliance",
                CheckSSMCompliance
                );
            RecommendationsArsenal recommendationsArsenal = new RecommendationsArsenal();
            recommendationsArsenal.AddRecommendation(recommendation);

            //Get unhealthy resource list for the resource type from 1
            string resourceType = "AWS::SSM::AssociationCompliance";
            List<Resource> unhealthyResources = await recommendationsArsenal.GetUnhealthyResource(resourceType);
        }
    }
}
