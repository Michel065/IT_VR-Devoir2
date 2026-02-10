using UnityEngine;
using TMPro;

public class CrystalAssemblyManager : MonoBehaviour
{
    [Header("Configuration")]
    public int totalFragments = 5;
    
    [Header("Référence au système de polissage")]
    public GameObject assembledCrystal;
    public CrystalPolishing polishingScript;
    
    [Header("UI - Écran de statistiques")]
    public GameObject assemblyStatsPanel;
    public TextMeshProUGUI chronoText;
    public TextMeshProUGUI errorsText;
    public TextMeshProUGUI messageText;
    
    [Header("Tracking")]
    public int currentExpectedOrder = 0;
    private int placedFragments = 0;
    private int totalErrors = 0; // ⭐ NOUVEAU
    
    [Header("Événements (optionnel)")]
    public UnityEngine.Events.UnityEvent onCrystalComplete;
    
    private CrystalScoringSystem scoringSystem;
    private float assemblyStartTime;
    private float assemblyEndTime;
    
    void Start()
    {
        scoringSystem = FindObjectOfType<CrystalScoringSystem>();
        assemblyStartTime = Time.time;
        
        // Vérifications
        Debug.Log("=== VÉRIFICATIONS CrystalAssemblyManager ===");
        
        if (assembledCrystal != null)
        {
            assembledCrystal.SetActive(false);
            Debug.Log("✅ assembledCrystal désactivé");
        }
        else
        {
            Debug.LogError("❌ assembledCrystal NON ASSIGNÉ !");
        }
        
        if (polishingScript != null)
        {
            polishingScript.enabled = false;
            Debug.Log("✅ Script de polissage désactivé");
        }
        else
        {
            Debug.LogError("❌ polishingScript NON ASSIGNÉ !");
        }
        
        if (assemblyStatsPanel != null)
        {
            assemblyStatsPanel.SetActive(false);
            Debug.Log("✅ Panneau de stats désactivé");
        }
        else
        {
            Debug.LogError("❌ assemblyStatsPanel NON ASSIGNÉ !");
        }
        
        // Vérifier les textes UI
        if (chronoText == null) Debug.LogError("❌ chronoText NON ASSIGNÉ !");
        if (errorsText == null) Debug.LogError("❌ errorsText NON ASSIGNÉ !");
        
        Debug.Log($"🎮 Manager prêt. Prochain fragment : {currentExpectedOrder}");
        Debug.Log("==========================================");
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
    
    /// <summary>
    /// ⭐ NOUVEAU : Enregistrer une erreur
    /// </summary>
    public void RegisterError()
    {
        totalErrors++;
        Debug.Log($"❌ Erreur enregistrée. Total : {totalErrors}");
    }
    
    void OnCrystalCompleted()
    {
        assemblyEndTime = Time.time;
        float totalTime = assemblyEndTime - assemblyStartTime;
        
        Debug.Log("========================================");
        Debug.Log("🎉🎉🎉 CRISTAL ASSEMBLÉ !");
        Debug.Log($"⏱️ Temps : {totalTime:F1}s");
        Debug.Log($"❌ Erreurs : {totalErrors}");
        Debug.Log("========================================");
        
        // Activer le cristal visuellement
        if (assembledCrystal != null)
        {
            assembledCrystal.SetActive(true);
            Debug.Log("✅ Cristal visible");
        }
        
        // Afficher l'écran de statistiques
        ShowStatsPanel(totalTime, totalErrors);
        
        // Notifier le scoring
        if (scoringSystem != null)
        {
            scoringSystem.OnAssemblyComplete();
        }
        
        onCrystalComplete?.Invoke();
    }
    
    void ShowStatsPanel(float time, int errors)
    {
        if (assemblyStatsPanel != null)
        {
            assemblyStatsPanel.SetActive(true);
            Debug.Log("✅ Panneau de stats affiché");
        }
        
        // Afficher le temps
        if (chronoText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            chronoText.text = $"⏱️ Temps : {minutes:00}:{seconds:00}";
        }
        
        // Afficher les erreurs avec couleur dynamique
        if (errorsText != null)
        {
            errorsText.text = $"❌ Erreurs : {errors}";
            
            // Changer la couleur selon le nombre d'erreurs
            if (errors == 0)
            {
                errorsText.color = Color.green; // Parfait !
            }
            else if (errors <= 2)
            {
                errorsText.color = new Color(1f, 0.65f, 0f); // Orange (acceptable)
            }
            else
            {
                errorsText.color = Color.red; // Rouge (beaucoup d'erreurs)
            }
        }
        
        // Message d'encouragement personnalisé
        if (messageText != null)
        {
            if (errors == 0)
            {
                messageText.text = "✨ Assemblage parfait ! Polis le cristal maintenant.";
                messageText.color = Color.green;
            }
            else if (errors <= 2)
            {
                messageText.text = "👍 Bon travail ! Continue avec le polissage.";
                messageText.color = new Color(1f, 0.9f, 0.4f); // Jaune
            }
            else
            {
                messageText.text = "💪 Pas mal ! Fais mieux au polissage !";
                messageText.color = new Color(1f, 0.65f, 0f); // Orange
            }
        }
    }
    
    /// <summary>
    /// Appelée par le bouton "Continuer"
    /// </summary>
    public void OnContinueButtonPressed()
    {
        Debug.Log("========================================");
        Debug.Log("▶️ BOUTON CONTINUER PRESSÉ");
        Debug.Log("========================================");
        
        // Cacher le panneau
        if (assemblyStatsPanel != null)
        {
            assemblyStatsPanel.SetActive(false);
            Debug.Log("✅ Panneau caché");
        }
        
        // Activer le polissage
        if (polishingScript != null)
        {
            polishingScript.enabled = true;
            Debug.Log("✅✅✅ Script de polissage ACTIVÉ !");
        }
        else
        {
            Debug.LogError("❌ Impossible d'activer le polissage !");
        }
        
        Debug.Log("========================================");
    }
    
    /// <summary>
    /// ⭐ Getter pour les stats (utilisé par le scoring)
    /// </summary>
    public int GetTotalErrors()
    {
        return totalErrors;
    }
    
    public float GetAssemblyTime()
    {
        return assemblyEndTime - assemblyStartTime;
    }
}