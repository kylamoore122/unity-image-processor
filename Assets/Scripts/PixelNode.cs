using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelNode : MonoBehaviour
{
    private int id;
    private Color defaultColor;
    private Color currentColor;
    private int row;
    private int column;
    private int depthMagnitude;
    private float depth;

    public PixelNode(int id, Color color, int row, int col)
    {
        this.id = id;
        this.defaultColor = color;
        this.currentColor = color;
        this.row = row;
        this.column = col;
        calculateDepth();
    }

    private void calculateDepth()
    {
        int baseDepth = 0;
        this.depth = (float)(baseDepth + (this.currentColor.grayscale * depthMagnitude));
        //Debug.Log("Node " + this.id + " Calc'ed Depth: " + (baseDepth + (this.currentColor.grayscale * depthMagnitude)));
    }

    public void setId(int id)
    {
        this.id = id;
    }

    public void setDefaultColor(Color color, bool overrideCurrent)
    {
        this.defaultColor = color;
        
        if (overrideCurrent)
        {
            this.currentColor = color;
        }

        calculateDepth();
    }

    public void setCurrentColor(Color color)
    {
        this.currentColor = color;
        calculateDepth();
    }

    public void setRow(int row)
    {
        this.row = row;
    }

    public void setColumn(int col)
    {
        this.column = col;
    }

    public void setDepthMagnitude(int dM)
    {
        this.depthMagnitude = dM;
        calculateDepth();
    }

    public int getId()
    {
        return this.id;
    }

    public Color getDefaultColor()
    {
        return this.defaultColor;
    }

    public Color getCurrentColor()
    {
        return this.currentColor;
    }

    public int getRow()
    {
        return this.row;
    }

    public int getColumn()
    {
        return this.column;
    }

    public float getDepth()
    {
        return this.depth;
    }

    public int getDepthMagnitude()
    {
        return this.depthMagnitude;
    }
}
