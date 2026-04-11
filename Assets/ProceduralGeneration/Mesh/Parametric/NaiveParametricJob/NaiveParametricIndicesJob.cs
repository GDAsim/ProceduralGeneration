using Unity.Collections;
using Unity.Jobs;

/// <summary>
/// Attempted Convertsion of Naive Parametric
/// </summary>
struct NaiveParametricIndicesJob : IJobFor
{
    //Read
    [ReadOnly] public bool isusingU;
    [ReadOnly] public bool isusingV;
    [ReadOnly] public bool isusingW;

    [ReadOnly] public int sampleresolution_U;
    [ReadOnly] public int sampleresolution_V;
    [ReadOnly] public int sampleresolution_W;

    [ReadOnly] public int numOfDimentions;

    //Write
    public NativeList<int> outputIndices;

    public void Execute(int index)
    {
        int k = index / (sampleresolution_U * sampleresolution_V);
        int j = index % (sampleresolution_U * sampleresolution_V) / sampleresolution_V;
        int i = index % sampleresolution_U;

        #region 3 Variables Used: Algo to create a 8 vert box
        //Drawing Clockwise
        if (numOfDimentions == 3)
        {
            if (k == 0 && i >= 1 && j >= 1)//front
            {
                //topright botright botleft topleft
                outputIndices.Add(index);
                outputIndices.Add(index - sampleresolution_U);
                outputIndices.Add(index - 1 - sampleresolution_U);
                outputIndices.Add(index - 1);
            }
            if (k == sampleresolution_W - 1 && i >= 1 && j >= 1)//back
            {
                //topright botright botleft topleft
                outputIndices.Add(index - 1);
                outputIndices.Add(index - 1 - sampleresolution_U);
                outputIndices.Add(index - sampleresolution_U);
                outputIndices.Add(index);
            }
            if (k >= 1 && i == 0 && j >= 1) //left
            {
                //topleft topright botright botleft
                outputIndices.Add(index);
                outputIndices.Add(index - sampleresolution_U * sampleresolution_V);
                outputIndices.Add(index - sampleresolution_U - sampleresolution_U * sampleresolution_V);
                outputIndices.Add(index - sampleresolution_U);
            }
            if (k >= 1 && i == sampleresolution_U - 1 && j >= 1) //right
            {
                outputIndices.Add(index - sampleresolution_U);
                outputIndices.Add(index - sampleresolution_U - sampleresolution_U * sampleresolution_V);
                outputIndices.Add(index - sampleresolution_U * sampleresolution_V);
                outputIndices.Add(index);
            }
            if (k >= 1 && j == 0 && i >= 1) //bot
            {
                outputIndices.Add(index);
                outputIndices.Add(index - 1);
                outputIndices.Add(index - 1 - sampleresolution_U * sampleresolution_V);
                outputIndices.Add(index - sampleresolution_U * sampleresolution_V);
            }
            if (k >= 1 && j == sampleresolution_V - 1 && i >= 1) //top
            {
                outputIndices.Add(index - sampleresolution_U * sampleresolution_V);
                outputIndices.Add(index - 1 - sampleresolution_U * sampleresolution_V);
                outputIndices.Add(index - 1);
                outputIndices.Add(index);
            }
        }
        #endregion

        #region 2 Variables Used
        //In order to allow for any parmetric u v w to be used in any order,
        //we need to know which is used/not used for generating curves
        //so that we can grab the correct loop index to do the triangle indexing
        else if (numOfDimentions == 2)
        {
            //i=u,j=v;k=w;
            int loopindex1 = i;
            int loopindex2 = j;
            int sampleres = sampleresolution_U;
            if (isusingU)
            {
                loopindex1 = i;
                sampleres = sampleresolution_U;
                if (isusingV)
                {
                    loopindex2 = j;
                }
                else if (isusingW)
                {
                    loopindex2 = k;
                }
            }
            else
            {
                sampleres = sampleresolution_V;
                loopindex1 = k;
                loopindex2 = j;
            }
            if (loopindex1 >= 1 && loopindex2 >= 1)
            {
                outputIndices.Add(index - 1);
                outputIndices.Add(index);
                outputIndices.Add(index - sampleres);
                outputIndices.Add(index - 1 - sampleres);
            }
        }
        #endregion

        #region 1 Variables Used
        else if (numOfDimentions == 1)
        {
            outputIndices.Add(index);
        }
        #endregion
    }
}