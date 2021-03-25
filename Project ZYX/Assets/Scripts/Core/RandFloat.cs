using UnityEngine;


[System.Serializable]
public class RandFloat
{
    //Peekaboo
    public enum Type { Single, Double, Curve }

    public Type type = Type.Single;
    public bool slider = false;
    public float single = 1f;
    public float min = 0f;
    public float max = 1f;
    public float minLimit = 0f;
    public float maxLimit = 1f;
    public AnimationCurve curve = new AnimationCurve();


    public RandFloat()
    {

    }
    public RandFloat(float value)
    {
        single = value;
        min    = value;
        max    = value;
    }
    public RandFloat(float value, float min, float max)
    {
        single = value;
        min    = value;
        max    = value;
        
        minLimit = min;
        maxLimit = max;
    }





    public float Get()
    {
        switch (type)
        {
            default: Debug.LogError("RandFloat type not supported!"); return 0f;
            case Type.Single: return single;
            case Type.Double: return Random.Range(min, max);
            case Type.Curve: 
                
                if (curve.length == 0)
                {
                    Debug.LogError("RandFloat curve has no keys!");
                    return Random.Range(min, max);
                }

                float start = curve.keys[0].time;
                float end   = curve.keys[curve.length-1].time;


            return curve.Evaluate(Random.Range(start, end));
        }
    }
}