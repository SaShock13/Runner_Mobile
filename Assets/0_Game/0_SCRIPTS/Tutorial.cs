using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class Tutorial : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float upDistance = 100f; // Дистанция движения вверх в пикселях
    [SerializeField] private float upDuration = 1f;   // Длительность движения вверх
    [SerializeField] private float downDuration = 0.3f; // Длительность движения вниз
    [SerializeField] private Ease upEase = Ease.OutQuad; // Стиль движения вверх
    [SerializeField] private Ease downEase = Ease.InQuad; // Стиль движения вниз
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TMP_Text tutText;
    private Vector2 _originalPosition;

    [Inject] private EventBus _bus;
    private IInput _input;
    Sequence bounceSequence;
    private bool isWaitForSwipeUp = false;
    private bool isWaitForSwipeDown = false;
    private bool isWaitForSwipeLeft = false;
    private bool isWaitForSwipeRight = false;
    private bool tutorialIsPlaying = false;

    public void SetInput(IInput input)
    {
        _input = input;
        _input.OnSwipeUp += OnSwipeUp;
        _input.OnSwipeDown += OnSwipeDown;
        _input.OnSwipeLeft += OnSwipeLeft;
        _input.OnSwipeRight += OnSwipeRight;
    }

    private void Start()
    {
        _originalPosition = _rectTransform.anchoredPosition;
        panel.SetActive(false);
    }

    private void OnSwipeRight()
    {
        if (isWaitForSwipeRight) StopTutorial();
    }

    private void OnSwipeLeft()
    {
        if (isWaitForSwipeLeft) StopTutorial();
    }

    private void OnSwipeDown()
    {
        if (isWaitForSwipeDown) StopTutorial();
    }

    private void OnSwipeUp()
    {
        if (isWaitForSwipeUp) StopTutorial();
    }

    public void StartTutorial()
    {
        StartCoroutine(TutorialCoroutine());        
    }


    IEnumerator TutorialCoroutine()
    {
        StartUpAnimation();
        while (tutorialIsPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        StartDownAnimation();
        while (tutorialIsPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        StartLeftAnimation();
        while (tutorialIsPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        _bus.PublishOnTutorialFinishedEvent();
        StartRightAnimation();
        while (tutorialIsPlaying)
        {
            yield return null;
        }
    }

    private void StopTutorial()
    {
        tutorialIsPlaying = false;
        panel.SetActive(false);
        KillSequence();        
    }

    public void StartUpAnimation()
    {
        tutorialIsPlaying = true;
        panel.SetActive(true);
        tutText.text = "Свайп вверх для прыжка"; 
        bounceSequence = DOTween.Sequence();
        bounceSequence.Append(
            _rectTransform.DOAnchorPosY(_originalPosition.y + upDistance, upDuration)
                .SetEase(upEase)
        );
        bounceSequence.Append(
            _rectTransform.DOAnchorPosY(_originalPosition.y, downDuration)
                .SetEase(downEase)
        );
        bounceSequence.SetLoops(-1, LoopType.Restart);
        bounceSequence.SetId("BounceAnimation");
        isWaitForSwipeUp = true;
    }
    public void StartDownAnimation()
    {
        tutorialIsPlaying = true;
        panel.SetActive(true);
        tutText.text = "Свайп вниз для слайда";
        bounceSequence = DOTween.Sequence();
        bounceSequence.Append(
            _rectTransform.DOAnchorPosY(_originalPosition.y - upDistance, upDuration)
                .SetEase(upEase)
        );
        bounceSequence.Append(
            _rectTransform.DOAnchorPosY(_originalPosition.y, downDuration)
                .SetEase(downEase)
        );
        bounceSequence.SetLoops(-1, LoopType.Restart);
        bounceSequence.SetId("BounceAnimation");
        isWaitForSwipeDown = true;
    }
    
    public void StartLeftAnimation()
    {
        tutorialIsPlaying = true;
        panel.SetActive(true);
        tutText.text = "Свайп влево для предвижения влево";
        bounceSequence = DOTween.Sequence();
        bounceSequence.Append(
            _rectTransform.DOAnchorPosX(_originalPosition.x - upDistance, upDuration)
                .SetEase(upEase)
        );
        bounceSequence.Append(
            _rectTransform.DOAnchorPosX(_originalPosition.x, downDuration)
                .SetEase(downEase)
        );
        bounceSequence.SetLoops(-1, LoopType.Restart);
        bounceSequence.SetId("BounceAnimation");
        isWaitForSwipeLeft = true;
    }
    
    public void StartRightAnimation()
    {
        tutorialIsPlaying = true;
        panel.SetActive(true);
        tutText.text = "Свайп вправо для предвижения вправо";
        bounceSequence = DOTween.Sequence();
        bounceSequence.Append(
            _rectTransform.DOAnchorPosX(_originalPosition.x + upDistance, upDuration)
                .SetEase(upEase)
        );
        bounceSequence.Append(
            _rectTransform.DOAnchorPosX(_originalPosition.x, downDuration)
                .SetEase(downEase)
        );
        bounceSequence.SetLoops(-1, LoopType.Restart);
        bounceSequence.SetId("BounceAnimation");
        isWaitForSwipeRight = true;
    }

    private void OnDestroy()
    {
        KillSequence();
    }

    private static void KillSequence()
    {
        DOTween.Kill("BounceAnimation");
    }
}
