using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    SpriteRenderer imagen;

    public Sprite front;
    public Sprite back;
    public string nombre;
    public int valor;
    public int indiceEnListaCartas;

    public GameManager gameManager;

    private bool frontCard = false;

    void Start()
    {
        imagen = GetComponent<SpriteRenderer>();
    }

    public void invertirCarta()
    {
        if (!frontCard)
        {
            imagen.sprite = front;
            frontCard = true;
        }
        else
        {
            imagen.sprite = back;
            frontCard = false;
        }
    }

    private void OnMouseDown()
    {
        if (!frontCard)
        {
            invertirCarta();
            gameManager.ClickOnCard(nombre, valor, indiceEnListaCartas);
        }
        else
        {
            invertirCarta();
        }
    }

    void Update()
    {

    }
}