using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Level.Covers.Classes
{
    public class CoverClusterMaker
    {
        private readonly int clustersCount;
        private readonly List<Cover> covers;

        private readonly Vector3[] centroids;
        private readonly int[] clusterIds;

        private readonly Random random;

        public CoverClusterMaker(int clustersCount, List<Cover> covers)
        {
            this.clustersCount = clustersCount;
            this.covers = covers;

            centroids = new Vector3[clustersCount];
            clusterIds = new int[covers.Count];

            random = new Random(clustersCount);
        }

        public List<int> Clusterize(out List<Vector3> centers)
        {
            RandomizeCentroids();
            while (MoveCentroidsToCenters() > 0.01f)
            {
                UpdateCoversClustersIds();
            }

            UpdateCoversClustersIds();

            centers = centroids.ToList();
            return clusterIds.ToList();
        }

        private void RandomizeCentroids()
        {
            Vector3 maxBound = new Vector3();
            Vector3 minBound = new Vector3();
            for (int i = 0; i < covers.Count; i++)
            {
                Vector3 p = covers[i].position;

                if (maxBound.x > p.x)
                    maxBound.x = p.x;
                if (maxBound.y > p.y)
                    maxBound.y = p.y;
                if (maxBound.z > p.z)
                    maxBound.z = p.z;

                if (minBound.x < p.x)
                    minBound.x = p.x;
                if (minBound.y < p.y)
                    minBound.y = p.y;
                if (minBound.z < p.z)
                    minBound.z = p.z;
            }

            for (int i = 0; i < clustersCount; i++)
            {
                centroids[i] = RandomVector3(minBound, maxBound);
            }
        }

        private void UpdateCoversClustersIds()
        {
            for (int i = 0; i < covers.Count; i++)
            {
                Vector3 position =
                    covers[i].position;
                float minDistance = float.MaxValue;
                int closestCentroid = 0;
                for (int j = 0; j < clustersCount; j++)
                {
                    Vector3 centroid = centroids[j];
                    float distance = Vector3.Distance(position, centroid);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestCentroid = j;
                    }
                }

                clusterIds[i] = closestCentroid;
            }
        }

        private float MoveCentroidsToCenters()
        {
            float delta = 0.0f;
            for (int i = 0; i < clustersCount; i++)
            {
                Vector3 position = new Vector3();
                int count = 0;
                for (int j = 0; j < covers.Count; j++)
                {
                    if (clusterIds[j] == i)
                    {
                        position += covers[j].position;
                        count++;
                    }
                }

                if (count == 0)
                    count = 1;
                delta += (centroids[i] - position / count).magnitude;
                centroids[i] = position / count;
            }

            return delta;
        }

        private Vector3 RandomVector3(Vector3 min, Vector3 max)
        {
            float x = min.x + (float) random.NextDouble() * (max.x - min.x);
            float y = min.y + (float) random.NextDouble() * (max.y - min.y);
            float z = min.z + (float) random.NextDouble() * (max.z - min.z);

            return new Vector3(x, y, z);
        }
    }
}