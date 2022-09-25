using Apteryx.MongoDB.Driver.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apteryx.Routing.Role.Authority
{
    public class Role:BaseMongoEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> RouteIds { get; set; }
    }
}
