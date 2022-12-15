using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MariaScript : MonoBehaviour, Damagable
{
    float speed = 0.1f;

    private Animator _animator;
    private GameObject target;
    private Damagable targetD;
    private float life = 100;
    private float maxLife = 1;
    public Slider Slider;
    private State _state;

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

    private float timepassed;

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

    private void updateScrollBar()
    {
        Slider.value = life / maxLife;
    }


    private int calls = 0;

    public void takeDamage(float damage)
    {
        life -= damage;
        updateScrollBar();
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

  


    private enum State
    {
        ATTACK,
        RUN,
        DYING
    }
}