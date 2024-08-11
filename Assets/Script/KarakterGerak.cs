using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarakterGerak : MonoBehaviour
{
    [Header("Kecepatan Bergerak")]
    public float Jalan = 2f;
    public float Lari = 6f;

    [Header("Animator")]
    public Animator animator;

    [Header("Kamera & Script")]
    public Transform Kamera;
    public TargetKunci targetKunci;

    private int idleV = 0;
    private int lastIdleV = -1;
    private float idleTimer = 0f;
    private bool isIdle = false;

    private void Update()
    {
        if (targetKunci.DiTargetkan)
        {
            // Liat Musuh
            Transform NPC = targetKunci.currentTarget;
            if (NPC != null)
            {
                Vector3 directionToTarget = (NPC.position - transform.position).normalized;
                directionToTarget.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Jalan * Time.deltaTime * 100);
            }
            animator.SetBool("LockTarget", true);

            HandleMovement();
        }
        else
        {
            animator.SetBool("LockTarget", false);

            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forward = Kamera.forward;
        Vector3 right = Kamera.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        float currentSpeed = Jalan;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = Lari;
            animator.SetBool("Lari", true);
            animator.SetBool("Jalan", false);
        }
        else
        {
            animator.SetBool("Lari", false);
        }

        transform.Translate(movement * currentSpeed * Time.deltaTime, Space.World);

        if (movement != Vector3.zero)
        {
            isIdle = false;
            idleTimer = 0f;

            animator.SetBool("Jalan", true);
            if (targetKunci.lockTargetAktif)
            {
                // Animasi W A S D Waktu Ngunci Target
                // Ke kanan
                if (Input.GetKey(KeyCode.D))
                {
                    animator.SetBool("DiamLock", false);
                    animator.ResetTrigger("Mundur");
                    animator.ResetTrigger("Kiri");
                    animator.ResetTrigger("JalanLock");
                    animator.ResetTrigger("Diamm");
                    animator.SetTrigger("Kanan");
                }
                // Ke kiri
                else if (Input.GetKey(KeyCode.A))
                {
                    animator.SetBool("DiamLock", false);
                    animator.ResetTrigger("Mundur");
                    animator.ResetTrigger("Kanan");
                    animator.ResetTrigger("JalanLock");
                    animator.ResetTrigger("Diamm");
                    animator.SetTrigger("Kiri");
                }
                // Mundur
                else if (Input.GetKey(KeyCode.S))
                {
                    animator.SetBool("DiamLock", false);
                    animator.ResetTrigger("Kiri");
                    animator.ResetTrigger("Kanan");
                    animator.ResetTrigger("JalanLock");
                    animator.ResetTrigger("Diamm");
                    animator.SetTrigger("Mundur");
                }
                // Maju
                else if (Input.GetKey(KeyCode.W))
                {
                    animator.SetBool("DiamLock", false);
                    animator.ResetTrigger("Kiri");
                    animator.ResetTrigger("Kanan");
                    animator.ResetTrigger("Mundur");
                    animator.ResetTrigger("Diamm");
                    animator.SetTrigger("JalanLock");
                }
                // Diam!
                else
                {
                    animator.SetBool("DiamLock", true);
                    animator.ResetTrigger("Mundur");
                    animator.ResetTrigger("Kanan");
                    animator.ResetTrigger("Mundur");
                    animator.ResetTrigger("JalanLock");
                    animator.SetTrigger("Diamm");
                }
            }

            // Bergerak Tidak Mengunci Target
            if (!targetKunci.DiTargetkan)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, currentSpeed * Time.deltaTime * 100);
            }
        }
        else
        {
            animator.SetBool("Jalan", false);
            isIdle = true;
            idleTimer += Time.deltaTime;
            animator.SetBool("Diam", true);

            if (isIdle && idleTimer >= 5f)
            {
                int newIdleV = Random.Range(0, 3);

                if (newIdleV != lastIdleV)
                {
                    idleV = newIdleV;
                    animator.SetBool("Diam", false);
                    animator.SetInteger("Idle", idleV);
                    lastIdleV = idleV;
                }
                idleTimer = 0f;
            }
        }
    }
}
