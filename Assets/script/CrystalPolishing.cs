using UnityEngine;

public class CrystalPolishing : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Contrôleur de la main droite (XR Controller)")]
    public Transform handController;

    [Tooltip("Distance max pour détecter la main")]
    public float detectionRadius = 0.15f; // 15 cm

    [Tooltip("Vitesse angulaire minimale (degrés/sec)")]
    public float minAngularSpeed = 90f;

    [Tooltip("Durée de polissage requise (secondes)")]
    public float polishingDuration = 3f;

    [Header("Feedback")]
    public ParticleSystem polishingParticles;
    public AudioSource polishingSound;
    public Material polishedMaterial;

    private Vector3 lastHandPosition;
    private float polishingTimer = 0f;
    private bool isPolishing = false;
    private bool isComplete = false;
    private Renderer crystalRenderer;

    void Start()
    {
        crystalRenderer = GetComponent<Renderer>();

        if (handController == null)
        {
            Debug.LogError("[CrystalPolishing] HandController non assigné !");
            enabled = false;
            return;
        }

        lastHandPosition = handController.position;
    }

    void Update()
    {
        if (isComplete) return;
        if (Time.deltaTime <= 0f) return;

        Vector3 currentHandPos = handController.position;

        // 🔒 Protection NaN / Infinity (XR tracking perdu)
        if (IsInvalidVector(currentHandPos))
        {
            StopPolishing();
            lastHandPosition = currentHandPos;
            return;
        }

        float distance = Vector3.Distance(currentHandPos, transform.position);

        if (distance > detectionRadius)
        {
            StopPolishing();
            lastHandPosition = currentHandPos;
            return;
        }

        Vector3 delta = currentHandPos - lastHandPosition;

        // Si delta invalide, on coupe tout
        if (IsInvalidVector(delta))
        {
            StopPolishing();
            lastHandPosition = currentHandPos;
            return;
        }

        // Approximation de vitesse angulaire
        float angularSpeed = (delta.magnitude / Time.deltaTime) * 50f;

        if (float.IsNaN(angularSpeed) || float.IsInfinity(angularSpeed))
        {
            StopPolishing();
            lastHandPosition = currentHandPos;
            return;
        }

        if (angularSpeed >= minAngularSpeed)
        {
            if (!isPolishing)
            {
                StartPolishing();
            }

            polishingTimer += Time.deltaTime;

            if (polishingTimer >= polishingDuration)
            {
                CompletePolishing();
            }
        }
        else
        {
            StopPolishing();
        }

        lastHandPosition = currentHandPos;
    }

    void StartPolishing()
    {
        isPolishing = true;

        if (polishingParticles != null && !polishingParticles.isPlaying)
        {
            polishingParticles.Play();
        }

        if (polishingSound != null && !polishingSound.isPlaying)
        {
            polishingSound.Play();
        }

        Debug.Log("Polissage démarré");
    }

    void StopPolishing()
    {
        if (!isPolishing) return;

        isPolishing = false;
        polishingTimer = 0f;

        if (polishingParticles != null)
        {
            polishingParticles.Stop();
        }

        if (polishingSound != null)
        {
            polishingSound.Stop();
        }
    }

    void CompletePolishing()
    {
        isComplete = true;
        isPolishing = false;

        if (polishedMaterial != null && crystalRenderer != null)
        {
            crystalRenderer.material = polishedMaterial;
        }

        if (polishingParticles != null)
        {
            polishingParticles.Stop();
        }

        if (polishingSound != null)
        {
            polishingSound.Stop();
        }

        Debug.Log("Cristal poli avec succès !");
    }

    bool IsInvalidVector(Vector3 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) ||
               float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
