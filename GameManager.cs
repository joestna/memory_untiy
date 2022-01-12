using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// FALTA POR HACER :
// COMPLETAR EL EJERCICIO 11
// REFACTORIZAR CODIGIO


// Juego de cartas MEMORY
public class GameManager : MonoBehaviour
{
    public GameObject cartaPrefab;

    public GameObject objetoTextoContadorParejas;
    private Text textoContadorParejas;
    public GameObject objetoTextoFinalJuego;
    private Text textoFinalJuego;
    

    private List<GameObject> listaCartas = new List<GameObject>();

    public List<Sprite> listaCartasFrontales = new List<Sprite>();
    public List<int> cartasYaUtilizadas = new List<int>();

    int contadorParejas = 0;


    private string[,] informacionCartas =
    {
        {"Dowager Queen", "Guard", "Assasin", "Bishop", "Constable", "Jester", "Count", "Sycophant", "Baroness", "Cardinal"},
        {"7", "1", "0", "9", "6", "0", "5", "4", "3", "2" }
    };

    int cartaRepetida;
    int randomFrontal;
    int cartaContada = 0;

    string nombreCarta = "carta";

    void Start()
    {
        int numeroDeCartasACrear = 10;
        int x = -10;
        int y = 3;


        for (int i = 0; i < numeroDeCartasACrear; i++)
        {

            // Evitar que la carta se repita mas de 1 vez
            // El primer algoritmo era mas sencillo porque el numero alatorio era solo de 0 a 5 teniendo en cuenta que eran 10 cartas
            // Algoritmo mejorado que coge 5 cartas aleatorias de entre una baraja de 10 (numero aleatorio 0 al 9) y las genera dos veces en posiciones aletorias en el tablero
            do
            {
                cartaRepetida = 0;
                randomFrontal = Random.Range(0, 10); //IMPORTANTE el ultimo numero no se cuenta en el rango ( el rango real es 0-9 )
                Debug.Log("Prueba" + randomFrontal);


                for (int j = 0; j < cartasYaUtilizadas.Count; j++)
                {
                    if (randomFrontal == cartasYaUtilizadas[j])
                    {
                        cartaRepetida++;
                    }

                }

            } while (cartaRepetida != 0 && cartaContada < 5 || cartaRepetida != 1 && cartaContada >= 5);

            cartasYaUtilizadas.Add(randomFrontal);


            // Instanciar la carta en el tablero
            GameObject carta = Instantiate(cartaPrefab, new Vector3(x, y, 0), Quaternion.identity);

            CardScript cartaScript = carta.GetComponent<CardScript>();

            cartaScript.gameManager = GetComponent<GameManager>();
            cartaScript.front = listaCartasFrontales[randomFrontal];
            cartaScript.nombre = informacionCartas[0, randomFrontal]; // Nombre de la carta
            cartaScript.valor = int.Parse(informacionCartas[1, randomFrontal]);
            cartaScript.indiceEnListaCartas = i;

            carta.name = informacionCartas[0, randomFrontal]; // Nombre del GameObject carta

            listaCartas.Add(carta);

            x += 3;

            if (i == 5 - 1)
            {
                x = -10;
                y = -3;
            }

            cartaContada++;


        }

        textoContadorParejas = objetoTextoContadorParejas.GetComponent<Text>();
        textoContadorParejas.text = "Numero Parejas : " + contadorParejas;
        textoContadorParejas.fontSize = 25;

        objetoTextoFinalJuego.SetActive(false);
    }

    /* Variables para gestionar si las cartas hacen o no hacen pareja */
    int[] cartasIguales = new int[2];
    int cartaSeleccionada = 0;
    

    // EL CODIGO NO ES PERFECTO, SI LA PRIMERA CARTA DE LA PAREJA LA VOLTEAMOS Y DESPUES LA PONEMOS BOCA ABAJO EL JUEGO SE ROMPE
    //DESACTIVAR EL BOX COLIDER DE LA 1RA CARTA QUE SELECCIONAMOS ( LA PRIMERA DE LA PAREJA )
    public void ClickOnCard(string nombre, int valor, int indiceEnListaCartas)
    {
        cartaSeleccionada++;
        Debug.Log(cartaSeleccionada); /* Prueba para ver si esta funcionando correctamente */

        if (cartaSeleccionada == 1 || cartaSeleccionada > 2)
        {
            cartaSeleccionada = 1;
            cartasIguales[cartaSeleccionada - 1] = indiceEnListaCartas;
        }
        else if (cartaSeleccionada == 2)
        {
            cartasIguales[cartaSeleccionada - 1] = indiceEnListaCartas;

            if (listaCartas[cartasIguales[0]].GetComponent<CardScript>().nombre == listaCartas[cartasIguales[1]].GetComponent<CardScript>().nombre)
            {
                Debug.Log("Las cartas son iguales");

                listaCartas[cartasIguales[0]].SetActive(false);
                listaCartas[cartasIguales[1]].SetActive(false);

                contadorParejas++;
                textoContadorParejas.text = "Num. Parejas : " + contadorParejas;
            }
            else
            {
                Debug.Log("No hay pareja");

                StartCoroutine(WaitAndPrint(listaCartas[cartasIguales[0]].GetComponent<CardScript>(), listaCartas[cartasIguales[1]].GetComponent<CardScript>() ));
            }

            cartasIguales[0] = 0;
            cartasIguales[1] = 0;

            cartaSeleccionada = 0;
        }
    }

    // IMPORTANTE : Esta funcion es una Coroutine, es multihilo, por lo que se ejecutara en paralelo respecto al hilo principal
    IEnumerator WaitAndPrint( CardScript carta1, CardScript carta2 )
    {
        // Desactiva todos los boxColider para que cuando dos cartas no son pareja y las este mostrando 2 segundos no se puedan pulsar las otras cartas
        /*foreach (GameObject carta in listaCartas)
        {
            carta.GetComponent<BoxCollider>().enabled = false;
        }*/

        habilitarColliders(false);

        yield return new WaitForSeconds(2);

        carta1.invertirCarta();
        carta2.invertirCarta();


        habilitarColliders(true);
        // Reactiva los boxColider
        /*foreach (GameObject carta in listaCartas)
        {
            carta.GetComponent<BoxCollider>().enabled = true;
        }*/
    }

    private void habilitarColliders( bool trueOrFalse )
    {
        foreach (GameObject carta in listaCartas)
        {
            carta.GetComponent<BoxCollider>().enabled = trueOrFalse;
        }
    }

    void Update()
    {

        if (contadorParejas >= 5)
        {
            habilitarColliders(false);
            objetoTextoContadorParejas.SetActive(false);

            textoFinalJuego = objetoTextoFinalJuego.GetComponent<Text>();
            textoFinalJuego.text = "FIN DEL JUEGO";
            textoFinalJuego.fontSize = 30;

            objetoTextoFinalJuego.SetActive(true);

        }
    }
}