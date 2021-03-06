using UnityEngine;

namespace IK
{
    public class IK : MonoBehaviour
    {
    
        public bool autoSpawnTargets = false;
        public bool autoInit = true;

        public int chainLength = 2;

        public Transform target;

        public Transform pole;

        [Header("Solver Parameters")] public int iterations = 10;

        public float delta = 0.001f;

        [Range(0, 1)] public float snapBackStrength = 1f;


        private float[] bonesLength; //Target to Origin
        private float completeLength;
        private Transform[] bones;
        private Vector3[] positions;
        private Vector3[] startDirectionSucc;
        private Quaternion[] startRotationBone;
        private Quaternion startRotationTarget;
        private Transform root;

        //some creepy stuff to make it work on Unity 5
        public const float kEpsilonNormalSqrt = 1e-15F;
        void Start()
        {
            if (autoInit)
                Init();
        }

        public void Init()
        {
            bones = new Transform[chainLength + 1];
            positions = new Vector3[chainLength + 1];
            bonesLength = new float[chainLength];
            startDirectionSucc = new Vector3[chainLength + 1];
            startRotationBone = new Quaternion[chainLength + 1];

            root = transform;
            for (var i = 0; i <= chainLength; i++)
            {
                if (!root)
                    throw new UnityException("The chain value is longer than the ancestor chain!");
                root = root.parent;
            }

            if (!target && autoSpawnTargets)
            {
                target = new GameObject(gameObject.name + " Target").transform;
                SetPositionRootSpace(target, GetPositionRootSpace(transform));
            }

            startRotationTarget = GetRotationRootSpace(target);

            var current = transform;
            completeLength = 0;
            for (var i = bones.Length - 1; i >= 0; i--)
            {
                bones[i] = current;
                startRotationBone[i] = GetRotationRootSpace(current);

                if (i == bones.Length - 1)
                {
                    startDirectionSucc[i] = GetPositionRootSpace(target) - GetPositionRootSpace(current);
                }
                else
                {
                    startDirectionSucc[i] = GetPositionRootSpace(bones[i + 1]) - GetPositionRootSpace(current);
                    bonesLength[i] = startDirectionSucc[i].magnitude;
                    completeLength += bonesLength[i];
                }

                current = current.parent;
            }
        }

        void LateUpdate()
        {
            ResolveIK();
        }

        private void ResolveIK()
        {
            if (!target)
                return;

            if (bonesLength.Length != chainLength)
                Init();
            for (int i = 0; i < bones.Length; i++)
                positions[i] = GetPositionRootSpace(bones[i]);

            var targetPosition = GetPositionRootSpace(target);
            var targetRotation = GetRotationRootSpace(target);


            if ((targetPosition - GetPositionRootSpace(bones[0])).sqrMagnitude >= completeLength * completeLength)
            {
                var direction = (targetPosition - positions[0]).normalized;

                for (int i = 1; i < positions.Length; i++)
                    positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
            }
            else
            {
                for (int i = 0; i < positions.Length - 1; i++)
                    positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + startDirectionSucc[i],
                        snapBackStrength);

                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    for (int i = positions.Length - 1; i > 0; i--)
                    {
                        if (i == positions.Length - 1)
                            positions[i] = targetPosition;
                        else
                            positions[i] = positions[i + 1] +
                                           (positions[i] - positions[i + 1]).normalized *
                                           bonesLength[i];
                    }


                    for (int i = 1; i < positions.Length; i++)
                        positions[i] = positions[i - 1] +
                                       (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];

                    if ((positions[positions.Length - 1] - targetPosition).sqrMagnitude < delta * delta)
                        break;
                }
            }

            //move towards pole
            if (pole)
            {
                var polePosition = GetPositionRootSpace(pole);
                for (int i = 1; i < positions.Length - 1; i++)
                {
                    Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                    Vector3 projectedPole =
                        ClosestPointOnPlane(plane, polePosition);
                    Vector3 projectedBone = ClosestPointOnPlane(plane, positions[i]);
                    float angle = SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1],
                        plane.normal);
                    positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) +
                                   positions[i - 1];
                }
            }

            for (int i = 0; i < positions.Length; i++)
            {
                if (i == positions.Length - 1)
                    SetRotationRootSpace(bones[i],
                        Quaternion.Inverse(targetRotation) * startRotationTarget *
                        Quaternion.Inverse(startRotationBone[i]));
                else
                    SetRotationRootSpace(bones[i],
                        Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) *
                        Quaternion.Inverse(startRotationBone[i]));
                SetPositionRootSpace(bones[i], positions[i]);
            }
        }

        private Vector3 GetPositionRootSpace(Transform current)
        {
            if (!root)
                return current.position;
            return Quaternion.Inverse(root.rotation) * (current.position - root.position);
        }

        private void SetPositionRootSpace(Transform current, Vector3 position)
        {
            if (!root)
                current.position = position;
            else
                current.position = root.rotation * position + root.position;
        }

        private Quaternion GetRotationRootSpace(Transform current)
        {
            if (!root)
                return current.rotation;
            return Quaternion.Inverse(current.rotation) * root.rotation;
        }

        private void SetRotationRootSpace(Transform current, Quaternion rotation)
        {
            if (!root)
                current.rotation = rotation;
            else
                current.rotation = root.rotation * rotation;
        }

        public float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            float unsignedAngle = Angle(from, to);

            float crossX = from.y * to.z - from.z * to.y;
            float crossY = from.z * to.x - from.x * to.z;
            float crossZ = from.x * to.y - from.y * to.x;
            float sign = Mathf.Sign(axis.x * crossX + axis.y * crossY + axis.z * crossZ);
            return unsignedAngle * sign;
        }

        public Vector3 ClosestPointOnPlane(Plane plane, Vector3 point)
        {
            float pointToPlaneDistance = Vector3.Dot(plane.normal, point) + plane.distance;
            return point - (plane.normal * pointToPlaneDistance);
        }

    

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public float Angle(Vector3 from, Vector3 to)
        {
            float denominator = (float) Mathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = Mathf.Clamp(Dot(from, to) / denominator, -1F, 1F);
            return ((float) Mathf.Acos(dot)) * Mathf.Rad2Deg;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var current = transform;
            for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
            {
                Gizmos.DrawLine(current.position, current.parent.position);
                current = current.parent;
            }
        }
    }
}