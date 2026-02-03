using UnityEngine;


public class CrystalAssemblyManager : MonoBehaviour
{
    [Header("Configuration")]
    public int totalFragments = 5;
    
    public int currentExpectedOrder = 0;
    private int placedFragments = 0;
    
    [Header("Événements")]
    public UnityEngine.Events.UnityEvent onCrystalComplete;
    
    private CrystalScoringSystem scoringSystem; // AJOUTE
    
    void Start()
    {
        scoringSystem = FindObjectOfType<CrystalScoringSystem>(); // AJOUTE
        Debug.Log($"🎮 CrystalAssemblyManager démarré. Prochain: {currentExpectedOrder}");
    }
    
    public bool CanPlaceFragment(int fragmentOrder)
    {
        bool result = fragmentOrder == currentExpectedOrder;
        Debug.Log($"🔍 CanPlaceFragment({fragmentOrder}) ? → {result}");
        return result;
    }
    
    public void RegisterFragment(int fragmentOrder)
    {
        placedFragments++;
        currentExpectedOrder++;
        
        Debug.Log($"✅ Fragment {fragmentOrder} enregistré ! {placedFragments}/{totalFragments}");
        
        if (placedFragments >= totalFragments)
        {
            OnCrystalCompleted();
        }
    }
    
    void OnCrystalCompleted()
    {
        Debug.Log("🎉🎉🎉 CRISTAL ASSEMBLÉ !");
        
        // ⭐ NOTIFIER LE SYSTÈME DE SCORE ⭐
        if (scoringSystem != null)
        {
            scoringSystem.OnAssemblyComplete();
        }
        
        onCrystalComplete?.Invoke();
    }
}