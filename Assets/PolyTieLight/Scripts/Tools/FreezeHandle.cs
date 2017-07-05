using UnityEngine;
using System.Collections;

public class FreezeHandle : MonoBehaviour 
{
    public bool Freeze;

    private SpriteRenderer _sprite;

    public void UpdateFreeze()
    {
        if (Freeze == true)
        {
            affectChildren();
        }
        else
        {
            var childFreezes = GetComponentsInChildren<FreezeHandle>();
            for (int i = 0; i < childFreezes.Length; i++)
            {
                if (childFreezes[i] == this)
                    continue;

                if (childFreezes[i].Freeze == true)
                {
                    childFreezes[i].Freeze = false;
                    childFreezes[i].UpdateFreeze();
                }                
                DestroyImmediate(childFreezes[i]);
            }
        }
    }

    private void affectChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var childFreeze = child.GetComponent<FreezeHandle>();
            if (childFreeze == null)
            {
                childFreeze = child.gameObject.AddComponent<FreezeHandle>();
                childFreeze.Freeze = true;
            }
            else
            {
                childFreeze.Freeze = true;
            }
        }
    }
}
