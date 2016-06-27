using UnityEngine;
using System.Collections;

public class EquipItemOnTime : StateMachineBehaviour
{
    public bool Unequip = false;
    public float TriggerOnTime = 0.5f;
    public string Item;
    public string TargetedSlot;
    private GameObject ItemObj;
    private GameObject TargetedSlotObj;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= TriggerOnTime)
        {
            if (GetChild(animator.transform.root, Item, true) != null && GetChild(animator.transform.root, TargetedSlot, true) != null)
            {
                //Only get childs just incase there is more than one player (Multiplayer)
                ItemObj = GetChild(animator.transform.root, Item, true).gameObject;
                TargetedSlotObj = GetChild(animator.transform.root, TargetedSlot, true).gameObject;

                ItemObj.transform.parent = TargetedSlotObj.transform;
                if (Unequip == false)
                {
                    Vector3 FixedPosition = ItemObj.GetComponent<WeaponOffset>().ItemEquipPositionOffset;
                    Vector3 FixedRotation = ItemObj.GetComponent<WeaponOffset>().ItemEquipRotationOffset;
                    ItemObj.transform.localPosition = FixedPosition;
                    ItemObj.transform.localRotation = Quaternion.Euler(FixedRotation.x, FixedRotation.y, FixedRotation.z);
                }
                else
                {
                    Vector3 FixedPosition = ItemObj.GetComponent<WeaponOffset>().ItemUnequipPositionOffset;
                    Vector3 FixedRotation = ItemObj.GetComponent<WeaponOffset>().ItemUnequipRotationOffset;
                    ItemObj.transform.localPosition = FixedPosition;
                    ItemObj.transform.localRotation = Quaternion.Euler(FixedRotation.x, FixedRotation.y, FixedRotation.z);
                }
            }
            else
            {
                ItemObj = null;
                TargetedSlotObj = null;
            }
        }
    }

    //Gets all childs within the hierarchy
    public static Transform GetChild(Transform inside, string wanted, bool recursive = false)
    {
        foreach (Transform child in inside)
        {
            if (child.name == wanted)
                return child;
            if (recursive)
            {
                var within = GetChild(child, wanted, true);
                if (within) return within;
            }
        }
        return null;
    }
}
