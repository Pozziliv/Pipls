using UnityEngine;

public class InstaParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    public void PlayParticles()
    {
        _particleSystem.Play();
    }
}
