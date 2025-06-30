using System;
using DG.Tweening;
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
    private Vector2 _originalPosition;

    [Inject] private EventBus _bus;
    private IInput _input;
    Sequence bounceSequence;

    private bool isWaitForSwipeUp = false;
    private bool isWaitForSwipeDown = false;
    private bool isWaitForSwipeLeft = false;
    private bool isWaitForSwipeRight = false;

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
        _bus.OnGameStartEvent += StartTutorial;
        
        
        //// Получаем компоненты
        //_rectTransform = GetComponent<RectTransform>();

        // Сохраняем исходную позицию
        _originalPosition = _rectTransform.anchoredPosition;

        panel.SetActive(false);

        // Запускаем анимацию
        //StartBounceAnimation();
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

    private void StartTutorial()
    {
        panel.SetActive(true);  
        StartUpAnimation();
    }

    private void StopTutorial()
    {
        panel.SetActive(false);
        KillSequence();
        _bus.PublishOnTutorialFinishedEvent();
    }

    public void StartUpAnimation()
    {
        // Создаем последовательность
        bounceSequence = DOTween.Sequence();

        // 1. Движение вверх
        bounceSequence.Append(
            _rectTransform.DOAnchorPosY(_originalPosition.y + upDistance, upDuration)
                .SetEase(upEase)
        );

        // 2. Быстрое движение вниз (возврат в исходную позицию)
        bounceSequence.Append(
            _rectTransform.DOAnchorPosY(_originalPosition.y, downDuration)
                .SetEase(downEase)
        );

        // 3. Зацикливаем анимацию
        bounceSequence.SetLoops(-1, LoopType.Restart);

        // Опционально: имя для отладки
        bounceSequence.SetId("BounceAnimation");
        isWaitForSwipeUp = true;
    }

    private void OnDestroy()
    {
        KillSequence();
    }

    private static void KillSequence()
    {
        // Останавливаем анимацию при уничтожении объекта
        DOTween.Kill("BounceAnimation");
    }
}
