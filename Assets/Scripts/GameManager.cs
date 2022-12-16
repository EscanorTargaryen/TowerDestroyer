using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The target you have to destroy, in this case the central tower
    /// </summary>
    public static GameObject target;

    /// <summary>
    /// Maria's GameObject (the warrior)
    /// </summary>
    public static GameObject maria;

    /// <summary>
    /// Mutant's GameObject (the warrior)
    /// </summary>
    public static GameObject mutant;

    /// <summary>
    /// The GameObject of the encampment that spawns Maria out.
    /// </summary>
    private GameObject Camp;

    /// <summary>
    /// You can only place camps every few seconds, this indicates whether the required time has passed
    /// </summary>
    private bool canPlace = true;

    /// <summary>
    /// The number of tree logs
    /// </summary>
    public int logs;

    /// <summary>
    /// The logs number in the UI
    /// </summary>
    public TMP_Text logsText;

    /// <summary>
    /// The timer in UI
    /// </summary>
    public TMP_Text timerText;

    /// <summary>
    /// The list of the rays in AR
    /// </summary>
    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    /// <summary>
    /// If the camps button is selected.
    /// </summary>
    public bool CampButtonSelected = false;

    /// <summary>
    /// The cost of a camps in logs
    /// </summary>
    public int campCost = 1;

    /// <summary>
    /// The in game UI
    /// </summary>
    public GameObject GameUI;

    public static GameManager instance;

    /// <summary>
    /// Tree's GameObject
    /// </summary>
    public static GameObject Tree;

    /// <summary>
    /// You can place camps only when the button is selected
    /// </summary>
    public Button CampButton;

    public GraphicRaycaster GraphicRaycaster;

    /// <summary>
    /// List of all Maria in the game
    /// </summary>
    public static List<GameObject> mariaList = new List<GameObject>();

    /// <summary>
    /// List of all Mutant in the game
    /// </summary>
    public static List<GameObject> MutantList = new List<GameObject>();

    /// <summary>
    /// the elapsed time of the game
    /// </summary>
    private float timer;

    /// <summary>
    /// Keeps the time of how often you have to make a tree spawn
    /// </summary>
    private float timePassed;

    private void Awake()
    {
        instance = this;
        maria = (GameObject)Resources.Load("prefab/Maria", typeof(GameObject));
        mutant = (GameObject)Resources.Load("prefab/Mutant", typeof(GameObject));
        Tree = (GameObject)Resources.Load("prefab/Tree", typeof(GameObject));
        Camp = (GameObject)Resources.Load("prefab/Camp", typeof(GameObject));
        GameUI.SetActive(false);
    }

    /// <summary>
    /// Spawn a new tree
    /// </summary>
    private void spawnTree()
    {
        if (PlanAnchor.playing)
        {
            Vector3 pos = PlanAnchor.instance.selectedPlane.center;

            pos.x += Random.Range(-0.6f, 0.6f);
            pos.z += Random.Range(-0.6f, 0.6f);

            Instantiate(Tree, pos, Quaternion.identity);
        }
    }


    void Update()
    {
        if (!GameOverScreen.GameOver && PlanAnchor.playing)
        {
            TimeSpan t = TimeSpan.FromSeconds(timer);
            timer += Time.deltaTime;

            string timerString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                t.Hours,
                t.Minutes,
                t.Seconds);
            
            timerText.text = timerString;

            ForceSelectGameObject();

            timePassed += Time.deltaTime;
            if (timePassed > 5)
            {
                spawnTree();

                timePassed = 0;
            }

            if (logs >= campCost && canPlace)
            {
                CampButton.interactable = true;
            }
            else
            {
                CampButton.interactable = false;
                CampButtonSelected = false;
            }

            if (Input.touchCount > 0)
            {
                //Check if you are touching the button
                PointerEventData ped = new PointerEventData(null);

                ped.position = Input.GetTouch(0).position;

                List<RaycastResult> results = new List<RaycastResult>();

                GraphicRaycaster.Raycast(ped, results);

                //if yes return
                if (results.Count > 0)
                {
                    return;
                }
                
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        GameObject obj = hit.collider.gameObject;

                        if (hit.collider.tag.Equals("Tree"))
                        {
                            Animator animator = obj.GetComponent<Animator>();
                            TreeScript s = obj.GetComponent<TreeScript>();
                            if (s.canTake)
                            {
                                animator.SetTrigger("Decap");
                                s.canTake = false;
                                logs++;
                                UpdateLogs();
                                Destroy(obj,2);
                                return;
                            }
                        }
                    }
                }

                if (PlanAnchor.playing && canPlace && logs >= campCost && CampButton.IsInteractable() &&
                    EventSystem.current.currentSelectedGameObject == CampButton.gameObject)
                {
                    if (target == null)
                    {
                        target = GameObject.FindWithTag("Target");
                    }


                    if (
                        PlanAnchor.instance.m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
                    {
                        HandleRaycast(m_Hits[Random.Range(0, m_Hits.Count)]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Force if the button of Camp is selected
    /// </summary>
    private void ForceSelectGameObject()
    {
        if (CampButtonSelected)
            EventSystem.current.SetSelectedGameObject(CampButton.gameObject);
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// Spawn a new Camp
    /// </summary>
    /// <param name="hit"></param>
    void HandleRaycast(ARRaycastHit hit)
    {
        if ((hit.hitType & TrackableType.Planes) != 0)
        {
            Vector3 pos = hit.pose.position;

            pos.y += 0.15f;

            GameObject m = Instantiate(Camp, pos, hit.pose.rotation);
            logs -= campCost;
            UpdateLogs();
            canPlace = false;
            StartCoroutine(wait());
        }
    }

    /// <summary>
    /// Update the number of logs in UI
    /// </summary>
    private void UpdateLogs()
    {
        logsText.text = logs + "";
    }

    /// <summary>
    /// Wait as long as necessary before you can place a new camp
    /// </summary>
    /// <returns></returns>
    public IEnumerator wait()
    {
        yield return new WaitForSeconds(5f);
        canPlace = true;
    }

    /// <summary>
    /// Handle the click on the camp button
    /// </summary>
    public void campcClick()
    {
        CampButtonSelected = !CampButtonSelected;
    }
}