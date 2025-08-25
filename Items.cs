using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Items : ScriptableObject
{
    public string itemName; // Name of the item
    public string description; // Description of the item
    public GameObject icon; // Icon representing the item
    public int maxStackSize = 99; // Maximum stack size for the item
    public bool isConsumable; // Indicates if the item is consumable
    public bool isEquippable; // Indicates if the item is equippable
    public bool isQuestItem; // Indicates if the item is a quest item
    public bool isCraftable; // Indicates if the item is craftable

    [Header("Item Type")]
    public ItemType itemType; // Type of the item (e.g., weapon, armor, potion, etc.)

    [Header("Item Attributes")]
    public int damage; // Damage value for weapons
    public int defense; // Defense value for armor
    public float healingAmount; // Healing amount for consumables
    public Rarity rarity; // Add this property

    [Header("Crafting Requirements")]
    public CraftingRequirement[] craftingRequirements; // Requirements for crafting the item

    [System.Serializable]
    public class CraftingRequirement
    {
        public Items requiredItem; // Required item for crafting
        public int requiredAmount; // Amount of required item for crafting
    }

    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        QuestItem,
        CraftingMaterial,
        Miscellaneous
    }
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
