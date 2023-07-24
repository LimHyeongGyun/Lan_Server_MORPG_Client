using UnityEngine;
using UnityEngine.AI;

public sealed class DEBUG_NAV : MonoBehaviour
{
    private NavMeshAgent agent;

    public void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        Debug.DrawRay(this.agent.nextPosition, Vector3.up * 4, Color.black, 0f, false);
        Debug.DrawRay(this.agent.destination, Vector3.up * 4, Color.black, 0f, false);
        Debug.DrawRay(this.agent.transform.position, Vector3.up * 4, Color.black, 0f, false);
        Debug.DrawLine(this.agent.nextPosition, this.agent.transform.position, Color.yellow, 0f, false);
        Debug.DrawLine(this.agent.nextPosition, this.agent.destination, Color.yellow, 0f, false);
        Debug.DrawLine(this.agent.destination, this.agent.transform.position, Color.yellow, 0f, false);
        var path = this.agent.path;
        if (path.corners.Length >= 2)
        {
            foreach (var e in path.corners)
            {
                Debug.DrawRay(e, Vector3.up * 5, Color.green, 0f, false);
            }
            Vector3 b = path.corners[1],
                a = path.corners[0];
            Vector3 dir = new Vector3(b.x - a.x, b.z - a.z);
            Debug.DrawRay(a, dir * 10, Color.red, 0f, false);
        }
        else
        {
            Debug.DrawRay(path.corners[0], Vector3.up * 5, Color.blue, 0f, false);
        }
    }
}