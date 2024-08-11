using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class TargetKunci : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] public string NamaPlayer;

    [Header("Kamera")]
    [SerializeField] private Camera KameraUtama;
    [SerializeField] private CinemachineFreeLook kameraCinemachine;

    [Header("Aim Icon")]
    [SerializeField] private Image aimIcon;
    [SerializeField] private float lokasiAimIcon;

    [Header("Settings")]
    [SerializeField] private string Target;
    [SerializeField] private Vector2 TargetOffset;
    [SerializeField] private float JarakMin;
    [SerializeField] private float JarakMax;

    [Header("UI")]
    [SerializeField] private GameObject tekanFUI;
    [SerializeField] private GameObject tekanEUI;

    [Header("Target")]
    public bool DiTargetkan;
    public Transform currentTarget;

    [Header("Script")]
    public KarakterGerak karakterGerak;

    [Header("Dialog Settings")]
    [SerializeField] private VisualDialog visualDialog;
    public bool isDialogActive = false;

    private List<Transform> targetDalamJangkauan = new List<Transform>();
    private int currentTargetIndex = 0;
    private float mouseX;
    private float mouseY;

    public bool lockTargetAktif = false;

    void Start()
    {
        kameraCinemachine.m_XAxis.m_InputAxisName = "";
        kameraCinemachine.m_YAxis.m_InputAxisName = "";

        tekanFUI.SetActive(false);
        tekanEUI.SetActive(false);
    }

    void Update()
    {
        CekTargetJangkauan();

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
                DiTargetkan = false;
                currentTarget = null;
                tekanEUI.SetActive(false);
                karakterGerak.animator.SetBool("DiamLock", false);
                karakterGerak.animator.SetBool("LockTarget", false);
            }
        }

        if (lockTargetAktif)
        {
            if (currentTarget == null || !targetDalamJangkauan.Contains(currentTarget))
            {
                GantiTarget();
            }

            if (DiTargetkan && currentTarget)
            {
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                if (distanceToTarget >= JarakMax)
                {
                    GantiTarget();
                }
                else
                {
                    NewInputTarget(currentTarget);
                }
            }
            else
            {
                lockTargetAktif = false;
                DiTargetkan = false;
                karakterGerak.animator.SetBool("DiamLock", false);
                karakterGerak.animator.SetBool("LockTarget", false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (visualDialog != null)
                {
                    visualDialog.StartDialog();
                }
                tekanEUI.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Tab) && !isDialogActive)
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
            aimIcon.gameObject.SetActive(DiTargetkan);
        }

        kameraCinemachine.m_XAxis.m_InputAxisValue = mouseX;
        kameraCinemachine.m_YAxis.m_InputAxisValue = mouseY;
    }

    private void CekTargetJangkauan()
    {
        targetDalamJangkauan.Clear();
        GameObject[] gos = GameObject.FindGameObjectsWithTag(Target);

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - transform.position;
            float distance = diff.magnitude;

            if (distance >= JarakMin && distance <= JarakMax)
            {
                targetDalamJangkauan.Add(go.transform);
            }
        }

        if (targetDalamJangkauan.Count == 0)
        {
            DiTargetkan = false;
            lockTargetAktif = false;
            currentTarget = null;
            karakterGerak.animator.SetBool("DiamLock", false);
            karakterGerak.animator.SetBool("LockTarget", false);
        }
    }

    private void TargetPasang()
    {
        tekanEUI.SetActive(true);

        if (targetDalamJangkauan.Count > 0)
        {
            currentTargetIndex = 0;
            currentTarget = targetDalamJangkauan[currentTargetIndex];
            DiTargetkan = true;
            karakterGerak.animator.SetBool("LockTarget", true);
            karakterGerak.animator.SetBool("DiamLock", true);
            karakterGerak.animator.SetTrigger("Diamm");

            visualDialog = currentTarget.GetComponent<VisualDialog>();
        }
        else
        {
            DiTargetkan = false;
            currentTarget = null;
            visualDialog = null;
        }
    }

    private void GantiTarget()
    {
        if (targetDalamJangkauan.Count < 1) return;

        currentTargetIndex = (currentTargetIndex + 1) % targetDalamJangkauan.Count;
        currentTarget = targetDalamJangkauan[currentTargetIndex];
        visualDialog = currentTarget.GetComponent<VisualDialog>();
    }

    private void NewInputTarget(Transform target)
    {
        if (!currentTarget) return;

        Vector3 viewPos = KameraUtama.WorldToViewportPoint(target.position);

        Vector3 screenPos = KameraUtama.WorldToScreenPoint(target.position);

        if (aimIcon)
        {
            aimIcon.transform.position = new Vector3(screenPos.x, lokasiAimIcon, screenPos.z);
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
