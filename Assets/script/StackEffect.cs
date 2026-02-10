using UnityEngine;

public class StackEffect : MonoBehaviour
{
    public void PlayStackEffect()
    {
        // Particules qui montent
        ParticleSystem ps = gameObject.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = Color.green;
        main.startSize = 0.05f;
        main.startLifetime = 1f;
        main.startSpeed = 0.2f;
        main.maxParticles = 20;
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20)
        });
        
        ps.Play();
        Destroy(ps.gameObject, 2f);
    }
}