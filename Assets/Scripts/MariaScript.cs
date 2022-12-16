using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage Maria behavior
/// </summary>
public class MariaScript : MonoBehaviour, Damagable
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
    /// The current Target
    /// </summary>
    private GameObject target;
    
    /// <summary>
    /// Damagable instance of the target. The target can be the tower or a Mutant
    /// </summary>
    private Damagable targetD;
    
    /// <summary>
    /// The Health points of the Character
    /// </summary>
    private float life = 100;
    
    /// <summary>
    /// The max Health points of the Character
    /// </summary>
    private float maxLife = 100;
    
    /// <summary>
    /// The health bar
    /// </summary>
    public Slider Slider;
    
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
        transform.LookAt(GameManager.target.transform);
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _state = State.RUN;
        findNewTarget();
        _animator.ResetTrigger("dying");
        _animator.SetTrigger("run");
        _animator.ResetTrigger("attack");
    }
    
    void Update()
    {
        if (target != null)
        {
            if (_state != State.ATTACK)
            {
                timepassed = 0;
            }

            switch (_state)
            {
                case State.DYING:

                    break;
                case State.RUN:

                    var step = speed * Time.deltaTime;

                    transform.LookAt(target.transform.position);
                    Vector3 move = new Vector3(target.transform.position.x, transform.position.y,
                        target.transform.position.z);

                    transform.position = Vector3.MoveTowards(transform.position, move, step);
                    break;

                case State.ATTACK:

                    if (target != null)
                    {
                        if (targetD.isAlive())
                            transform.LookAt(target.transform.position);
                        timepassed += Time.deltaTime;
                        if (timepassed > _animator.GetCurrentAnimatorStateInfo(0).length)
                        {
                            timepassed = 0;
                            targetD.takeDamage(10);
                            if (!targetD.isAlive())
                            {
                                target = null;
                                findNewTarget();
                                _animator.ResetTrigger("dying");
                                _animator.SetTrigger("run");
                                _animator.ResetTrigger("attack");
                                _state = State.RUN;
                            }
                        }
                    }
                    else
                    {
                        findNewTarget();
                    }


                    break;
            }
        }
        else
        {
            findNewTarget();
        }
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

            _animator.ResetTrigger("dying");
            _animator.ResetTrigger("run");

            _state = State.ATTACK;
        }
    }
    
    private void UpdateHealthBar()
    {
        Slider.value = life / maxLife;
    }
    
    private int calls = 0;
    
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
                _animator.ResetTrigger("attack");
                _state = State.DYING;
                GameManager.mariaList.Remove(gameObject);
                Destroy(gameObject, 2f);
            }
        }
    }

    /// <inheritdoc />
    public bool isAlive()
    {
        return life > 0;
    }
    
    public void findNewTarget()
    {
        target = null;
        foreach (var m in GameManager.MutantList)
        {
            if (m.GetComponent<MutantBehavior>().isAlive())
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

        if (target == null || Vector3.Distance(transform.position, GameManager.target.transform.position) <
            Vector3.Distance(transform.position, target.transform.position))
        {
            target = GameManager.target;
            targetD = GameManager.target.GetComponent<Tower>();
        }
        else
        {
            targetD = target.GetComponent<MutantBehavior>();
        }
    }
    
    /// <summary>
    /// Enum for the different states of the character
    /// </summary>
    private enum State
    {
        ATTACK,
        RUN,
        DYING
    }
}