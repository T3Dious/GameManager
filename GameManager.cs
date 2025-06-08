using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerStats playerStats;

    public static GameManager instance; // Singleton instance of the GameManager

    public delegate void OnDamageTaken(float damage); // Delegate for damage taken event
    public event OnDamageTaken onDamageTaken; // Event to notify when damage is taken

    public delegate void OnHeal(float amount); // Delegate for healing event
    public event OnHeal onHeal; // Event to notify when healing is done


    int nextLevelExperience, skillPoints; // Tracks the current experience points
    float currentStamina, currentHealth; // Tracks the current stamina and health

    void Start()
    {
        if (!instance)
            instance = this; // Assign the singleton instance

    }

    public void DealDamage(float damage) => onDamageTaken?.Invoke(damage); // Notify subscribers about the damage taken
    public void Heal(float amount) => onHeal?.Invoke(-amount); // Notify subscribers about the healing done
}
