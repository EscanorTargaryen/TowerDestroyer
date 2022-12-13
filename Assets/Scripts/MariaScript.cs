using UnityEngine;

public class MariaScript : MonoBehaviour
{
    float speed = 0.1f;
    public Animator anim;
    private bool attacking = false;
    private Animator _animator;

    private void Awake()
    {
        transform.LookAt(GameManager.target.transform);
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
    }

    private float timepassed;

    void Update()
    {
        if (!attacking)
        {
            var step = speed * Time.deltaTime;

            Vector3 target = GameManager.target.transform.position;
            target.y = transform.position.y;

            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
        else
        {
            timepassed += Time.deltaTime;
            if (timepassed > _animator.GetCurrentAnimatorStateInfo(0).length)
            {
                timepassed = 0;
                Tower.towerTakeDamage(100);
            }
        }
    }

   /* public string GetCurrentClipName()
    {
        var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        return clipInfo[0].clip.name;
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
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Target"))
        {
            attacking = true;

            anim.SetTrigger("attack");
        }
    }
}