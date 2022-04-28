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
    private bool isEnemy;

    public ReactiveProperty<Chest> openChest { get; private set; } = new ReactiveProperty<Chest>(null);
    public ReactiveProperty<Enemy> attackEnemy { get; private set; } = new ReactiveProperty<Enemy>(null);

    void Start () {
        player = MainManager.Instance.player;
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
                Enemy enemy = hit.transform.GetComponent<Enemy>();

                // chest click
                if (chest != null)
                {
                    Vector3 newDest = hit.collider.gameObject.transform.position
                    + Vector3.Scale(
                        hit.collider.gameObject.transform.forward,
                        new Vector3(hit.collider.gameObject.transform.localScale.x, 0, hit.collider.gameObject.transform.localScale.z)
                        );

                    newDest.y = agent.destination.y;

                    // chest is far away schedule chest open
                    if (Vector3.Distance(newDest, agent.destination) > 5f)
                    {
                        player.setNextPosition(newDest);
                        openChest.Value = chest;
                    }
                    else // chest is near, open chest
                    {
                        chest.Open();
                    }
                } else if (enemy != null) // enemy click
                {
                    Vector3 newDest = hit.collider.gameObject.transform.position
                    + Vector3.Scale(
                        hit.collider.gameObject.transform.forward * 2f,
                        new Vector3(hit.collider.gameObject.transform.localScale.x, 0, hit.collider.gameObject.transform.localScale.z)
                        );

                    newDest.y = agent.destination.y;

                    // enemy is far away schedule attack
                    if (Vector3.Distance(newDest, agent.destination) > 5f)
                    {
                        player.setAttackTarget(enemy, newDest);
                        attackEnemy.Value = enemy;
                    }
                    else // enemy is near, attack
                    {
                        player.setAttackTarget(enemy);
                    }
                } else
                {
                    // walkable area click
                    if (NavMesh.SamplePosition(hit.point, out meshHit, 2f, 1))
                    {
                        openChest.Value = null;
                        attackEnemy.Value = null;

                        if (Vector3.Distance(hit.point, agent.destination) > 1f)
                        {
                            player.setNextPosition(hit.point);
                        }
                    }
                }
            });

        // delayed chest open
        player.navComplete()
            .Select(_ => openChest.Value)
            .Where((Chest chest) => chest != null)
            .Subscribe((Chest chest) =>
            {
                player.transform.LookAt(chest.transform);
                chest.Open();
            });

        // delayed enemy attack
        player.navComplete()
            .Select(_ => attackEnemy.Value)
            .Where((Enemy enemy) => enemy != null)
            .Subscribe((Enemy enemy) =>
            {


                Vector3 look = enemy.transform.position
                    - Vector3.Scale(
                        enemy.transform.forward,
                        new Vector3(enemy.transform.localScale.x, 0, enemy.transform.localScale.z)
                        );

                Debug.DrawLine(look, new Vector3(look.x, 50, look.z), Color.red, 60f);
                player.setAttackTarget(enemy);
            });
    }
    void Update () {

    }
}
