using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api.Hubs
{
    public class ConnectionContainer : IConnectionContainer
    {
        private readonly ConcurrentDictionary<Guid, string> _userGuidToConnectionIdMapping;

        public ConnectionContainer()
        {
            _userGuidToConnectionIdMapping = new ConcurrentDictionary<Guid, string>();
        }

        public void SaveConnection(Guid userGuid, string connectionId)
        {
            _userGuidToConnectionIdMapping[userGuid] = connectionId;
        }

        public string GetConnectionId(Guid userGuid)
        {
            return _userGuidToConnectionIdMapping.TryGetValue(userGuid, out var connectionId) 
                ? connectionId 
                : null;
        }

        public void RemoveConnection(Guid useGuid)
        { 
            _userGuidToConnectionIdMapping.Remove(useGuid, out _);
        }
    }
}
