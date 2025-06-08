using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryBag : MonoBehaviour
{
    public int maxBagSize = 20; // Maximum unique item slots
    public List<InventorySlot> slots = new List<InventorySlot>();

    [System.Serializable]
    public class InventorySlot
    {
        public Items item;
        public int count;
    }

    // Add item to bag, stacking if possible
    public bool AddItem(Items item, int amount)
    {
        var matchingSlots = slots
            .Where(s => s.item == item && s.count < s.item.maxStackSize)
            .OrderBy(s => (int)s.item.rarity) // Assuming Rarity is an enum where lower values are more common
            .ToList();
        // Try to stack first
        foreach (var slot in slots)
        {
            if (slot.item == item && slot.count < item.maxStackSize)
            {
                int space = item.maxStackSize - slot.count;
                int toAdd = Mathf.Min(space, amount);
                slot.count += toAdd;
                amount -= toAdd;
                if (amount <= 0) return true;
            }
        }
        // Add new slot if space
        while (amount > 0 && slots.Count < maxBagSize)
        {
            int toAdd = Mathf.Min(item.maxStackSize, amount);
            InventorySlot newSlot = new InventorySlot { item = item, count = toAdd };
            slots.Add(newSlot);
            amount -= toAdd;
        }
        // If still have leftover, bag is full
        return amount == 0;
    }
}
