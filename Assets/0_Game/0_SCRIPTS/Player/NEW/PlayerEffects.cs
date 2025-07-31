using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] ParticleSystem invincibilityFXPrefab;
    private ParticleSystem invincibilityFX;
    private ParticleSystemRenderer InvincibiityFXRenderer;
    private Transform skinTransform;
    private EventBus _eventBus;
    private float durationOffset = 1f; // время до окнчания эффекта для сигнализирования , что скоро закончится
    private int blinkTimes = 5 ; // количество моргания для сигнлизирования окончания эффекта
    private Coroutine _coroutine;

    [Inject]
	public void Construct(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

    private void Start()
    {
        _eventBus.OnInvincibilityEffectEvent += Invincibility;
    }


    private void Invincibility(float duration)
    {
        if(_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(InvincibilityCoroutine(duration));
    }


    IEnumerator InvincibilityCoroutine(float duration)
    {
        Debug.Log($"invincibilityFX Coroutine {invincibilityFX != null}");
        invincibilityFX.Play();
        yield return new WaitForSeconds(duration - durationOffset);
        if (InvincibiityFXRenderer != null) {
            for (int i = 0; i < blinkTimes; i++)
            {
                invincibilityFX.Stop();
                yield return new WaitForSeconds(durationOffset / blinkTimes);
                invincibilityFX.Play();
            }
        } else yield return new WaitForSeconds(durationOffset);
        invincibilityFX.Stop();


    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Invincibility(5);
        }

    }

    public void Initialize()
    {
        skinTransform = transform;
        if (invincibilityFX != null) Destroy(invincibilityFX.gameObject);
        invincibilityFX = Instantiate(invincibilityFXPrefab, skinTransform);
        InvincibiityFXRenderer = invincibilityFX.GetComponent<ParticleSystemRenderer>();

        Debug.Log($"skinTransform {skinTransform!= null}");
        Debug.Log($"invincibilityFX {invincibilityFX != null}");
        Debug.Log($"InvincibiityFXRenderer {InvincibiityFXRenderer != null}");
    }

    public void Reset()
    {
       StopAllCoroutines();
        invincibilityFX.Stop();
    }
}
