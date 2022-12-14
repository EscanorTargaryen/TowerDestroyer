using UnityEngine;

public class MutantBehavior : MonoBehaviour
{
    float speed = 0.1f;
    private bool attacking = false;
    private Animator _animator;

    private float life = 100;
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
                        break;
                    }
                }

                transform.LookAt(target.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                break;
            case State.IDLE:
                if (findNewTarget() != null)
                {
                    _state = State.RUN;
                    _animator.SetTrigger("run");
                    _animator.ResetTrigger("idle");
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(target))
        {
            _animator.SetTrigger("attack");
            _animator.ResetTrigger("idle");
            _animator.ResetTrigger("run");
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

    private void takeDamage(float damage)
    {
        life -= damage;
    }

    private bool isAlive()
    {
        return life > 0;
    }

    public GameObject findNewTarget()
    {
        target = null;
        foreach (var m in GameManager.mariaList)
        {
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

    private enum State
    {
        IDLE,
        ATTACK,
        RUN
    }
}