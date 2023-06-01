using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Windows;
using UnityEngine;

public class ImageFilterDisplayer : MonoBehaviour
{
    private bool isSurfacePanelOpen = false;
    private bool isColorizationPanelOpen = false;
    private bool isLightingPanelOpen = false;
    private bool isDetailsPanelOpen = false;
    public GameObject[] panels;
    Texture2D texture = null;

    // Start is called before the first frame update
    void Start()
    {
        panels = GameObject.FindGameObjectsWithTag("UI Panel");

        // Check if the Generate Button has been clicked
        GameObject.Find("GenerateButton").GetComponent<Button>().onClick.AddListener(changeIcon);

         // Check if the Filter Reset Button has been clicked
        GameObject.Find("FilterResetButton").GetComponent<Button>().onClick.AddListener(resetFilter);

        // Check if the Depth slider has been moved
        if (true)
        {
            Debug.Log("[Checking file panel for updates...]");
            GameObject.Find("DepthSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateFilePanel();});
        }

        // Check if the Surface Tab has been clicked
        GameObject.Find("SurfaceTab").GetComponent<Button>().onClick.AddListener(openSurfacePanel);
        // Check if a Surface slider has been moved
        if (true)
        {
            Debug.Log("[Checking surface panel for updates...]");
            GameObject.Find("ReflectivitySlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateSurfacePanel();});
            GameObject.Find("TransparencySlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateSurfacePanel();});
            GameObject.Find("NeonSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateSurfacePanel();});
        }

        // Check if a Colorization slider has been moved
        GameObject.Find("ColorizationTab").GetComponent<Button>().onClick.AddListener(openColorizationPanel);
        if (true)
        {
            GameObject.Find("HueSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateColorizationPanel();});
            GameObject.Find("SaturationSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateColorizationPanel();});
            GameObject.Find("NegativitySlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateColorizationPanel();});
        }

        // Check if a Lighting slider has been moved
        GameObject.Find("LightingTab").GetComponent<Button>().onClick.AddListener(openLightingPanel);
        if (true)
        {
            GameObject.Find("BrightnessSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateLightingPanel();});
            GameObject.Find("HighlightsSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateLightingPanel();});
            GameObject.Find("ShadowsSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateLightingPanel();});
        }

        // Check if a Details slider has been moved
        GameObject.Find("DetailsTab").GetComponent<Button>().onClick.AddListener(openDetailsPanel);
        if (true)
        {
            GameObject.Find("SharpenSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateDetailsPanel();});
            GameObject.Find("BlurSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateDetailsPanel();});
            GameObject.Find("GrainSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateDetailsPanel();});
        }
    }

    void Update()
    {
    }

    private void resetFilter()
    {
        float reflectivity = GameObject.Find("ReflectivitySlider").GetComponent<Slider>().value = 0;
        float transparency = GameObject.Find("TransparencySlider").GetComponent<Slider>().value = 0;
        float neon = GameObject.Find("NeonSlider").GetComponent<Slider>().value = 0;
        updateSurfacePanel();

        float hueity = GameObject.Find("HueSlider").GetComponent<Slider>().value = 0;
        float saturation = GameObject.Find("SaturationSlider").GetComponent<Slider>().value = 0;
        float negativity = GameObject.Find("NegativitySlider").GetComponent<Slider>().value = 0;
        updateColorizationPanel();

        float brightness = GameObject.Find("BrightnessSlider").GetComponent<Slider>().value = 0;
        float highlights = GameObject.Find("HighlightsSlider").GetComponent<Slider>().value = 0;
        float shadows = GameObject.Find("ShadowsSlider").GetComponent<Slider>().value = 0;
        updateLightingPanel();

        float sharpness = GameObject.Find("SharpenSlider").GetComponent<Slider>().value = 0;
        float blurIntensity = GameObject.Find("BlurSlider").GetComponent<Slider>().value = 0;
        float grainIntensity = GameObject.Find("GrainSlider").GetComponent<Slider>().value = 0;
        updateDetailsPanel();
    }

    private string verifyFile()
    {
        Debug.Log("[Verifying image...]");

        string pathInput = GameObject.Find("FilePathInputField").GetComponent<InputField>().text;
        string filePath = "";

        if (File.Exists(pathInput))
        {
            Debug.Log("File found!");
            filePath = pathInput; 
        }
        else
        {
            Debug.Log("[Error]: File not found!");
            pathInput = "[Error]: File not found!";
        }

        return filePath;
    }

    private void changeIcon()
    {
        string filePath = verifyFile();
        byte[] fileData = File.ReadAllBytes(filePath);

        texture = new Texture2D(100, 100);
        texture.LoadImage(fileData);

        Image icon = GameObject.Find("ImageIcon").GetComponent<Image>();

        Rect rectangle = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite newSprite = Sprite.Create(texture, rectangle, pivot);

        icon.sprite = newSprite;
    }

    private void updateFilePanel()
    {
        //Debug.Log("[Updating file panel label...]");

        GameObject.Find("DepthLabel").GetComponent<Text>().text = "Depth: " + GameObject.Find("DepthSlider").GetComponent<Slider>().value;
    }

    private void openSurfacePanel()
    {
        Debug.Log("[Toggling surface panel...]");

        if (isSurfacePanelOpen == false)
        {
            //Debug.Log("[Opening surface panel...]");

            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = true;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            isSurfacePanelOpen = !isSurfacePanelOpen;
        }
        else if (isSurfacePanelOpen == true)
        {
            //Debug.Log("[Closing surface panel...]");

            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            isSurfacePanelOpen = !isSurfacePanelOpen;
        }
    }

    private void updateSurfacePanel()
    {
        //Debug.Log("[Updating surface panel labels...]");

        GameObject.Find("ReflectivityLabel").GetComponent<Text>().text = "Reflectivity: " + GameObject.Find("ReflectivitySlider").GetComponent<Slider>().value;
        GameObject.Find("TransparencyLabel").GetComponent<Text>().text = "Transparency: " + GameObject.Find("TransparencySlider").GetComponent<Slider>().value;
        GameObject.Find("NeonLabel").GetComponent<Text>().text = "Neon: " + GameObject.Find("NeonSlider").GetComponent<Slider>().value;
    }

    private void openColorizationPanel()
    {
        Debug.Log("[Toggling colorization panel...]");

        if (isColorizationPanelOpen == false)
        {
            //Debug.Log("[Opening colorization panel...]");

            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            isColorizationPanelOpen = !isColorizationPanelOpen;
        }
        else if (isColorizationPanelOpen == true)
        {
            //Debug.Log("[Closing colorization panel...]");

            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            isColorizationPanelOpen = !isColorizationPanelOpen;
        }
    }

    private void updateColorizationPanel()
    {
        //Debug.Log("[Updating colorization panel labels...]");

        GameObject.Find("HueLabel").GetComponent<Text>().text = "Hue: " + GameObject.Find("HueSlider").GetComponent<Slider>().value;
        GameObject.Find("SaturationLabel").GetComponent<Text>().text = "Saturation: " + GameObject.Find("SaturationSlider").GetComponent<Slider>().value;
        GameObject.Find("NegativityLabel").GetComponent<Text>().text = "Negativity: " + GameObject.Find("NegativitySlider").GetComponent<Slider>().value;
    }

    private void openLightingPanel()
    {
        Debug.Log("[Toggling lighting panel...]");

        if (isLightingPanelOpen == false)
        {
            //Debug.Log("[Opening lighting panel...]");

            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            isLightingPanelOpen = !isLightingPanelOpen;
        }
        else if (isLightingPanelOpen == true)
        {
            //Debug.Log("[Closing lighting panel...]");

            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            isLightingPanelOpen = !isLightingPanelOpen;
        }
    }

    private void updateLightingPanel()
    {
        //Debug.Log("[Updating lighting panel labels...]");

        GameObject.Find("BrightnessLabel").GetComponent<Text>().text = "Brightness: " + GameObject.Find("BrightnessSlider").GetComponent<Slider>().value;
        GameObject.Find("HighlightsLabel").GetComponent<Text>().text = "Highlights: " + GameObject.Find("HighlightsSlider").GetComponent<Slider>().value;
        GameObject.Find("ShadowsLabel").GetComponent<Text>().text = "Shadows: " + GameObject.Find("ShadowsSlider").GetComponent<Slider>().value;
    }

    private void openDetailsPanel()
    {
        Debug.Log("[Toggling details panel...]");

        if (isDetailsPanelOpen == false)
        {
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;

            isDetailsPanelOpen = !isDetailsPanelOpen;
        }
        else if (isDetailsPanelOpen == true)
        {
            //Debug.Log("[Closing lighting panel...]");

            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("SurfacePanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("ColorizationPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("LightingPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("DetailsPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;

            isDetailsPanelOpen = !isDetailsPanelOpen;
        }
    }

    private void updateDetailsPanel()
    {
        //Debug.Log("[Updating details panel labels...]");

        GameObject.Find("SharpenLabel").GetComponent<Text>().text = "Sharpen: " + GameObject.Find("SharpenSlider").GetComponent<Slider>().value;
        GameObject.Find("BlurLabel").GetComponent<Text>().text = "Blur: " + GameObject.Find("BlurSlider").GetComponent<Slider>().value;
        GameObject.Find("GrainLabel").GetComponent<Text>().text = "Grain: " + GameObject.Find("GrainSlider").GetComponent<Slider>().value;
    }
}
