using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = UnityEngine.Random;

public class PlanAnchor : MonoBehaviour
{
    public ARRaycastManager m_RaycastManager;
    public ARPlaneManager m_PlaneManager;

    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public Material selectedMat;
    public Material unselectedMat;
    public Material Grass;

    public ARPlane selectedPlane;

    public GameObject button;

    public GameObject selectPlaneText;
    public TMP_Text text;


    public static bool playing;

    private GameObject Tower;
    public static PlanAnchor instance;

    private void Awake()
    {
        instance = this;
        button.SetActive(false);
        selectPlaneText.SetActive(true);
        Tower = (GameObject)Resources.Load("prefab/twr", typeof(GameObject));
        m_PlaneManager.enabled = true;

        m_PlaneManager.planesChanged += planeChanged;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteRenderer.material.mainTexture = spriteRenderer.sprite.texture;
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;


        if (!playing && Input.GetTouch(0).phase == TouchPhase.Ended &&
            m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            HandleRaycast(m_Hits[Random.Range(0, m_Hits.Count)]);
        }

        if (selectedPlane != null && !playing)
        {
            button.SetActive(true);
            selectPlaneText.SetActive(false);
        }


        if (playing)
        {
            m_PlaneManager.enabled = false;
            foreach (var plane in m_PlaneManager.trackables)
            {
                if (!plane.Equals(selectedPlane))
                    Destroy(plane.gameObject);
            }
        }
    }

    void HandleRaycast(ARRaycastHit hit)
    {
        if ((hit.hitType & TrackableType.Planes) != 0)
        {
            if (!playing && m_PlaneManager.GetPlane(hit.trackableId).gameObject.activeInHierarchy)
            {
                MeshRenderer mr;
                if (selectedPlane != null)
                {
                    mr = selectedPlane.GetComponent<MeshRenderer>();

                    mr.material = unselectedMat;
                }

                selectedPlane = m_PlaneManager.GetPlane(hit.trackableId);
                mr = selectedPlane.GetComponent<MeshRenderer>();

                mr.material = selectedMat;
            }
        }
    }

    public void play()
    {
        playing = true;
        button.SetActive(false);
        MeshRenderer mr = selectedPlane.GetComponent<MeshRenderer>();

        mr.material = Grass;

        Vector3 v = selectedPlane.center;
        v.y += 0.2f;

        Instantiate(Tower, v, Quaternion.Euler(0, 0, 0));
        GameManager.instance.GameUI.SetActive(true);
    }

  

    private void planeChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added != null && args.added.Count > 0)
        {
        }

        foreach (var plane in m_PlaneManager.trackables)
        {
            if (plane.extents.x * plane.extents.y > 0.5)
            {
                plane.gameObject.SetActive(true);
            }
            else
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}