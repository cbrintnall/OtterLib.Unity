using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public static class NavigationUtilities
{
    public static bool IsAtDestination(this NavMeshAgent a)
    {
        if (!a.pathPending)
        {
            if (a.remainingDistance <= a.stoppingDistance)
            {
                if (!a.hasPath || Mathf.Approximately(a.velocity.sqrMagnitude, 0f))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsWithinDistanceOfDestination(this NavMeshAgent a, float dist)
    {
        return a.remainingDistance <= dist;
    }

    public static float TotalLength(this NavMeshPath path) => path.corners.Sum(c => c.magnitude);
}
