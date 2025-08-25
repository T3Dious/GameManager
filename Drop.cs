using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField]
    int itemsToDrop = 6; // Set your desired max here or expose as a public variable
    // Example drop chances (adjust as needed)
    private static readonly Dictionary<Items.Rarity, float> rarityDropChances = new Dictionary<Items.Rarity, float>
        {
            { Items.Rarity.Common, 0.6f },      // 60%
            { Items.Rarity.Uncommon, 0.2f },    // 20%
            { Items.Rarity.Rare, 0.12f },       // 12%
            { Items.Rarity.Epic, 0.06f },       // 6%
            { Items.Rarity.Legendary, 0.02f }   // 2%
        };

    public List<Items> possibleDrops; // Assign in Inspector


    protected internal List<Items> GetRandomDropByRarity(int maxItemsToDrop)
    {
        List<Items> drops = new List<Items>();
        if (possibleDrops == null || possibleDrops.Count == 0 || maxItemsToDrop <= 0)
            return drops;

        // Group items by rarity
        var grouped = new Dictionary<Items.Rarity, List<Items>>();
        foreach (var item in possibleDrops)
        {
            if (!grouped.ContainsKey(item.rarity))
                grouped[item.rarity] = new List<Items>();
            grouped[item.rarity].Add(item);
        }

        int attempts = 0;
        int maxAttempts = maxItemsToDrop * 5; // Prevent infinite loop

        while (drops.Count < maxItemsToDrop && attempts < maxAttempts)
        {
            attempts++;

            // Randomly pick a rarity based on drop chances
            float rand = Random.value;
            float cumulative = 0f;
            Items.Rarity? selectedRarity = null;
            foreach (var kvp in rarityDropChances.OrderBy(r => (int)r.Key)) // Common first
            {
                cumulative += kvp.Value;
                if (rand <= cumulative)
                {
                    selectedRarity = kvp.Key;
                    break;
                }
            }

            // If we have items of that rarity, pick one
            if (selectedRarity.HasValue && grouped.ContainsKey(selectedRarity.Value) && grouped[selectedRarity.Value].Count > 0)
            {
                var itemsOfRarity = grouped[selectedRarity.Value];
                Items selected = itemsOfRarity[Random.Range(0, itemsOfRarity.Count)];
                if (!drops.Contains(selected))
                    drops.Add(selected);
            }
            // If no item of that rarity, skip this attempt
        }
        return drops;
    }

    [System.Obsolete]
    protected internal void DropItem()
    {
        InventoryBag bag = FindObjectOfType<InventoryBag>();
        int maxItemsToDrop = itemsToDrop; // Set your desired max here or expose as a public variable

        // Calculate total drop chance for all possible drops
        var totalChance = possibleDrops.Sum(item => rarityDropChances[item.rarity]);

        // Calculate how many drops each item should get based on its rarity drop chance
        var itemDropCounts = new Dictionary<Items, int>();
        foreach (var item in possibleDrops)
        {
            float share = rarityDropChances[item.rarity] / totalChance;
            int count = Mathf.FloorToInt(share * maxItemsToDrop);
            itemDropCounts[item] = count;
        }

        // Distribute remaining drops (due to rounding) to items with highest drop chance
        int distributed = itemDropCounts.Values.Sum();
        int remaining = maxItemsToDrop - distributed;
        if (remaining > 0)
        {
            var sortedItems = possibleDrops.OrderByDescending(i => rarityDropChances[i.rarity]).ToList();
            for (int i = 0; i < remaining; i++)
            {
                itemDropCounts[sortedItems[i % sortedItems.Count]]++;
            }
        }

        foreach (var kvp in itemDropCounts)
        {
            var dropItem = kvp.Key;
            int dropAmount = kvp.Value;
            if (dropAmount <= 0) continue;

            if (dropItem.icon != null)
            {
                for (int i = 0; i < dropAmount; i++)
                {
                    _ = Instantiate(dropItem.icon, new Vector3(
                        Random.Range(transform.position.x, transform.position.x + 0.6f),
                        Random.Range(transform.position.x, transform.position.x + 0.6f), 0), Quaternion.identity);
                }
            }
            else
            {
                Debug.LogWarning($"DropItem: icon not assigned for {dropItem.itemName}");
            }
            bool added = bag.AddItem(dropItem, dropAmount);
            // if (added)
            //     Debug.Log($"Dropped {dropAmount}x {dropItem.itemName} (Rarity: {dropItem.rarity}) and added to bag.");
            // else
            //     Debug.Log("Bag is full, could not add dropped item.");
        }
    }
}
