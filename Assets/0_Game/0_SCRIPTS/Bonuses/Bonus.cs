using UnityEngine;

public enum BonusType
{
    Simple,
    Coin,
    Diamand,
    SpeedBoost,
    Invincibility,
    MultiplyerX2
}

public class Bonus : MonoBehaviour
{
    public string name = "Бонус жизней";

    [SerializeField] private ParticleSystem collectVFX;
    [SerializeField] private AudioClip collectAudioClip;
    public BonusType bonusType;
    private AudioSource _audio;
    [SerializeField] private Renderer _renderer;
    [SerializeField] public float duration;

    private void Start()
    {
        _audio = gameObject.AddComponent<AudioSource>();
        //_renderer  = GetComponentInChildren<Renderer>();
    }

    internal void Collect()
    {

        DebugUtils.LogEditor($"Bonus Collect {this.bonusType}");
        if(collectAudioClip !=null)
        {
            GetComponent<AudioSource>().clip = collectAudioClip;  
            GetComponent<AudioSource>().Play();
        }
        if (collectVFX != null)
        {
            collectVFX.Play();
        }
        _renderer.enabled = false;
        Destroy(transform.parent.gameObject, 1);
    }
}
