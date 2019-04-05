using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingController : MonoBehaviour
{
    [SerializeField]
    ComputeShader previewShader;
    [SerializeField]
    int selectBrushIndex = 0;
    [SerializeField]
    [Range(1, 100)]
    float selectBrushSize = 0.5f;
    [SerializeField]
    Texture[] brushTexture;
    [SerializeField]
    Texture2D selectedBrush;
    [SerializeField]
    Color brushColor;

    GameObject modelParent;
    GameObject modelPivot;
    GameObject model;
    Material modelMaterial;
    GameObject brush;
    Texture2D orignalTexture;
    RenderTexture paintTexture;
    int textureWidth;
    int textureHeight;

    UIControler uiControler;

    public int SelectBrushIndex
    {
        get { return selectBrushIndex; }
        set
        {
            selectBrushIndex = value;
            setBrush();
        }
    }

    public float SelectBrushSize
    {
        get { return selectBrushSize; }
        set
        {
            selectBrushSize = value;
            setBrush();
        }
    }

    public Color BrushColor
    {
        get { return brushColor; }
        set
        {
            brushColor = value;
        }
    }

    void Start()
    {
        modelParent = GameObject.FindGameObjectWithTag("modelParent");
        modelPivot = modelParent.transform.Find("Pivot").gameObject;
        if (modelParent.transform.childCount > 0)
        {
            model = modelPivot.transform.GetChild(0).gameObject;
            modelMaterial = model.GetComponent<MeshRenderer>().material;
            textureWidth = modelMaterial.mainTexture.width;
            textureHeight = modelMaterial.mainTexture.height;
            orignalTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
            orignalTexture.SetPixels(((Texture2D)modelMaterial.mainTexture).GetPixels());
            orignalTexture.Apply();
            paintTexture = new RenderTexture(textureWidth, textureHeight, 24);
            paintTexture.enableRandomWrite = true;
            paintTexture.Create();
        }
        else
            model = null;

        selectedBrush = new Texture2D(brushTexture[0].width, brushTexture[0].height) ;
        InitBrush();
        setBrush();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 100))
        {
            showOnTexture(hit.textureCoord);
        }
        else
        {
            showOnTexture(new Vector2(-1, -1));
        }
        //if (Input.GetAxis("Mouse ScrollWheel") != 0) // forward
        //{
        //    if (selectBrushSize > 0 && selectBrushSize <= 1)
        //    {
        //        selectBrushSize += Input.GetAxis("Mouse ScrollWheel") * 0.1f;
        //        setBrush();
        //    }
        //    else if (selectBrushSize <= 0) selectBrushSize = 0.1f;
        //    else if (selectBrushSize > 1) selectBrushSize = 1;
        //}
    }
    
    void showOnTexture(Vector2 textureCord)
    {
        if (model == null)
            return;
        int kernelHandle = previewShader.FindKernel("CSMain");
        previewShader.SetTexture(kernelHandle, "Result", paintTexture);
        previewShader.SetTexture(kernelHandle, "Orignal", orignalTexture);
        previewShader.SetTexture(kernelHandle, "Brush", GetSelectedBrush());
        previewShader.SetVector("cursor", new Vector4(textureCord.x, textureCord.y, textureWidth, textureHeight));
        previewShader.SetVector("color", new Vector4(brushColor.r, brushColor.g, brushColor.b, brushColor.a));
        previewShader.Dispatch(kernelHandle, textureWidth / 8, textureHeight / 8, 1);
        modelMaterial.mainTexture = paintTexture;

        if (Input.GetMouseButton(0))
            toTexture2D(paintTexture);
    }
    void setBrush()
    {
        if (selectBrushIndex >= brushTexture.Length)
            selectBrushIndex = selectBrushIndex % brushTexture.Length;
        if (selectBrushIndex < 0)
            selectBrushIndex = 0;
        
        selectedBrush.SetPixels(((Texture2D)brushTexture[selectBrushIndex]).GetPixels());
        selectedBrush = new Texture2D(brushTexture[0].width, brushTexture[0].height);
        int w = (int)(brushTexture[selectBrushIndex].width * selectBrushSize);
        int h = (int)(brushTexture[selectBrushIndex].height * selectBrushSize);
        selectedBrush = TextureScaler.scaled((Texture2D)brushTexture[selectBrushIndex], w, h);
        TextureScaler.scale(selectedBrush, w, h);
    }
    void InitBrush()
    {
        uiControler = GameObject.FindObjectOfType<UIControler>();
        for (int i = 0; i < brushTexture.Length; i++)
        {
            uiControler.addBrush((Texture2D)brushTexture[i],i);
        }
    }
    Texture GetSelectedBrush()
    {
        
        return selectedBrush;
    }

    void toTexture2D(RenderTexture rTex)
    {
        RenderTexture.active = rTex;
        orignalTexture.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        orignalTexture.Apply();
    }
}
