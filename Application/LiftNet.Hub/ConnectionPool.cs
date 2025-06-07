using System.Threading;

namespace LiftNet.Hub
{
    public class ConnectionPool
    {
        private static readonly Dictionary<string, Dictionary<string, HashSet<string>>> _userConnections = new();
        private readonly object _lock = new();

        public void AddConnection(string userId, string connectionId, string hubName)
        {
            lock (_lock)
            {
                if (!_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId] = new Dictionary<string, HashSet<string>>();
                }

                if (!_userConnections[userId].ContainsKey(hubName))
                {
                    _userConnections[userId][hubName] = new HashSet<string>();
                }

                _userConnections[userId][hubName].Add(connectionId);
            }
        }

        public void RemoveConnection(string userId, string connectionId, string hubName)
        {
            if (_userConnections.ContainsKey(userId) && _userConnections[userId].ContainsKey(hubName))
            {
                _userConnections[userId][hubName].Remove(connectionId);

                if (_userConnections[userId][hubName].Count == 0)
                {
                    _userConnections[userId].Remove(hubName);
                }

                if (_userConnections[userId].Count == 0)
                {
                    _userConnections.Remove(userId);
                }
            }
        }

        public IEnumerable<string> GetUserConnectionsByHub(string userId, string hubName)
        {
            return _userConnections.ContainsKey(userId) && _userConnections[userId].ContainsKey(hubName)
                ? _userConnections[userId][hubName]
                : Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetAllConnectionsByHub(string hubName)
        {
            var allConnections = new List<string>();

            foreach (var userConnections in _userConnections)
            {
                if (userConnections.Value.ContainsKey(hubName))
                {
                    allConnections.AddRange(userConnections.Value[hubName]);
                }
            }

            return allConnections;
        }
    }
}
