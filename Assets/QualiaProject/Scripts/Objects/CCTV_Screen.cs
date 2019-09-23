using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCTV_Screen : MonoBehaviour {

    public RawImage[] CCTV_Backgrounds;
    public Texture[] camerasTextures;
    public RawImage screen;

	// Automatically display Camera 0
	void Start () {
        screen.texture = camerasTextures[0];
        StartCoroutine(StartPulsing(0));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //When player hits CCTV button, change to the selected screen and start pulsing its button
    public void ChangeColor(int camera)
    {
        StopAllCoroutines();
        screen.texture = camerasTextures[camera];
        MakeAllIconsTransparent();
        StartCoroutine(StartPulsing(camera));
    }

    //Coroutine that calls fading color subroutines
    IEnumerator StartPulsing(int camera)
    {
        while(true)
        {
            StartCoroutine(MakeCyan(camera));
            yield return new WaitForSeconds(.75f);
            StartCoroutine(MakeTransparent(camera));
            yield return new WaitForSeconds(.75f);
        }
        
    }

    IEnumerator MakeTransparent(int camera)
    {
        // fade from opaque to transparent
        // loop over 1 second backwards
        for (float i = .75f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            CCTV_Backgrounds[camera].color = new Color(0f / 255f, 255f / 255f, 255f / 255f, i); 
            yield return null;
        } 
    }

    IEnumerator MakeCyan(int camera)
    {
        // fade from transparent to opaque
        // loop over 1 second
        for (float i = 0; i <= .75f; i += Time.deltaTime)
        {
            // set color with i as alpha
            CCTV_Backgrounds[camera].color = new Color(0f / 255f, 255f / 255f, 255f / 255f, i);
            yield return null;
        }
    }

    void MakeAllIconsTransparent()
    {
        for(int i = 0; i < CCTV_Backgrounds.Length; i++){
            CCTV_Backgrounds[i].color = new Color(0f / 255f, 255f / 255f, 255f / 255f, 0);
        }
    }

}
