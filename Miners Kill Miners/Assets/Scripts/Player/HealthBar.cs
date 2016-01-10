using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{


    public Slider healthSlider;                                 // Reference to the UI's health bar.
    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


    Animator anim;                                              // Reference to the Animator component.
    AudioSource playerAudio;                                    // Reference to the AudioSource component.

    bool isDead;                                                // Whether the player is dead.
    bool damaged;                                               // True when the player gets damaged.
    //public Roland.GameSceneController controller;

   

    void Update()
    {
       // if (controller.GameHasStarted)
        {
            // If the player has just been damaged...
            if (damaged)
            {
                // ... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            // Otherwise...
            else
            {
                // ... transition the colour back to clear.
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Reset the damaged flag.
            damaged = false;
        }
    }

    public void UpdateHealthBar(int HPNow, int TotalHP)
    {
        // Set the damaged flag so the screen will flash.
        damaged = true;
        healthSlider.value = HPNow;
        healthSlider.maxValue = TotalHP;
    }
}
