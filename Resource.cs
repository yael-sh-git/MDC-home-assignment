using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssignment_MDC
{
    public class Resource
    {
        public string Id { get; set;}
        public string ResourceType { get; set; }
        private Resource(string id, string resourceType)
        {
            Id = id;
            ResourceType = resourceType;
        }

        public static async Task<Resource> CreateResource(string id, string resourceType)
        {
            if (string.IsNullOrEmpty(resourceType) || !await ResourceValidator.IsValidResourceTypeAsync(resourceType))
                throw new InvalidOperationException($"Invalid resource type: {resourceType}");
            return new Resource(id, resourceType);
        }
    }
}
