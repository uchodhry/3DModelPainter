using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
    public GameObject brushPrefab;
    public GameObject breshsParent;
    public GameObject colorButton;

    PaintingController paintingController;
    ColorPickerTriangle colorPicker;
    bool colorPickerState = false;

    private void Awake()
    {
        paintingController = FindObjectOfType<PaintingController>();
        colorPicker = FindObjectOfType<ColorPickerTriangle>();
    }
    public void addBrush(Texture2D brushTexxture, int brushIndex)
    {
        if (brushPrefab != null)
        {
            GameObject tempBrush =  GameObject.Instantiate(brushPrefab, breshsParent.transform);
            tempBrush.GetComponent<Button>().onClick.AddListener(() => SelectBrush(brushIndex));
            tempBrush.GetComponent<Image>().sprite = Sprite.Create(brushTexxture,new Rect(0,0, brushTexxture.width, brushTexxture.height),new Vector2(.5f,.5f));
        }
    }
    public void SelectBrush(int index)
    {
        paintingController.SelectBrushIndex = index;
    }
    public void BrushSizeChanged(Slider sizeSlider)
    {
        paintingController.SelectBrushSize = sizeSlider.value;
    }
    public void toggleColorPicker()
    {
        colorPicker.gameObject.SetActive(colorPickerState);
        colorPickerState = !colorPickerState;

    }
    public void colorChanged(Color newColor)
    {
        colorButton.GetComponent<Image>().color = newColor;
        paintingController.BrushColor = newColor;
    }
}
