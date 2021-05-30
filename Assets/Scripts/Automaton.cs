using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Automaton : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 128;
    [SerializeField] private RawImage debugImage;
    [SerializeField] private int colorNum = 20;

    private int kernelIdNextState;
    private int kernelIdDrawTexture;
    private ComputeBuffer stateBuffer;
    private ComputeBuffer nextStateBuffer;
    private ComputeBuffer colorBuffer;
    private uint[] stateArray1;
    private uint[] stateArray2;
    private RenderTexture targetTexture;
    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 15;

        kernelIdNextState = computeShader.FindKernel("CalcNextState");
        kernelIdDrawTexture = computeShader.FindKernel("DrawTexture");

        stateArray1 = new uint[width * height];
        stateArray2 = new uint[width * height];
        stateBuffer = new ComputeBuffer(width * height, sizeof(uint), ComputeBufferType.Raw);
        nextStateBuffer = new ComputeBuffer(width * height, sizeof(uint), ComputeBufferType.Raw);

        targetTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        targetTexture.enableRandomWrite = true;
        targetTexture.filterMode = FilterMode.Point;
        targetTexture.Create();
        debugImage.texture = targetTexture;

        for (var i = 0; i < 1; i++)
        {
            var idx = width * Random.Range(0, height - 1) + Random.Range(0, width - 1);
            stateArray1[idx] = 1;
        }

        var colorArray = new float[3 * colorNum];
        for (var i = 0; i < colorNum; i++)
        {
            var color = Color.HSVToRGB((float)i / (float)colorNum, (float)i / (float)colorNum, 1f);
            colorArray[3 * i + 0] = color.r;
            colorArray[3 * i + 1] = color.g;
            colorArray[3 * i + 2] = color.b;
        }
        colorBuffer = new ComputeBuffer(3 * colorNum, sizeof(float), ComputeBufferType.Raw);
        colorBuffer.SetData(colorArray);

        computeShader.SetInt("_Width", width);
        computeShader.SetInt("_Height", height);
        computeShader.SetBuffer(kernelIdNextState, "_States", stateBuffer);
        computeShader.SetBuffer(kernelIdNextState, "_NextStates", nextStateBuffer);
        computeShader.SetTexture(kernelIdDrawTexture, "_Texture", targetTexture);
        computeShader.SetBuffer(kernelIdDrawTexture, "_States", nextStateBuffer);
        computeShader.SetBuffer(kernelIdDrawTexture, "_Colors", colorBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        if (frame % 2 == 0)
        {
            stateBuffer.SetData(stateArray1);
            computeShader.Dispatch(kernelIdNextState, width / 8, height / 8, 1);
            nextStateBuffer.GetData(stateArray2);
            // Debug.Log(string.Join(",", stateArray2));
        }
        else
        {
            stateBuffer.SetData(stateArray2);
            computeShader.Dispatch(kernelIdNextState, width / 8, height / 8, 1);
            nextStateBuffer.GetData(stateArray1);
            // Debug.Log(string.Join(",", stateArray1));
        }
        computeShader.Dispatch(kernelIdDrawTexture, width / 8, height / 8, 1);

        frame++;
    }

    void OnDestroy()
    {
        if (stateBuffer != null) stateBuffer.Dispose();
        if (nextStateBuffer != null) nextStateBuffer.Dispose();
    }
}
