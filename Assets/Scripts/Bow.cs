using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private readonly int IsDrawnHash = Animator.StringToHash("varDrawn");

    public Animator animator;

    public Transform arrowSpawn;
    public GameObject arrowPrefab;
    public AudioClip arrowFire;
    public AudioSource audioSource;
    public float power;

    public bool isDrawing;

    private DateTime startedDraw;

    public void Draw()
    {
        animator.SetBool(IsDrawnHash, true);
    }

    private void Update()
    {
        if (isDrawing)
            Draw();
    }

    public void Fire()
    {
        float percentDrawn = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //Debug.Log(percentDrawn);
        isDrawing = false;
        animator.SetBool(IsDrawnHash, false);
        audioSource.PlayOneShot(arrowFire);

        //// Create a ray from the camera going through the middle of your screen
        //Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //RaycastHit hit;
        //// Check whether your are pointing to something so as to adjust the direction
        //Vector3 targetPoint;
        //if (Physics.Raycast(ray, out hit))
        //    targetPoint = hit.point;
        //else
        //    targetPoint = ray.GetPoint(1000); // You may need to change this value according to your needs
        //                                      // Create the bullet and give it a velocity according to the target point computed before
        //
        //arrow.GetComponent<Rigidbody>().velocity = (targetPoint - arrowSpawn.position).normalized * (power); // * percentDrawn
        
        var arrow = Instantiate(arrowPrefab, arrowSpawn.position, transform.rotation);
    }
}
