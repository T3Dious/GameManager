using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager instance; // Singleton instance of the ExperienceManager
    public delegate void OnExperienceChanged(int newExperience);
    public event OnExperienceChanged onExperienceChanged; // Event to notify when experience changes

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    public void AddExperience(int amount) => onExperienceChanged?.Invoke(amount); // Notify subscribers about the experience change
    // // stats.TotalExperience += amount;
    // // Debug.Log($"Experience Added: {amount}. Total Experience: {stats.TotalExperience}");
}
