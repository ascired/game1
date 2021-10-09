using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class ClickToMove : MonoBehaviour {
    RaycastHit hitInfo = new RaycastHit();
    NavMeshAgent agent;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
    }
    void Update () {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            {
                Chest chest = hitInfo.transform.GetComponent<Chest>();

                if (chest != null)
                {
                    agent.destination = hitInfo.collider.gameObject.transform.position + hitInfo.collider.gameObject.transform.forward;
                    chest.Open();
                }
                else 
                {
                    agent.destination = hitInfo.point;
                }

            }
        }
    }
}
