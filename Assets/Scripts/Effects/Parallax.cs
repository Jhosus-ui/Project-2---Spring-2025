using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector2 velocidadMovimiento;

    private Vector2 offset;
    private Material material;
    private Rigidbody2D jugadorRB;
    private Vector2 velocidadAnterior;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        jugadorRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        velocidadAnterior = jugadorRB.linearVelocity;
    }

    private void Update()
    {
        Vector2 velocidadActual = jugadorRB.linearVelocity;

        // Solo aplica el efecto parallax si el jugador se est� moviendo
        if (velocidadActual != velocidadAnterior)
        {
            offset = (jugadorRB.linearVelocity.x * 0.1f) * velocidadMovimiento * Time.deltaTime;
            material.mainTextureOffset += offset;
        }

        // Actualiza la velocidad anterior
        velocidadAnterior = velocidadActual;
    }
}