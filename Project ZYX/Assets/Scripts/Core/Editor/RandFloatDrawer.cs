using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RandFloat))]
public class RandFloatDrawer : PropertyDrawer
{
    //Peekaboo
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty type   = property.FindPropertyRelative("type");
        SerializedProperty slider = property.FindPropertyRelative("slider");
        SerializedProperty single = property.FindPropertyRelative("single");
        SerializedProperty min    = property.FindPropertyRelative("min");
        SerializedProperty max    = property.FindPropertyRelative("max");
        SerializedProperty curve  = property.FindPropertyRelative("curve");

        float minLimit = property.FindPropertyRelative("minLimit").floatValue;
        float maxLimit = property.FindPropertyRelative("maxLimit").floatValue;

        
        GUILayout.BeginHorizontal();
        GUILayout.Label(property.displayName);
        ///////////////////////

        switch(type.enumValueIndex)
        {
            case (int)RandFloat.Type.Single:   

                switch(slider.boolValue)
                {
                    case true:  single.floatValue = EditorGUILayout.Slider(single.floatValue, minLimit, maxLimit, GUILayout.Width(Screen.width * 0.58f - 80)); break;
                    case false: single.floatValue = EditorGUILayout.FloatField(single.floatValue, GUILayout.Width(Screen.width * 0.58f - 80)); break;
                }
                break;
            case (int)RandFloat.Type.Double: 

                switch(slider.boolValue)
                {
                    case true: 
                    float x = min.floatValue; 
                    float y = max.floatValue;

                    EditorGUILayout.MinMaxSlider(ref x, ref y, minLimit, maxLimit, GUILayout.Width(Screen.width * 0.58f - 80 - 52));
                    x = EditorGUILayout.FloatField(x, GUILayout.Width(24));
                    y = EditorGUILayout.FloatField(y, GUILayout.Width(24));

                    min.floatValue = x;
                    max.floatValue = y;

                    break;
                    case false: 

                    min.floatValue = EditorGUILayout.FloatField(min.floatValue, GUILayout.Width((Screen.width * 0.58f-82)/2f)); 
                    max.floatValue = EditorGUILayout.FloatField(max.floatValue, GUILayout.Width((Screen.width * 0.58f-82)/2f));    
                    
                    break;
                }
                
                break;
            case (int)RandFloat.Type.Curve: 
                curve.animationCurveValue = EditorGUILayout.CurveField(curve.animationCurveValue, GUILayout.Width(Screen.width * 0.58f-80));
                break;
            default: Debug.LogError($"RandFloat Type \"{type.enumNames[type.enumValueIndex]}\" not supported!"); break;
        }
        type.enumValueIndex = EditorGUILayout.Popup(type.enumValueIndex, type.enumDisplayNames, GUILayout.Width(65), GUILayout.ExpandWidth(false));
        slider.boolValue    = EditorGUILayout.Toggle(slider.boolValue, GUILayout.ExpandWidth(false), GUILayout.Width(15));
     
        ///////////////////////
        GUILayout.EndHorizontal();
    }



    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0f;
    }
}
