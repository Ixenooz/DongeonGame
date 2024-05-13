using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCharacterScript : MonoBehaviour
{
    // Variables
    public float moveSpeed = 5.0f; // Vitesse de déplacement du personnage
    public Rigidbody2D rb; // Rigidbody du personnage
    public Animator animator; // Animator du personnage
    public SpriteRenderer spriteRenderer; // Sprite Renderer du personnage
    Vector2 dir; // Direction que l'on souhaite appliquer au personnage
    int dirValue = 0; // 0 = Idle ; 1 = Down ; 2 = Side ; 3 = UP
    

    // Fonction Unity
    public void HandleUpdate()
    {
        HandleKeys();
        HandleMove();
    }

    // Fonction custom
    public void HandleKeys() 
    {
        if(Input.GetKey(KeyCode.RightArrow)) // Droite
        {
            dir = Vector2.right;
            dirValue = 2;
            spriteRenderer.flipX = false;
        }
        else if(Input.GetKey(KeyCode.LeftArrow)) // Gauche
        {
            dir = Vector2.left;
            dirValue = 2;
            spriteRenderer.flipX = true;
        }
        else if(Input.GetKey(KeyCode.UpArrow)) // Haut
        {
            dir = Vector2.up;
            dirValue = 3;
        }
        else if(Input.GetKey(KeyCode.DownArrow)) // Bas
        {
            dir = Vector2.down;
            dirValue = 1;
        }
        else // Jouer les animations d'Idle en fonction de la dernière direction (dir) : 0 = IdleDown, 12 = IdleSide, 13 = IdleUp
        {
            if(dir == Vector2.down)
            {
                dir = Vector2.zero; // Rien
                dirValue = 0; 
            }
            else if(dir == Vector2.right || dir == Vector2.left)
            {
                dir = Vector2.zero; // Rien
                dirValue = 12;
            }
            else if(dir == Vector2.up)
            {
                dir = Vector2.zero; // Rien
                dirValue = 13;
            }
                
        }
    }

    // Gestion du mouvement
    public void HandleMove()
    {
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime); // On souhaite appliquer le mouvement au Rigidbody
        animator.SetInteger("dir", dirValue); // On accède au paramètre dir du animator et on lui attribue dirValue
    }
}
