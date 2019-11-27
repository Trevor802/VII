using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VII
{
    public class Inventory
    {
        public List<Item> items = new List<Item>();

        public void AddItem(Item i_Item)
        {
            items.Add(i_Item);
        }

        public bool ContainItem(Item i_Item)
        {
            return items.Contains(i_Item);
        }

        public void RemoveItem(Item i_Item)
        {
            items.Remove(i_Item);
        }

        public void RemoveDroppableItems()
        {
            items.RemoveAll(x => x.droppable);
        }
    }
}

