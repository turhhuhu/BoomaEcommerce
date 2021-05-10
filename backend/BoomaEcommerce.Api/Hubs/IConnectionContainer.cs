using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api.Hubs
{
    public interface IConnectionContainer
    {
        public void SaveConnection(Guid userGuid, string connectionId);
        public string GetConnectionId(Guid userGuid);
        public void RemoveConnection(Guid useGuid);

    }
}
