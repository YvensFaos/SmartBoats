using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
   [SerializeField]
   private GameObject followThis;

   [SerializeField, Range(0.0f, 1.0f)]
   private float easing = 0.9f;

   private void Update()
   {
      transform.position = Vector3.Lerp(transform.position, followThis.transform.position, easing);
   }
}
