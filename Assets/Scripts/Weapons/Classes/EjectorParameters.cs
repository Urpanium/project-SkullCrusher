using System;

namespace Weapons.Parameters
{
    [Serializable]
    public class EjectorParameters
    {
        public float shellMinimumVelocity;
        public float shellMaximumVelocity;

        public float shellMinimumAngularVelocity;
        public float shellMaximumAngularVelocity;
    }
}