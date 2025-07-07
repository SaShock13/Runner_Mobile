using System.Collections;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour
{    
    private float startColliderHeight ;
    private Vector3 startColliderCenter;
    private CapsuleCollider _capsuleCollider ;
    public bool isAutoRun = true;
    private float speedIncreseSpeed = 1f;
    private float starafeIncreseSpeed = 0.0002f;
    private bool isCanRun = true;
    private bool isCanStrafe = true;
    private bool isCanMove   = true;
    private int cellsCount = 5;
    private int currentCell = 3;
    private float cellWidth = 1;
    private IInput _input;
    private float strafeHandicap = 0.2f;
    private bool isBuffered = false;
    private delegate void BufferedDelegate();
    private BufferedDelegate bufferedMove;
    private PlayerAnimatorManager _animatorManager;
    private SoundManager _soundManager;
    private PlayerStats _playerStats;
    private EventBus _eventBus;
    private float increaseSpeedTimeInterval = 15;

    [Inject]
    public void Construct(PlayerAnimatorManager animatorManager, SoundManager soundManager, PlayerStats playerStats, EventBus eventBus)
    {
        _playerStats = playerStats;
        _animatorManager = animatorManager;
        _soundManager = soundManager;
        _eventBus = eventBus;
    }


    private void Start()
    {
        GameObject plane = GameObject.Find("Floor");
        _capsuleCollider = GetComponent<CapsuleCollider>();
        startColliderCenter = _capsuleCollider.center;
        startColliderHeight = _capsuleCollider.height;
        StartCoroutine(IncreaseSpeedCoroutine(_playerStats.maxRunSpeed));
        float filedWidth = plane.GetComponent<Renderer>().bounds.size.x;
        cellWidth = filedWidth / (float)cellsCount;
        _input.OnSwipeLeft += StrafeLeft;
        _input.OnSwipeRight += StrafeRight;
        _input.OnSwipeUp += Jump;
        _input.OnSwipeDown += Slide;

    }

    public void SetInput(IInput input)
    {
        _input = input;
    }

    private void Jump()
    {

        if (_animatorManager != null && isCanMove)
        {
            bufferedMove = null;
            _animatorManager.PlayJump();
            _soundManager.PlaySFX(Sounds.jump);
            StartCoroutine(JumpCoroutine()); 
        }
        else bufferedMove = Jump;
    }
    private void Slide()
    {
        if (_animatorManager != null && isCanMove)
        {
            bufferedMove = null;
            _animatorManager.PlaySlide();
            _soundManager.PlaySFX(Sounds.slide);
            StartCoroutine(SlideCoroutine());
        }
        else bufferedMove = Slide;
    }



    IEnumerator JumpCoroutine()
    {
        isCanMove = false;
        _capsuleCollider.height = 0.5f;
        var newCenter = _capsuleCollider.center;
        newCenter.y = 1.25f;
        _capsuleCollider.center = newCenter;
        yield return new WaitForSeconds(0.7f);
        _capsuleCollider.height = startColliderHeight;
        _capsuleCollider.center = startColliderCenter;
        isCanMove = true;
        if (bufferedMove != null) bufferedMove();
    }


    IEnumerator SlideCoroutine()
    {
        isCanMove = false;
        _capsuleCollider.height = 0.5f;
        var newCenter = _capsuleCollider.center;
        newCenter.y = 0.25f;
        _capsuleCollider.center = newCenter;
        yield return new WaitForSeconds(1f);
        _capsuleCollider.height = startColliderHeight;
        _capsuleCollider.center = startColliderCenter;
        isCanMove = true;
        if (bufferedMove != null) bufferedMove();
    }

    public void Stop()
    {
        _playerStats.currentRunSpeed = 0.0f;
        isCanRun = false;
        StopAllCoroutines();
    }

    private IEnumerator IncreaseSpeedCoroutine(float targetSpeed)
    {
        while (_playerStats.currentRunSpeed < targetSpeed)
        {
            yield return new WaitForSeconds(increaseSpeedTimeInterval);
            _playerStats.currentRunSpeed += speedIncreseSpeed;
            _playerStats.strafeTime -= Time.deltaTime * starafeIncreseSpeed;
            _eventBus.PublishOnSpeedChangedEvent(_playerStats.currentRunSpeed);
            _eventBus.PublishOnGameDifficultyIncreasedEvent();
            if (_playerStats.currentRunSpeed > 10 && _playerStats.currentRunSpeed < 11) {  }  
        }
    }

    public void MoveForward(Vector3 direction)
    {
        if (isCanRun)
        {
            transform.position += new Vector3(0, 0, direction.z) * _playerStats.currentRunSpeed * Time.deltaTime;
        }
    }      

    public void StrafeRight()
    {
        if (isCanStrafe )
        {
            bufferedMove = null;
            var newPosition = transform.position;
            if (currentCell < cellsCount)
            {
                newPosition.x += cellWidth;
                currentCell++;
            }
            isCanStrafe = false;
            transform.DOMoveX(newPosition.x, _playerStats.strafeTime).OnComplete(() => { isCanStrafe = true; if (bufferedMove != null) bufferedMove(); });
        }
        else 
        {
            Debug.Log($"buffered = StrafeRight; {this}");
            bufferedMove = StrafeRight;
        }
    }

    public void StrafeLeft()
    {
        if (isCanStrafe )
        {
            bufferedMove = null;
            var newPosition = transform.position;
            if (currentCell > 1)
            {
                newPosition.x -= cellWidth;
                currentCell--;
            }
            isCanStrafe = false;
            transform.DOMoveX(newPosition.x, _playerStats.strafeTime).OnComplete(()=> { isCanStrafe = true; if(bufferedMove != null) bufferedMove(); });
        }
        else 
        {

            Debug.Log($"buffered = StrafeLeft {this}");
            bufferedMove = StrafeLeft;
        }
    }

    private void Update()
    {
        if (isAutoRun)
        {
            MoveForward(new Vector3(0, 0, 1));
        }
    }
}
