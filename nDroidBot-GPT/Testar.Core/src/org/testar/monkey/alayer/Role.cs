using System.Collections.Concurrent;
using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class Role
    {
        private static readonly ConcurrentDictionary<string, Role> existingRoles = new();
        private readonly HashSet<Role> parentsSet;
        private readonly HashSet<Role> ancestorsSet;
        private readonly string roleName;
        private int hashcode;

        public static bool isOneOf(Role role, params Role[] oneOf)
        {
            Assert.notNull(role, oneOf);
            foreach (var candidate in oneOf)
            {
                if (role.isA(candidate))
                {
                    return true;
                }
            }

            return false;
        }

        public static Role from(string name, params Role[] inheritFrom)
        {
            Assert.notNull(name, inheritFrom);
            if (existingRoles.TryGetValue(name, out var existing))
            {
                return existing;
            }

            var created = new Role(name, inheritFrom);
            existingRoles[name] = created;
            return created;
        }

        private Role(string name, params Role[] inheritFrom)
        {
            roleName = name;
            parentsSet = new HashSet<Role>();
            ancestorsSet = new HashSet<Role>();

            foreach (var parent in inheritFrom)
            {
                parentsSet.Add(parent);
                CalculateAncestors(parent.parents(), ancestorsSet);
            }

            parentsSet.ExceptWith(ancestorsSet);
            ancestorsSet.UnionWith(parentsSet);
        }

        public IReadOnlyCollection<Role> parents() => parentsSet;
        public IEnumerable<Role> ancestorsList() => ancestorsSet;
        public bool isA(Role other) => Equals(other) || ancestorsSet.Contains(other);
        public string name() => roleName;

        public override string ToString() => roleName;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Role other)
            {
                return roleName == other.roleName && parentsSet.SetEquals(other.parentsSet);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (hashcode == 0)
            {
                hashcode = roleName.GetHashCode() + 31 * parentsSet.GetHashCode();
            }

            return hashcode;
        }

        private static void CalculateAncestors(IEnumerable<Role> parentSet, HashSet<Role> outSet)
        {
            foreach (var parent in parentSet)
            {
                if (outSet.Add(parent))
                {
                    CalculateAncestors(parent.parents(), outSet);
                }
            }
        }
    }
}
