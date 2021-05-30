using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Automaton : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 128;

    private int kernelIdNextState;
    private ComputeBuffer stateBuffer;
    private ComputeBuffer nextStateBuffer;
    private uint[] stateArray1;
    private uint[] stateArray2;
    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        kernelIdNextState = computeShader.FindKernel("CalcNextState");
        stateArray1 = new uint[width * height];
        stateArray2 = new uint[width * height];
        stateBuffer = new ComputeBuffer(width * height, sizeof(uint), ComputeBufferType.Raw);
        nextStateBuffer = new ComputeBuffer(width * height, sizeof(uint), ComputeBufferType.Raw);
        stateArray1[(height / 2) * width + (width / 2)] = 1;

        computeShader.SetInt("_Width", width);
        computeShader.SetInt("_Height", height);
        computeShader.SetBuffer(kernelIdNextState, "_States", stateBuffer);
        computeShader.SetBuffer(kernelIdNextState, "_NextStates", nextStateBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        if (frame % 2 == 0)
        {
            stateBuffer.SetData(stateArray1);
            computeShader.Dispatch(kernelIdNextState, width / 8, height / 8, 1);
            nextStateBuffer.GetData(stateArray2);
            Debug.Log(string.Join(",", stateArray2));
        }
        else
        {
            stateBuffer.SetData(stateArray2);
            computeShader.Dispatch(kernelIdNextState, width / 8, height / 8, 1);
            nextStateBuffer.GetData(stateArray1);
            Debug.Log(string.Join(",", stateArray1));
        }
        frame++;
    }

    void OnDestroy()
    {
        if (stateBuffer != null) stateBuffer.Dispose();
        if (nextStateBuffer != null) nextStateBuffer.Dispose();
    }
}
