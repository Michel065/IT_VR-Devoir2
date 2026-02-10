using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrystalScoringSystem : MonoBehaviour
{
    [Header("Références")]
    public CrystalAssemblyManager assemblyManager;
    public CrystalPolishing polishingSystem;
    
    [Header("UI Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI errorsText;
    public TextMeshProUGUI finalScoreText;
    public GameObject finalScorePanel;
    
    [Header("Configuration Score")]
    [Tooltip("Temps parfait pour assemblage (secondes)")]
    public float perfectTime = 30f;
    
    [Tooltip("Temps maximum avant pénalité (secondes)")]
    public float maxTime = 120f;
    
    [Tooltip("Points max pour le temps")]
    public int timePoints = 40;
    
    [Tooltip("Points max pour précision (pas d'erreurs)")]
    public int precisionPoints = 30;
    
    [Tooltip("Points max pour vitesse de placement")]
    public int speedPoints = 20;
    
    [Tooltip("Points max pour polissage")]
    public int polishPoints = 10;
    
    // Variables de tracking
    private float startTime;
    private float assemblyEndTime;
    private int totalErrors = 0;
    private int fragmentsPlaced = 0;
    private float totalPlacementDistance = 0f;
    private bool isAssemblyComplete = false;
    private bool isScoringActive = false;
    
    // Score
    private int finalScore = 0;
    
    void Start()
    {
        // Cacher le panneau final au départ
        if (finalScorePanel != null)
        {
            finalScorePanel.SetActive(false);
        }
        
        StartScoring();
    }
    
    void Update()
    {
        if (isScoringActive && !isAssemblyComplete)
        {
            UpdateTimer();
        }
    }
    
    public void StartScoring()
    {
        startTime = Time.time;
        isScoringActive = true;
        totalErrors = 0;
        fragmentsPlaced = 0;
        
        Debug.Log("🎮 Scoring démarré !");
        
        // Initialiser l'affichage
        UpdateScoreDisplay();
    }
    
    void UpdateTimer()
    {
        float elapsed = Time.time - startTime;
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);
            timerText.text = $"⏱️ Temps: {minutes:00}:{seconds:00}";
        }
    }
    
    /// <summary>
    /// Appelé quand un fragment est placé avec succès
    /// </summary>
    public void OnFragmentPlaced(float distanceFromTarget)
    {
        fragmentsPlaced++;
        totalPlacementDistance += distanceFromTarget;
        
        Debug.Log($"✅ Fragment {fragmentsPlaced}/5 placé. Distance: {distanceFromTarget:F3}m");
        
        UpdateScoreDisplay();
    }
    
    /// <summary>
    /// Appelé quand une erreur est commise (mauvais ordre)
    /// </summary>
    public void OnError()
    {
        totalErrors++;
        
        Debug.Log($"❌ Erreur #{totalErrors}");
        
        if (errorsText != null)
        {
            errorsText.text = $"❌ Erreurs: {totalErrors}";
            
            // Flash rouge
            StartCoroutine(FlashErrorText());
        }
    }
    
    /// <summary>
    /// Appelé quand l'assemblage est terminé
    /// </summary>
    public void OnAssemblyComplete()
    {
        isAssemblyComplete = true;
        assemblyEndTime = Time.time;
        
        float totalTime = assemblyEndTime - startTime;
        
        Debug.Log($"🎉 Assemblage terminé en {totalTime:F1}s avec {totalErrors} erreur(s)");
    }
    
    /// <summary>
    /// Appelé quand le polissage est terminé
    /// </summary>
    public void OnPolishingComplete(float polishQuality)
    {
        isScoringActive = false;
        CalculateFinalScore(polishQuality);
        ShowFinalScore();
    }
    
    void CalculateFinalScore(float polishQuality)
    {
        finalScore = 0;
        
        // === 1. SCORE DE TEMPS (40 points max) ===
        float totalTime = assemblyEndTime - startTime;
        int timeScore = CalculateTimeScore(totalTime);
        finalScore += timeScore;
        
        // === 2. SCORE DE PRÉCISION (30 points max) ===
        int errorPenalty = totalErrors * 5; // -5 points par erreur
        int precisionScore = Mathf.Max(0, precisionPoints - errorPenalty);
        finalScore += precisionScore;
        
        // === 3. SCORE DE VITESSE DE PLACEMENT (20 points max) ===
        float avgDistance = fragmentsPlaced > 0 ? totalPlacementDistance / fragmentsPlaced : 1f;
        int placementScore = CalculatePlacementScore(avgDistance);
        finalScore += placementScore;
        
        // === 4. SCORE DE POLISSAGE (10 points max) ===
        int polishScore = Mathf.RoundToInt(polishQuality * polishPoints);
        finalScore += polishScore;
        
        // === AFFICHAGE DÉTAILLÉ ===
        Debug.Log("========== SCORE FINAL ==========");
        Debug.Log($"⏱️  Temps: {timeScore}/{timePoints} pts ({totalTime:F1}s)");
        Debug.Log($"🎯 Précision: {precisionScore}/{precisionPoints} pts ({totalErrors} erreurs)");
        Debug.Log($"📏 Placement: {placementScore}/{speedPoints} pts (moy: {avgDistance:F2}m)");
        Debug.Log($"✨ Polissage: {polishScore}/{polishPoints} pts ({polishQuality:P0})");
        Debug.Log($"🏆 TOTAL: {finalScore}/100");
        Debug.Log("================================");
    }
    
    int CalculateTimeScore(float time)
    {
        if (time <= perfectTime)
        {
            return timePoints; // Score parfait
        }
        else if (time >= maxTime)
        {
            return 0; // Trop lent
        }
        else
        {
            // Score linéaire entre perfectTime et maxTime
            float ratio = 1f - ((time - perfectTime) / (maxTime - perfectTime));
            return Mathf.RoundToInt(ratio * timePoints);
        }
    }
    
    int CalculatePlacementScore(float avgDistance)
    {
        // Plus on est proche du snap point, meilleur le score
        // Distance < 0.05m = parfait
        // Distance > 0.15m = 0 points
        
        if (avgDistance <= 0.05f)
        {
            return speedPoints;
        }
        else if (avgDistance >= 0.15f)
        {
            return 0;
        }
        else
        {
            float ratio = 1f - ((avgDistance - 0.05f) / 0.10f);
            return Mathf.RoundToInt(ratio * speedPoints);
        }
    }
    
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"📦 Fragments: {fragmentsPlaced}/5";
        }
        
        if (errorsText != null)
        {
            errorsText.text = $"❌ Erreurs: {totalErrors}";
        }
    }
    
    void ShowFinalScore()
    {
        if (finalScorePanel != null)
        {
            finalScorePanel.SetActive(true);
        }
        
        if (finalScoreText != null)
        {
            string grade = GetGrade(finalScore);
            string message = GetScoreMessage(finalScore);
            
            finalScoreText.text = $"SCORE FINAL\n\n{finalScore}/100\n\n{grade}\n\n{message}";
        }
    }
    
    string GetGrade(int score)
    {
        if (score >= 90) return "🏆 RANG S - PARFAIT !";
        if (score >= 80) return "🥇 RANG A - EXCELLENT !";
        if (score >= 70) return "🥈 RANG B - TRÈS BIEN !";
        if (score >= 60) return "🥉 RANG C - BIEN";
        if (score >= 50) return "⭐ RANG D - PEUT MIEUX FAIRE";
        return "📝 RANG E - RÉESSAYE";
    }
    
    string GetScoreMessage(int score)
    {
        if (score >= 90) return "Maîtrise parfaite de l'assemblage VR !";
        if (score >= 80) return "Excellente précision et rapidité !";
        if (score >= 70) return "Bon travail, quelques améliorations possibles.";
        if (score >= 60) return "C'est correct, mais prends ton temps.";
        if (score >= 50) return "Entraîne-toi pour améliorer ta précision.";
        return "N'abandonne pas, réessaye !";
    }
    
    System.Collections.IEnumerator FlashErrorText()
    {
        if (errorsText != null)
        {
            Color original = errorsText.color;
            errorsText.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            errorsText.color = original;
        }
    }
}