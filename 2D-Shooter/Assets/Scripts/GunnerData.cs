using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerData : ScriptableObject
{
    [Header("MOVEMENT")]
    public float moveSpeed = 50f;
    public float jumpSpeed = 100f;

    [Header("CHECK VARIABLES")]
    public float groundCheckDistance = 0.5f;
}
