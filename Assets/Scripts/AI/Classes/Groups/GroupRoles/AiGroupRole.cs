using UnityEngine;

namespace AI.Classes.Groups.GroupRoles
{
    public abstract class AiGroupRole
    {
        public string name = "AiGroupRole";

        public abstract void CanBeAssigned(AiBot bot);
    }
}