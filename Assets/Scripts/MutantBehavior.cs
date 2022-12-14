using DefaultNamespace;
using UnityEngine;

public class MutantBehavior : MonoBehaviour, Damagable
{
    float speed = 0.1f;
    private bool attacking = false;
    private Animator _animator;

    private float life = 1;
    private GameObject target;
    private State _state;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _state = State.IDLE;
    }

    private float timepassed;

    void Update()
    {
        if (_state != State.ATTACK)
        {
            timepassed = 0;
        }

        switch (_state)
        {
            case State.RUN:


                var step = speed * Time.deltaTime;

                if (target == null)
                {
                    if (findNewTarget() == null)
                    {
                        _animator.SetTrigger("idle");
                        _animator.ResetTrigger("run");
                        _animator.ResetTrigger("attack");
                        _animator.ResetTrigger("dying");
                        _state = State.IDLE;
                        break;
                    }
                }

                transform.LookAt(target.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                break;
            case State.IDLE:

                if (target == null)
                {
                    findNewTarget();
                }

                if (target != null)
                {
                    _state = State.RUN;
                    _animator.SetTrigger("run");
                    _animator.ResetTrigger("idle");
                    _animator.ResetTrigger("attack");
                    _animator.ResetTrigger("dying");
                }


                break;

            case State.ATTACK:

                if (target == null)
                {
                    _animator.SetTrigger("idle");
                    _animator.ResetTrigger("attack");
                    _state = State.IDLE;
                }
                else
                {
                    if (Vector3.Distance(transform.position, target.transform.position) > 10)
                        transform.LookAt(target.transform.position);
                    timepassed += Time.deltaTime;
                    if (timepassed > _animator.GetCurrentAnimatorStateInfo(0).length)
                    {
                        timepassed = 0;
                        target.GetComponent<MariaScript>().takeDamage(10);
                        if (!target.GetComponent<MariaScript>().isAlive())
                        {
                            _animator.SetTrigger("idle");
                            _animator.ResetTrigger("attack");
                            _state = State.IDLE;
                        }
                    }
                }


                break;
            case State.DYING:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(target))
        {
            _animator.SetTrigger("attack");
            _animator.ResetTrigger("idle");
            _animator.ResetTrigger("run");
            _animator.ResetTrigger("dying");
            _state = State.ATTACK;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(target))
        {
            _animator.ResetTrigger("attack");
            _animator.ResetTrigger("idle");
            _animator.SetTrigger("run");
            _state = State.RUN;
        }
    }

    public void takeDamage(float damage)
    {
        _ShowAndroidToastMessage("ss");
        life -= damage;
        if (!isAlive())
        {
            if (calls == 0)
            {
                calls++;
                _animator.SetTrigger("dying");
                _animator.ResetTrigger("run");
                _animator.ResetTrigger("idle");
                _animator.ResetTrigger("attack");
                _state = State.DYING;
                GameManager.MutantList.Remove(gameObject);
                Destroy(gameObject, 2f);
            }
        }
    }

    private int calls = 0;


    public bool isAlive()
    {
        return life > 0;
    }

    public GameObject findNewTarget()
    {
        target = null;
        foreach (var m in GameManager.mariaList)
        {
            if (m.GetComponent<MariaScript>().isAlive())
                if (target == null)
                {
                    target = m;
                }
                else
                {
                    if (Vector3.Distance(transform.position, m.transform.position) <
                        Vector3.Distance(transform.position, target.transform.position))
                    {
                        target = m;
                    }
                }
        }

        return target;
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    private enum State
    {
        IDLE,
        ATTACK,
        RUN,
        DYING
    }
}