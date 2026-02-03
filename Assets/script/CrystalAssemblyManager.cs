using UnityEngine;

public class CrystalAssemblyManager : MonoBehaviour
{
    [Header("Configuration")]
    public int totalFragments = 5;
    
    public int currentExpectedOrder = 0; // CHANGÉ EN PUBLIC pour debug
    private int placedFragments = 0;
    
    [Header("Événements")]
    public UnityEngine.Events.UnityEvent onCrystalComplete;
    
    void Start()
    {
        Debug.Log($"🎮 CrystalAssemblyManager démarré. Prochain fragment attendu : {currentExpectedOrder}");
    }
    
    public bool CanPlaceFragment(int fragmentOrder)
    {
        bool result = fragmentOrder == currentExpectedOrder;
        Debug.Log($"🔍 CanPlaceFragment({fragmentOrder}) ? Attendu={currentExpectedOrder} → {result}");
        return result;
    }
    
    public void RegisterFragment(int fragmentOrder)
    {
        placedFragments++;
        currentExpectedOrder++;
        
        Debug.Log($"✅ Fragment {fragmentOrder} enregistré ! Progression : {placedFragments}/{totalFragments}");
        Debug.Log($"➡️ Prochain fragment attendu : {currentExpectedOrder}");
        
        if (placedFragments >= totalFragments)
        {
            OnCrystalCompleted();
        }
    }
    
    void OnCrystalCompleted()
    {
        Debug.Log("🎉🎉🎉 CRISTAL ASSEMBLÉ AVEC SUCCÈS !");
        onCrystalComplete?.Invoke();
    }
}