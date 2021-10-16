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

                Chest chest = hit.transform.GetComponent<Chest>();

                if (chest != null)
                {
                    Vector3 newDest = hit.collider.gameObject.transform.position + hit.collider.gameObject.transform.forward * 5.5f;
                    newDest.y = agent.destination.y;

                    if (agent.destination != newDest)
                    {
                        agent.destination = newDest;
                        openChest.Value = chest;
                    }
                    else 
                    {
                        chest.Open();
                    }
                }
                else
                {
                    openChest.Value = null;
                    agent.destination = hit.point;
                }
            });

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
