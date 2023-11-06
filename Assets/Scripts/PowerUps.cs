using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUps : MonoBehaviour
{
    // Start is called before the first frame update


    public Image powerUpImage1;
    public float cooldown1 = 5.0f;
    bool isCooldown = false;
    public KeyCode powerUp1;


    void Start()
    {
        powerUpImage1.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void PowerUp1 () 
    {
        if (Input.GetKey(powerUp1) && isCooldown == false)
        {
             isCooldown = true;
             powerUpImage1.fillAmount = 1;
        }

        if(isCooldown)
        {
            powerUpImage1.fillAmount -= 1/cooldown1 *Time.deltaTime;

            if (powerUpImage1.fillAmount <= 0)
            {
                powerUpImage1.fillAmount = 0;
                isCooldown = false;
            }
        }
    }
}
