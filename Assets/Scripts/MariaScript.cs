using UnityEngine;

public class MariaScript : MonoBehaviour
{
    float speed = 0.1f;
    public Animator anim;
    private bool attacking = false;

    private void Awake()
    {
        transform.LookAt(GameManager.target.transform);
    }

    void Start()
    {
    }


    void Update()
    {
        if (!attacking)
        {
            var step = speed * Time.deltaTime;

            Vector3 target = GameManager.target.transform.position;
            target.y = transform.position.y;

            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Target"))
        {
            attacking = true;

            anim.SetTrigger("attack");
        }
    }
}