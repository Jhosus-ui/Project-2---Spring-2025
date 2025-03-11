using UnityEngine;
using System.Collections.Generic;

public class GeneradorMapa : MonoBehaviour
{
    public int anchoMapa = 10; // Ancho del mapa en celdas
    public int altoMapa = 10;  // Alto del mapa en celdas
    public float tama�oCelda = 10f; // Tama�o de cada celda en unidades de Unity
    public GameObject prefabPrimeraHabitacion; // Prefab de la primera habitaci�n
    public GameObject[] prefabsHabitaciones; // Prefabs de las habitaciones
    public int numeroHabitaciones = 10; // N�mero de habitaciones a generar

    private List<Vector2> posicionesHabitaciones = new List<Vector2>();

    void Start()
    {
        GenerarCuadricula();
        ColocarPrimeraHabitacion();
        GenerarHabitacionesAleatorias();
        ConectarHabitaciones();
    }

    void GenerarCuadricula()
    {
        for (int x = 0; x < anchoMapa; x++)
        {
            for (int y = 0; y < altoMapa; y++)
            {
                Vector2 posicion = new Vector2(x * tama�oCelda, y * tama�oCelda);
                Debug.DrawLine(posicion, posicion + new Vector2(tama�oCelda, 0), Color.white, 100f);
                Debug.DrawLine(posicion, posicion + new Vector2(0, tama�oCelda), Color.white, 100f);
            }
        }
    }

    void ColocarPrimeraHabitacion()
    {
        Vector2 posicionInicial = new Vector2((anchoMapa / 2) * tama�oCelda, (altoMapa / 2) * tama�oCelda);
        posicionesHabitaciones.Add(posicionInicial);
        Instantiate(prefabPrimeraHabitacion, posicionInicial, Quaternion.identity);
    }

    void GenerarHabitacionesAleatorias()
    {
        for (int i = 0; i < numeroHabitaciones; i++)
        {
            Vector2 nuevaPosicion = ObtenerNuevaPosicion();
            posicionesHabitaciones.Add(nuevaPosicion);
            InstanciarHabitacion(nuevaPosicion);
        }
    }

    Vector2 ObtenerNuevaPosicion()
    {
        Vector2 posicionAleatoria = posicionesHabitaciones[Random.Range(0, posicionesHabitaciones.Count)];
        Vector2 direccion = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2)) * tama�oCelda;

        while (posicionesHabitaciones.Contains(posicionAleatoria + direccion))
        {
            direccion = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2)) * tama�oCelda;
        }

        return posicionAleatoria + direccion;
    }

    void InstanciarHabitacion(Vector2 posicion)
    {
        GameObject habitacion = prefabsHabitaciones[Random.Range(0, prefabsHabitaciones.Length)];
        Instantiate(habitacion, posicion, Quaternion.identity);
    }

    void ConectarHabitaciones()
    {
        for (int i = 0; i < posicionesHabitaciones.Count - 1; i++)
        {
            Vector2 habitacionActual = posicionesHabitaciones[i];
            Vector2 habitacionSiguiente = posicionesHabitaciones[i + 1];

            CrearPasillo(habitacionActual, habitacionSiguiente);
        }
    }

    void CrearPasillo(Vector2 inicio, Vector2 fin)
    {
        Debug.Log($"Creando pasillo desde {inicio} hasta {fin}");
        // L�gica para crear un pasillo entre dos habitaciones
    }
}