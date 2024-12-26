using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssignment_MDC
{
    public class Recommendation
    {
        public string Id { get;}
        public string ResourceType { get;}
        public Func<Resource,Task<bool>> CheckHealth { get; }
        private Recommendation(string id, string resourceType, Func<Resource, Task<bool>> checkHealth)
        {
            Id = id;
            ResourceType = resourceType;
            CheckHealth = checkHealth;
        }
        public static async Task<Recommendation> CreateRecommendation (string id, string resourceType, Func<Resource, Task<bool>> checkHealth)
        {
            if (string.IsNullOrEmpty(resourceType) || !ResourceValidator.IsValidResourceTypeAsync(resourceType).Result)
                throw new InvalidOperationException($"Invalid resource type: {resourceType}");
            return new Recommendation(id, resourceType, checkHealth);
        }
    }
}
