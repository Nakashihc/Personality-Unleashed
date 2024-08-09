using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class TargetKunci : MonoBehaviour
{
    [Header("Kamera")]
    [SerializeField] private Camera KameraUtama;
    [SerializeField] private CinemachineFreeLook kameraCinemachine;

    [Header("Aim Icon")]
    [SerializeField] private Image aimIcon;

    [Header("Settings")]
    [SerializeField] private string tagMusuh;
    [SerializeField] private Vector2 TargetOffset;
    [SerializeField] private float JarakMin;
    [SerializeField] private float JarakMax;

    [Header("UI")]
    [SerializeField] private GameObject tekanAltUI;

    [Header("Musuh")]
    public bool MusuhDiTargetkan;
    public Transform currentTarget;

    [Header("Script")]
    public KarakterGerak karakterGerak;

    private List<Transform> targetDalamJangkauan = new List<Transform>();
    private int currentTargetIndex = 0;
    private float mouseX;
    private float mouseY;

    private bool lockTargetAktif = false;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        kameraCinemachine.m_XAxis.m_InputAxisName = "";
        kameraCinemachine.m_YAxis.m_InputAxisName = "";

        tekanAltUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            lockTargetAktif = !lockTargetAktif;
            if (lockTargetAktif)
            {
                TargetPasang();
            }
            else
            {
                MusuhDiTargetkan = false;
                currentTarget = null;
                tekanAltUI.SetActive(true);
                karakterGerak.animator.SetBool("DiamLock", false);
                karakterGerak.animator.SetBool("LockTarget", false);
            }
        }

        if (lockTargetAktif)
        {
            CekTargetJangkauan();

            if (MusuhDiTargetkan && currentTarget)
            {
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                if (distanceToTarget > JarakMax)
                {
                    GantiTarget();
                }
                else
                {
                    NewInputTarget(currentTarget);
                    tekanAltUI.SetActive(false);
                }
            }
            else
            {
                tekanAltUI.SetActive(true);
                karakterGerak.animator.SetBool("DiamLock", false);
                karakterGerak.animator.SetBool("LockTarget", false);
                lockTargetAktif = false;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                GantiTarget();
            }
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        if (aimIcon)
        {
            aimIcon.gameObject.SetActive(MusuhDiTargetkan);
        }

        kameraCinemachine.m_XAxis.m_InputAxisValue = mouseX;
        kameraCinemachine.m_YAxis.m_InputAxisValue = mouseY;
    }

    private void CekTargetJangkauan()
    {
        targetDalamJangkauan.Clear();
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tagMusuh);

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - transform.position;
            float distance = diff.magnitude;

            if (distance >= JarakMin && distance <= JarakMax)
            {
                targetDalamJangkauan.Add(go.transform);
            }
        }

        if (targetDalamJangkauan.Count > 0)
        {
            tekanAltUI.SetActive(false);
            if (!MusuhDiTargetkan)
            {
                TargetPasang();
            }
        }
        else if (MusuhDiTargetkan)
        {
            MusuhDiTargetkan = false;
            currentTarget = null;
            tekanAltUI.SetActive(true);
            karakterGerak.animator.SetBool("DiamLock", false);
            karakterGerak.animator.SetBool("LockTarget", false);
        }
    }

    private void TargetPasang()
    {
        if (targetDalamJangkauan.Count > 0)
        {
            currentTargetIndex = 0;
            currentTarget = targetDalamJangkauan[currentTargetIndex];
            MusuhDiTargetkan = true;
            karakterGerak.animator.SetBool("LockTarget", true);
            karakterGerak.animator.SetBool("DiamLock", true);
            karakterGerak.animator.SetTrigger("Diamm");
        }
        else
        {
            MusuhDiTargetkan = false;
            currentTarget = null;
        }
    }

    private void GantiTarget()
    {
        if (targetDalamJangkauan.Count <= 1) return;

        currentTargetIndex = (currentTargetIndex + 1) % targetDalamJangkauan.Count;
        currentTarget = targetDalamJangkauan[currentTargetIndex];
    }

    private void NewInputTarget(Transform target)
    {
        if (!currentTarget) return;

        Vector3 viewPos = KameraUtama.WorldToViewportPoint(target.position);

        if (aimIcon)
        {
            aimIcon.transform.position = KameraUtama.WorldToScreenPoint(target.position);
        }

        if ((target.position - transform.position).magnitude < JarakMin) return;
        mouseX = (viewPos.x - 0.5f + TargetOffset.x) * 3f;
        mouseY = (viewPos.y - 0.5f + TargetOffset.y) * 3f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, JarakMax);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, JarakMin);
    }
}
