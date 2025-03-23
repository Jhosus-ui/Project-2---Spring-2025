using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{

    [SerializeField] private Vector2 velocidadMovimiento;

    private Vector2 offset;
    private Material material;

    private Rigidbody2D jugadorRB;

    private void Awake()
    {
       material = GetComponent<SpriteRenderer>().material;
       jugadorRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        offset = (jugadorRB.linearVelocity.x * 0.1f) * velocidadMovimiento * Time.deltaTime;
        material.mainTextureOffset += offset;
    }

}
