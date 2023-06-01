using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.CoreModule;
using UnityEngine.Windows;
using UnityEngine.UI;
using static PixelNode;
using static StandardShaderUtils;

public class ImageVisualizer : MonoBehaviour
{
    private bool regen; // If an image has been generated
    public string filePath = "";
    //public GameObject plot = GameObject.Find("ImagePlot");
    private List<PixelNode> nodes = new List<PixelNode>();
    public List<GameObject> points = new List<GameObject>();
    public Texture2D texture = null;
    public List<GameObject> meshGroups = new List<GameObject>();
    private CombineInstance[] combine;
    private List<CombineInstance[]> combines = new List<CombineInstance[]>();

    // Start is called before the first frame update
    void Start()
    {
        regen = false;
        Debug.Log("[Starting up image visualization...]");

        // Check if the Generate Button has been clicked
        GameObject.Find("GenerateButton").GetComponent<Button>().onClick.AddListener(generateImage);

        // Check if the Submit Button has been clicked
        GameObject.Find("SubmitButton").GetComponent<Button>().onClick.AddListener(filterImage);

        // Check if the Filter Reset Button has been clicked
        GameObject.Find("FilterResetButton").GetComponent<Button>().onClick.AddListener(resetFilter);

        // Check if the Camera Reset Button has been clicked
        GameObject.Find("CameraResetButton").GetComponent<Button>().onClick.AddListener(resetCamera);

        if (true)
        {
            Debug.Log("[Checking surface panel for updates...]");
            GameObject.Find("DepthSlider").GetComponent<Slider>().onValueChanged.AddListener(delegate{updateDepthLabel();});
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void resetFilter()
    {
        //generateImage();
        resetImage();
    }

    public void resetCamera()
    {
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().fieldOfView = 60;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (texture != null)
        {
            camera.transform.position = new Vector3(texture.width/2, texture.height/2, (texture.width+texture.height) * -1);
        }
        else
        {
            camera.transform.position = new Vector3(0, 0, 0);
        }
    }

    private void updateDepthLabel()
    {
        Debug.Log("[Updating depth label...]");

        GameObject.Find("DepthLabel").GetComponent<Text>().text = "Depth: " + GameObject.Find("DepthSlider").GetComponent<Slider>().value;
    }

    private void wipeImagePlot()
    {
        Debug.Log("[Wiping old image...]");

        for (int i = 0; i < points.Count; i++)
        {
            //Debug.Log("[Destroying point" + i + "...]");
            string name = "PixelPoint" + i;
            Destroy(points[i], 0f);
            //GameObject.Destroy(GameObject.Find(name), 0f);
        }

        points.Clear();
        Debug.Log("Number of Image Points PostWipe: " + points.Count);
    }

    public void generateImage()
    {
        Debug.Log("[Generating image...]");
        filePath = verifyFile();

        if (regen == true)
        {
            wipeImagePlot();
            nodes.Clear();
            Debug.Log("Number of Image Nodes PostWipe: " + nodes.Count);
        }

        if (filePath != "")    
        {
            readBitmap(filePath);
            plotImage();
            regen = true;
            resetCamera();
        }
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

    // reading and parsing CSV file and adding data to appropriate data structures
    private void readBitmap(string filePath)
    {
        Debug.Log("[Reading texture data...]");

        //Bitmap pixelMap = new Bitmap(filePath, true);

        byte[] fileData;
 
            fileData = File.ReadAllBytes(filePath);
            texture = new Texture2D(100, 100);
            texture.LoadImage(fileData);

            int count = 0;

            for(int x = 0; x < texture.width; x++)
            {
                for(int y = 0; y < texture.height; y++)
                {
                    PixelNode node = new PixelNode(count, texture.GetPixel(x, y), y, x);
                    nodes.Add(node);

                    count++;
                }
            }

            Debug.Log("Number of Image Nodes Read: " + nodes.Count);
    }

    private void resetImage()
    {
        Debug.Log("[Resetting image color...]");

        for (int i = 0; i < points.Count; i++)
        {
            string name = "PixelPoint" + i;
            Color defaultColor = points[i].GetComponent<PixelNode>().getDefaultColor();
            Material material = points[i].GetComponent<Renderer>().material;

            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Glossiness", 0f);

            material.DisableKeyword("_EMISSION");

            float localAlpha = ((10f - 0)/10f);
            defaultColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, localAlpha);

            points[i].GetComponent<PixelNode>().setCurrentColor(defaultColor);
            material.color = defaultColor;
        }
    }

    private PixelNode findNode(int row, int col)
    {
        PixelNode node = null;

        return node;
    }

    private float findLuminance(float red, float green, float blue)
    {
        float luminance = (0.2126f * red) + (0.7152f * green) + (0.0722f * blue);

        return luminance;
    }

    private Color findLightestPointColor()
    {
        Color lightest = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < points.Count; i++)
        {
            float pointGrayscale = points[i].GetComponent<PixelNode>().getDefaultColor().grayscale;
            if (pointGrayscale < lightest.grayscale && pointGrayscale > 0f)
            {
                lightest = points[i].GetComponent<PixelNode>().getDefaultColor();
            }
        }

        return lightest;
    }

    private Color findDarkestPointColor()
    {
        Color darkest = new Color(0f, 0f, 0f, 1f);

        for (int i = 0; i < points.Count; i++)
        {
            float pointGrayscale = points[i].GetComponent<PixelNode>().getDefaultColor().grayscale;
            if (pointGrayscale > darkest.grayscale && pointGrayscale < 1f) 
            {
                darkest = points[i].GetComponent<PixelNode>().getDefaultColor();
            }
        }

        return darkest;
    }

    private void filterImage()
    {
        Debug.Log("[Filtering image...]");

        float reflectivity = GameObject.Find("ReflectivitySlider").GetComponent<Slider>().value;
        float transparency = GameObject.Find("TransparencySlider").GetComponent<Slider>().value;
        float neon = GameObject.Find("NeonSlider").GetComponent<Slider>().value;
        float hueity = GameObject.Find("HueSlider").GetComponent<Slider>().value;
        float saturation = GameObject.Find("SaturationSlider").GetComponent<Slider>().value;
        float negativity = GameObject.Find("NegativitySlider").GetComponent<Slider>().value;
        float brightness = GameObject.Find("BrightnessSlider").GetComponent<Slider>().value;
        float highlights = GameObject.Find("HighlightsSlider").GetComponent<Slider>().value;
        float shadows = GameObject.Find("ShadowsSlider").GetComponent<Slider>().value;
        float sharpness = GameObject.Find("SharpenSlider").GetComponent<Slider>().value;
        float blurIntensity = GameObject.Find("BlurSlider").GetComponent<Slider>().value;
        float grainIntensity = GameObject.Find("GrainSlider").GetComponent<Slider>().value;

        Color newColor;
        Color sampleColor;
        Material material;
        int count = 0;
        int width = nodes[nodes.Count-1].getColumn();
        int height = nodes[nodes.Count-1].getRow();

        Color lightestColor = findLightestPointColor() + findDarkestPointColor();
        Color darkestColor = findLightestPointColor() + findDarkestPointColor();
        if (negativity == 0)
        {
            lightestColor = findLightestPointColor();
            darkestColor = findDarkestPointColor();
            Debug.Log("Brightest Found: " + lightestColor.grayscale);
            Debug.Log("Darkest Found: " + darkestColor.grayscale);
            Debug.Log("Brightest/Darkest: " + Mathf.Lerp(lightestColor.grayscale, darkestColor.grayscale, 0.5f));
        }
        else if (negativity == 1)
        {
            lightestColor = findDarkestPointColor();
            darkestColor = findLightestPointColor();
            Debug.Log("Brightest Found: " + lightestColor.grayscale);
            Debug.Log("Darkest Found: " + darkestColor.grayscale);
            Debug.Log("Brightest/Darkest: " + Mathf.Lerp(lightestColor.grayscale, darkestColor.grayscale, 0.5f));
        }

        Color tranColor;
        Color hueColor;
        Color satColor;
        Color negColor;
        Color brightColor;
        Color highColor;
        Color shadColor;

        float red;
        float green;
        float blue;
        float alpha;

        for (int i = 0; i < points.Count; i++)
        {
            newColor = points[i].GetComponent<PixelNode>().getDefaultColor();
            material = points[i].GetComponent<Renderer>().material;

            //red = alpha = points[i].GetComponent<PixelNode>().getDefaultColor().r;
            //green = points[i].GetComponent<PixelNode>().getDefaultColor().g;
            //blue = points[i].GetComponent<PixelNode>().getDefaultColor().b;
            //alpha = points[i].GetComponent<PixelNode>().getDefaultColor().a;

            // Reflectivity
            if (reflectivity > 0)
            {
                material.SetFloat("_Metallic", reflectivity/10f);
                material.SetFloat("_Glossiness", reflectivity/10f);
            }
            // Transparency
            if (transparency > 0)
            {
                float localAlpha = ((10f - transparency)/10f);
                tranColor = new Color(newColor.r, newColor.g, newColor.b, localAlpha);
                newColor = Color.Lerp(newColor, tranColor, 1.0f);
            }
            // Neon
            if (neon > 0)
            {
                //Debug.Log("Glow plz");
                material.EnableKeyword("_EMISSION");
                
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }
            // Hue
            if (hueity != 0)
            {
                float localHue = 0f;
                float localSaturation = 0f;
                float localBrightness = 0f;
                float magnitude = Mathf.Abs(hueity)/5f;
                hueColor = newColor;

                Color.RGBToHSV(newColor, out localHue, out localSaturation, out localBrightness);
                if (hueity < 0)
                {
                    hueColor = Color.HSVToRGB(localHue - (magnitude * localHue), localSaturation, localBrightness, false);
                }
                else if (hueity > 0)
                {
                    hueColor = Color.HSVToRGB(localHue + (magnitude * localHue), localSaturation, localBrightness, false);
                }

                newColor = Color.Lerp(newColor, hueColor, 1.0f);
            }
            // Saturation
            if (saturation != 0)
            {
                float localHue = 0f;
                float localSaturation = 0f;
                float localBrightness = 0f;
                float magnitude = Mathf.Abs(saturation)/5f;
                satColor = newColor;

                Color.RGBToHSV(newColor, out localHue, out localSaturation, out localBrightness);
                if (saturation < 0)
                {
                    satColor = Color.HSVToRGB(localHue, localSaturation - (magnitude * localSaturation), localBrightness, false);
                }
                else if (saturation > 0)
                {
                    satColor = Color.HSVToRGB(localHue, localSaturation + (magnitude * localSaturation), localBrightness, false);
                }

                newColor = Color.Lerp(newColor, satColor, 1.0f);
            }
            // Negativity
            if (negativity > 0)
            {
                float localRed = 1f - newColor.r;
                float localGreen = 1f - newColor.g;
                float localBlue = 1f - newColor.b;

                negColor = new Color(localRed, localGreen, localBlue, newColor.a);
                newColor = Color.Lerp(newColor, negColor, 1.0f);
            }
            // Brightness
            if (brightness != 0)
            {
                float localHue = 0f;
                float localSaturation = 0f;
                float localBrightness = 0f;
                float magnitude = Mathf.Abs(brightness)/5f;
                brightColor = newColor;

                Color.RGBToHSV(newColor, out localHue, out localSaturation, out localBrightness);
                if (brightness < 0)
                {
                    brightColor = Color.HSVToRGB(localHue, localSaturation, localBrightness - (magnitude * localBrightness), false);
                }
                else if (brightness > 0)
                {
                    brightColor = Color.HSVToRGB(localHue, localSaturation, localBrightness + (magnitude * localBrightness), false);
                }

                newColor = Color.Lerp(newColor, brightColor, 1.0f);
            }
            // Highlights
            if (highlights > 0)
            {
                float localHue = 0f;
                float localSaturation = 0f;
                float localBrightness = 0f;
                float magnitude = highlights/10f;

                Color.RGBToHSV(newColor, out localHue, out localSaturation, out localBrightness);
                if (newColor.grayscale > Mathf.Lerp(lightestColor.grayscale, darkestColor.grayscale, 0.5f))
                {
                    Debug.Log("Highlight Local Brightness: " + localBrightness);
                    highColor = Color.HSVToRGB(localHue, localSaturation, localBrightness - (magnitude * localBrightness), false);
                }
                else
                {
                    highColor = Color.HSVToRGB(localHue, localSaturation, localBrightness, false);
                }

                newColor = Color.Lerp(newColor, highColor, 1.0f);
            }
            // Shadows
            if (shadows != 0)
            {
                float localHue = 0f;
                float localSaturation = 0f;
                float localBrightness = 0f;
                float magnitude = shadows/10f;

                Color.RGBToHSV(newColor, out localHue, out localSaturation, out localBrightness);
                if (newColor.grayscale < Mathf.Lerp(lightestColor.grayscale, darkestColor.grayscale, 0.5f))
                {
                    Debug.Log("Shadow Local Brightness: " + localBrightness);
                    shadColor = Color.HSVToRGB(localHue, localSaturation, localBrightness + (magnitude * localBrightness), false);
                }
                else
                {
                    shadColor = Color.HSVToRGB(localHue, localSaturation, localBrightness, false);
                }

                newColor = Color.Lerp(newColor, shadColor, 1.0f);
            }
            // Sharpen
            if (sharpness > 0)
            {
            }
            // Blur
            if (blurIntensity > 0)
            {
            }
            // Grain
            if (grainIntensity > 0)
            {
            }

            //newColor = negColor;
            points[i].GetComponent<PixelNode>().setCurrentColor(newColor);
            material.color = newColor;

            if (neon > 0)
            {
                material.SetColor("_EmissionColor", newColor);
            }
        }
    }

    private void plotImage()
    {
        Debug.Log("[Plotting new image...]");

        GameObject meshGroup;
        GameObject plot = GameObject.Find("ImagePlot");
        float scale = 1f;

        combine = new CombineInstance[1000];
        int meshCount = 0;

        for (int i = 0; i < nodes.Count; i++)
        {
            //Debug.Log("Node " + i);

            //var pixelPt = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var pixelPt = Instantiate(GameObject.Find("ExampleCube"));
            pixelPt.name = "PixelPoint" + i;
            pixelPt.AddComponent<PixelNode>();
            points.Add(pixelPt);
            PixelNode pixNode = pixelPt.GetComponent<PixelNode>();
            pixNode.setId(nodes[i].getId());
            pixNode.setDefaultColor(nodes[i].getDefaultColor(), true);
            pixNode.setRow(nodes[i].getRow());
            pixNode.setColumn(nodes[i].getColumn());
            pixNode.setDepthMagnitude((int)GameObject.Find("DepthSlider").GetComponent<Slider>().value);

            float xPos = pixNode.getColumn();
            float yPos = pixNode.getRow();
            float zPos = pixNode.getDepth() * -1;
            //Debug.Log("Plot " + i + " xPos " + xPos + " yPos " + yPos + " zPos " + zPos);
            //Debug.Log("Node " + i + " Depth: " + pixNode.getDepth());
            //Debug.Log("Node " + i + " Depth Magnitude: " + pixNode.getDepthMagnitude());
            //Debug.Log("Node " + i + " Grayscale: " + pixNode.getDefaultColor().grayscale);

            pixelPt.transform.position = new Vector3(xPos, yPos, zPos);
            pixelPt.transform.rotation = Quaternion.identity;
            pixelPt.transform.localScale = new Vector3(scale, scale, scale);

            //Material newMaterial = new Material(Shader.Find("VertexLit"));
            Material newMaterial = new Material(Shader.Find("Standard"));
            BlendMode blendMode = StandardShaderUtils.BlendMode.Transparent;
            StandardShaderUtils.ChangeRenderMode(newMaterial, blendMode);
            newMaterial.color = pixNode.getCurrentColor();
            pixelPt.GetComponent<Renderer>().material = newMaterial;
            pixelPt.GetComponent<BoxCollider>().enabled = false;
            pixelPt.GetComponent<Renderer>().receiveShadows = false;
            pixelPt.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //pixelPt.GetComponent<Renderer>().staticShadowCaster = true;
            pixelPt.gameObject.SetActive(true);

            pixelPt.transform.parent = plot.transform;

            combine[meshCount].mesh = pixelPt.GetComponent<MeshFilter>().mesh;
            combine[meshCount].transform = pixelPt.transform.localToWorldMatrix;
            meshCount++;

            if (meshCount == 1000)
            {
                Debug.Log("[Saving meshes...]");

                combines.Add(combine);

                combine = new CombineInstance[1000];
                meshCount = 0;
            }
        }

        Debug.Log("[Batching points...]");
        StaticBatchingUtility.Combine((GameObject[]) points.ToArray(), plot);

        combines.Add(combine);

        //Debug.Log("[Batching meshes...]");
        for (int i = 0; i < combines.Count; i++)
        {
            GameObject meshGroupi = new GameObject();
            meshGroupi.name = "MeshGroup" + meshGroups.Count;
            meshGroupi.transform.parent = plot.transform;
            meshGroupi.AddComponent<MeshFilter>();
            meshGroupi.AddComponent<MeshRenderer>();

            //meshGroupi.GetComponent<MeshFilter>().mesh = new Mesh();
            //meshGroupi.GetComponent<MeshFilter>().mesh.CombineMeshes(combines[combines.Count-1]);
            meshGroups.Add(meshGroupi);
        }

        combine = new CombineInstance[1000];
        meshCount = 0;

        Debug.Log("Number of Image Points Made: " + points.Count);
    }
}
