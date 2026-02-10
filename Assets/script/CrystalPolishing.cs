using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class CrystalPolishing : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Contrôleur de la main droite")]
    public Transform handController;
    
    [Tooltip("Distance max pour détecter la main (mètres)")]
    public float detectionRadius = 0.2f; // 20 cm
    
    [Tooltip("Vitesse minimale pour détecter le mouvement circulaire")]
    public float minMovementSpeed = 0.3f;
    
    [Tooltip("Durée de polissage requise (secondes)")]
    public float polishingDuration = 3f;
    
    [Header("Feedback Visuel")]
    [Tooltip("Système de particules pour le polissage")]
    public ParticleSystem polishingParticles;
    
    [Tooltip("Matériau brillant final après polissage")]
    public Material polishedMaterial;
    
    [Tooltip("Lumière qui s'intensifie pendant le polissage")]
    public Light polishingLight;
    
    [Header("Feedback Sonore")]
    [Tooltip("Son en boucle pendant le polissage")]
    public AudioSource polishingSound;
    
    [Tooltip("Son joué à la fin du polissage")]
    public AudioClip polishCompleteSound;
    
    [Header("Debug")]
    [Tooltip("Afficher les gizmos de la zone de détection")]
    public bool showDebugGizmos = true;
    
    // Variables privées
    private Vector3 lastHandPosition;
    private float polishingTimer = 0f;
    private bool isPolishing = false;
    private bool isComplete = false;
    private Renderer crystalRenderer;
    private Material originalMaterial;
    private XRBaseController controller;
    
    void Start()
    {
        Debug.Log("🟡 CrystalPolishing.Start() appelé");
        
        // Récupérer le renderer du cristal
        crystalRenderer = GetComponentInChildren<Renderer>();
        if (crystalRenderer != null)
        {
            originalMaterial = crystalRenderer.material;
            Debug.Log("✅ Renderer du cristal trouvé");
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun Renderer trouvé sur le cristal ou ses enfants");
        }
        
        // Initialiser la position de la main
        if (handController != null)
        {
            lastHandPosition = handController.position;
            controller = handController.GetComponent<XRBaseController>();
            Debug.Log($"✅ Hand Controller assigné : {handController.name}");
        }
        else
        {
            Debug.LogError("❌ Hand Controller NON ASSIGNÉ ! Le polissage ne fonctionnera pas.");
        }
        
        // Désactiver particules au départ
        if (polishingParticles != null)
        {
            polishingParticles.Stop();
        }
        
        // Désactiver lumière au départ
        if (polishingLight != null)
        {
            polishingLight.intensity = 0;
        }
        
        // IMPORTANT : Désactiver ce script au départ
        // Il sera activé automatiquement après l'assemblage
        this.enabled = false;
        
        Debug.Log("✅ CrystalPolishing initialisé (script désactivé en attente)");
    }
    
    void OnEnable()
    {
        Debug.Log("========================================");
        Debug.Log("🟢🟢🟢 CrystalPolishing ACTIVÉ !");
        Debug.Log("========================================");
        Debug.Log("📍 Position du cristal : " + transform.position);
        
        if (handController != null)
        {
            Debug.Log("📍 Position de la main : " + handController.position);
            float initialDistance = Vector3.Distance(handController.position, transform.position);
            Debug.Log($"📏 Distance initiale : {initialDistance:F2}m");
        }
        else
        {
            Debug.LogError("❌ Hand Controller NULL dans OnEnable !");
        }
        
        Debug.Log($"🎯 Zone de détection : {detectionRadius}m");
        Debug.Log($"⚡ Vitesse minimale requise : {minMovementSpeed}");
        Debug.Log($"⏱️  Durée de polissage requise : {polishingDuration}s");
        Debug.Log("========================================");
        
        // Réinitialiser les variables
        polishingTimer = 0f;
        isPolishing = false;
        isComplete = false;
        
        if (handController != null)
        {
            lastHandPosition = handController.position;
        }
    }
    
    void Update()
    {
        // Ne rien faire si le polissage est terminé ou si pas de contrôleur
        if (isComplete || handController == null) 
            return;
        
        // Calculer la distance entre la main et le cristal
        float distance = Vector3.Distance(handController.position, transform.position);
        
        // Debug périodique (toutes les secondes environ)
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"👋 Main à {distance:F2}m du cristal (zone de détection: {detectionRadius}m)");
        }
        
        // Vérifier si la main est dans la zone de détection
        if (distance <= detectionRadius)
        {
            // Main dans la zone !
            Vector3 currentHandPos = handController.position;
            Vector3 movement = currentHandPos - lastHandPosition;
            float speed = movement.magnitude / Time.deltaTime;
            
            // Debug vitesse périodique
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"🔄 Vitesse de mouvement : {speed:F2} (minimum requis: {minMovementSpeed})");
            }
            
            // Vérifier si le mouvement est assez rapide
            if (speed >= minMovementSpeed)
            {
                // Mouvement circulaire détecté !
                if (!isPolishing)
                {
                    StartPolishing();
                }
                
                // Incrémenter le timer
                polishingTimer += Time.deltaTime;
                
                // Mettre à jour les feedbacks progressifs
                UpdatePolishingFeedback(polishingTimer / polishingDuration);
                
                // Debug progression (chaque seconde)
                if (Mathf.FloorToInt(polishingTimer) != Mathf.FloorToInt(polishingTimer - Time.deltaTime))
                {
                    Debug.Log($"⏱️  Polissage : {polishingTimer:F1}s / {polishingDuration}s ({(polishingTimer / polishingDuration * 100):F0}%)");
                }
                
                // Vérifier si le polissage est terminé
                if (polishingTimer >= polishingDuration)
                {
                    CompletePolishing();
                }
            }
            else
            {
                // Mouvement trop lent
                if (isPolishing)
                {
                    Debug.Log("⚠️ Mouvement trop lent, polissage interrompu");
                }
                StopPolishing();
            }
            
            lastHandPosition = currentHandPos;
        }
        else
        {
            // Main trop éloignée
            if (isPolishing)
            {
                Debug.Log("⚠️ Main trop éloignée, polissage interrompu");
            }
            StopPolishing();
        }
    }
    
    void StartPolishing()
    {
        isPolishing = true;
        
        Debug.Log("========================================");
        Debug.Log("🔄 POLISSAGE DÉMARRÉ !");
        Debug.Log("========================================");
        
        // Activer particules
        if (polishingParticles != null && !polishingParticles.isPlaying)
        {
            polishingParticles.Play();
            Debug.Log("✅ Particules activées");
        }
        
        // Activer son en boucle
        if (polishingSound != null && !polishingSound.isPlaying)
        {
            polishingSound.loop = true;
            polishingSound.Play();
            Debug.Log("✅ Son de polissage démarré");
        }
        
        // Vibration légère continue
        if (controller != null)
        {
            controller.SendHapticImpulse(0.2f, 0.1f);
        }
    }
    
    void StopPolishing()
    {
        if (!isPolishing) 
            return;
        
        isPolishing = false;
        polishingTimer = 0f;
        
        // Arrêter particules
        if (polishingParticles != null && polishingParticles.isPlaying)
        {
            polishingParticles.Stop();
        }
        
        // Arrêter son
        if (polishingSound != null && polishingSound.isPlaying)
        {
            polishingSound.Stop();
        }
        
        // Réinitialiser lumière
        if (polishingLight != null)
        {
            polishingLight.intensity = 0;
        }
        
        // Réinitialiser matériau si en cours de polissage
        if (crystalRenderer != null && originalMaterial != null)
        {
            crystalRenderer.material = originalMaterial;
        }
    }
    
    void UpdatePolishingFeedback(float progress)
    {
        // Progression entre 0 et 1
        progress = Mathf.Clamp01(progress);
        
        // Intensifier la lumière progressivement
        if (polishingLight != null)
        {
            polishingLight.intensity = progress * 2f; // Max intensité = 2
        }
        
        // Augmenter la brillance du matériau progressivement
        if (crystalRenderer != null)
        {
            Material currentMat = crystalRenderer.material;
            
            // Interpoler vers le matériau poli
            if (polishedMaterial != null)
            {
                currentMat.Lerp(originalMaterial, polishedMaterial, progress);
            }
            else
            {
                // Si pas de matériau poli, juste augmenter l'émission
                Color emissionColor = Color.Lerp(Color.black, Color.white, progress);
                currentMat.SetColor("_EmissionColor", emissionColor);
                currentMat.EnableKeyword("_EMISSION");
            }
        }
        
        // Vibration progressive
        if (controller != null && Time.frameCount % 10 == 0) // Toutes les 10 frames
        {
            controller.SendHapticImpulse(0.1f + progress * 0.3f, 0.05f);
        }
    }
    
    void CompletePolishing()
    {
        isComplete = true;
        
        Debug.Log("========================================");
        Debug.Log("✨✨✨ CRISTAL POLI AVEC SUCCÈS !");
        Debug.Log("========================================");
        
        // Arrêter le polissage
        if (polishingParticles != null)
        {
            polishingParticles.Stop();
        }
        
        if (polishingSound != null)
        {
            polishingSound.Stop();
        }
        
        // Appliquer le matériau final poli
        if (crystalRenderer != null && polishedMaterial != null)
        {
            crystalRenderer.material = polishedMaterial;
            Debug.Log("✅ Matériau poli appliqué");
        }
        
        // Lumière au maximum
        if (polishingLight != null)
        {
            polishingLight.intensity = 3f;
            Debug.Log("✅ Lumière maximale");
        }
        
        // Son de complétion
        if (polishCompleteSound != null)
        {
            AudioSource.PlayClipAtPoint(polishCompleteSound, transform.position, 1f);
            Debug.Log("✅ Son de complétion joué");
        }
        
        // Vibration de succès (plus forte)
        if (controller != null)
        {
            StartCoroutine(SuccessVibration());
        }
        
        // Calculer la qualité du polissage (toujours 100% si terminé)
        float polishQuality = 1f;
        
        // Notifier le système de scoring
        CrystalScoringSystem scoringSystem = FindObjectOfType<CrystalScoringSystem>();
        if (scoringSystem != null)
        {
            scoringSystem.OnPolishingComplete(polishQuality);
            Debug.Log($"✅ Score notifié (qualité: {polishQuality:P0})");
        }
        
        Debug.Log("========================================");
        
        // Désactiver le script (optionnel)
        this.enabled = false;
    }
    
    IEnumerator SuccessVibration()
    {
        // Vibration en 3 impulsions
        for (int i = 0; i < 3; i++)
        {
            if (controller != null)
            {
                controller.SendHapticImpulse(0.8f, 0.2f);
            }
            yield return new WaitForSeconds(0.15f);
        }
    }
    
    // Afficher la zone de détection en mode Scene
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) 
            return;
        
        // Zone de détection en cyan
        Gizmos.color = new Color(0, 1, 1, 0.3f); // Cyan transparent
        Gizmos.DrawSphere(transform.position, detectionRadius);
        
        // Contour en cyan opaque
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Ligne vers la main si assignée
        if (handController != null && Application.isPlaying)
        {
            float distance = Vector3.Distance(handController.position, transform.position);
            
            // Couleur selon la distance
            if (distance <= detectionRadius)
            {
                Gizmos.color = Color.green; // Main dans la zone
            }
            else
            {
                Gizmos.color = Color.red; // Main hors zone
            }
            
            Gizmos.DrawLine(transform.position, handController.position);
        }
    }
}