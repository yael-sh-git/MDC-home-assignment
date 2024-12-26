using Amazon.ResourceExplorer2;
using Amazon.ResourceExplorer2.Model;
using Amazon.ResourceGroupsTaggingAPI;
using Amazon.ResourceGroupsTaggingAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssignment_MDC
{
    public class RecommendationsArsenal
    {
        private readonly List<Recommendation> recommendations;

        public RecommendationsArsenal()
        {
            recommendations = new List<Recommendation>();
        }

        public void AddRecommendation(Recommendation recommendation)
        {
            recommendations.Add(recommendation);
        }

        public async Task<bool> IsResourceHealthy(Resource resource)
        {
            var relevantRecommendations = recommendations.Where(r => r.ResourceType == resource.ResourceType);

            foreach(Recommendation recommendation in relevantRecommendations)
            {
                if(!await recommendation.CheckHealth(resource))
                    return false;
            }

            return true;
        }

        public async Task<List<Resource>> GetResourcesByTypeAsync(string resourceType)
        {
            List<Resource> resources = new List<Resource>();

            try
            {
                var request = new ListResourcesRequest
                {
                    Filters = new SearchFilter { FilterString = $"resourcetype:{resourceType.Split("::")[1].Replace("::", ":").ToLower()}" }
                };
                var response = await ResourceValidator.Client.ListResourcesAsync(request);

                foreach (var resource in response.Resources)
                {
                    resources.Add(await Resource.CreateResource(resource.Arn, resource.ResourceType));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching resources of type {resourceType}: {ex.Message}");
            }
            return resources;
        }

        public async Task<List<Resource>> GetUnhealthyResource(string resourceType)
        {
            List<Resource> resources = await GetResourcesByTypeAsync(resourceType);
            List<Resource> unhealthyResources = new List<Resource>();

            foreach (Resource resource in resources)
            {
                if(! await IsResourceHealthy(resource))
                    unhealthyResources.Add(resource);
            }
            return unhealthyResources;
        }
    }
}
