using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent (typeof (NavMeshAgent))]
public class ClickToMove : MonoBehaviour {
    private Player player;
    private NavMeshAgent agent;
    IObservable<RaycastHit> raycastHitStream;
    public ReactiveProperty<Chest> openChest { get; private set; } = new ReactiveProperty<Chest>(null);

    void Start () {
        player = SceneManager.Instance.player;
        agent = player.agent;

        raycastHitStream = this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            .Select<Unit, Ray>(_ => Camera.main.ScreenPointToRay(Input.mousePosition))
            .Where((Ray ray) => Physics.Raycast(ray.origin, ray.direction))
            .Select<Ray, RaycastHit>((Ray ray) =>
            {
                RaycastHit hit = new RaycastHit();

                Physics.Raycast(ray.origin, ray.direction, out hit);

                return hit;
            });

        raycastHitStream
            .Subscribe((RaycastHit hit) => 
            {
                NavMeshHit meshHit;
                Chest chest = hit.transform.GetComponent<Chest>();


                // chest click
                if (chest != null)
                {
                    Vector3 newDest =
                    hit.collider.gameObject.transform.position
                    + Vector3.Scale(
                        hit.collider.gameObject.transform.forward,
                        new Vector3(hit.collider.gameObject.transform.localScale.x, 0, hit.collider.gameObject.transform.localScale.z)
                        );

                    newDest.y = agent.destination.y;

                    // chest is far away schedule chest open
                    if (agent.destination != newDest)
                    {
                        agent.destination = newDest;
                        openChest.Value = chest;
                    }
                    else // chest is near, open chest
                    {
                        chest.Open();
                    }
                }
                else
                {
                    // walkable area click
                    if (NavMesh.SamplePosition(hit.point, out meshHit, 2f, 1))
                    {
                        openChest.Value = null;
                        agent.destination = hit.point;
                    }
                }
            });

        // delayed chest open
        player.navComplete()
            .Select(_ => openChest.Value)
            .Where((Chest chest) => chest != null)
            .Subscribe((Chest chest) =>
            {
                chest.Open();
            });
    }
    void Update () {

    }
}
