using System;
using UnityEngine;
using UnityEngine.VFX;

public enum BonusType
{
    Simple,
    Coin,
    Diamand
}

public class Bonus : MonoBehaviour
{
    public string name = "Бонус жизней";

    [SerializeField] private ParticleSystem collectVFX;
    [SerializeField] private AudioClip collectAudioClip;
    public BonusType bonusType;
    private AudioSource _audio;
    [SerializeField] private Renderer _renderer;

    private void Start()
    {
        _audio = gameObject.AddComponent<AudioSource>();
        //_renderer  = GetComponentInChildren<Renderer>();
    }

    internal void Collect()
    {

        Debug.Log($"Bonus Collect {this.bonusType}");
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
