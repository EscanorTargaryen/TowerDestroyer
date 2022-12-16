using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage Mutant behavior
/// </summary>
public class MutantBehavior : MonoBehaviour, Damagable
{
    /// <summary>
    /// The speed of the character
    /// </summary>
    float speed = 0.1f;

    /// <summary>
    /// The animator of the GameObject
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// The Health points of the Character
    /// </summary>
    private float life = 120;

    /// <summary>
    /// The max Health points of the Character
    /// </summary>
    private float maxLife = 120;

    /// <summary>
    /// The health bar
    /// </summary>
    public Slider Slider;

    /// <summary>
    /// The current Target
    /// </summary>
    private GameObject target;

    /// <summary>
    /// The current state of the character
    /// </summary>
    private State _state;

    /// <summary>
    /// Controls how much time has passed since the attack animation to inflict damage on the target
    /// </summary>
    private float timepassed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _state = State.RUN;
        _animator.SetTrigger("run");
        _animator.ResetTrigger("idle");
        _animator.ResetTrigger("attack");
        _animator.ResetTrigger("dying");
    }


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

                lookTarget();
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                if (Vector3.Distance(transform.position, target.transform.position) < 0.01)
                {
                    _animator.SetTrigger("attack");
                    _animator.ResetTrigger("idle");
                    _animator.ResetTrigger("run");
                    _animator.ResetTrigger("dying");
                    _state = State.ATTACK;
                }


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
                    findNewTarget();
                    _animator.SetTrigger("idle");
                    _animator.ResetTrigger("attack");
                    _state = State.IDLE;
                }
                else
                {
                    if (target.GetComponent<MariaScript>().isAlive())
                        lookTarget();
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

    /// <summary>
    /// Look the target but not change the y value
    /// </summary>
    private void lookTarget()
    {
        if (target == null)
            return;

        Vector3 targetPostition = new Vector3(target.transform.position.x,
            transform.position.y,
            target.transform.position.z);
        transform.LookAt(targetPostition);
    }

    private void UpdateHealthBar()
    {
        Slider.value = life / maxLife;
    }

    /// <summary>
    /// Handle colliding with the target
    /// </summary>
    /// <param name="other"></param>
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

    /// <summary>
    /// Handle exit colliding with the target
    /// </summary>
    /// <param name="other"></param>
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

    /// <inheritdoc />
    public void takeDamage(float damage)
    {
        life -= damage;
        UpdateHealthBar();
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

    /// <inheritdoc />
    public bool isAlive()
    {
        return life > 0;
    }

    public GameObject findNewTarget()
    {
        target = null;
        var transformPosition = transform.position;
        foreach (var m in GameManager.mariaList)
        {
            if (m.GetComponent<MariaScript>().isAlive())
                if (target == null)
                {
                    target = m;
                }
                else
                {
                    if (Vector3.Distance(transformPosition, m.transform.position) <
                        Vector3.Distance(transformPosition, target.transform.position))
                    {
                        target = m;
                    }
                }
        }

        if (target != null)
        {
            transformPosition.y = target.transform.position.y;
        }

        return target;
    }

    /// <summary>
    /// Enum for the different states of the character
    /// </summary>
    private enum State
    {
        IDLE,
        ATTACK,
        RUN,
        DYING
    }
}