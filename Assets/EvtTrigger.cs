using UnityEngine;
using UnityEngine.EventSystems;

public class EvtTrigger : EventTrigger
{
    public delegate void DelegateCallback(int id);

    public int id;

    public override void OnPointerDown(PointerEventData data)
    {
        AddSelectedItem();
    }

    private void AddSelectedItem() 
    {
        ItemsManager.Instance.addItemToPlayer(id);
        gameObject.SetActive(false);
    }
}