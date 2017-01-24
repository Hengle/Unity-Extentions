using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
/*
Vector Range Attribute by Just a Pixel (Danny Goodayle @DGoodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
USAGE
[VectorRange(minX, maxX, minY, maxY, clamped)]
public Vector2 yourVector;
*/

/*
Modified by ZenRumiko
Usage VectorRange[minX, maxX, minY, maxY, minZ, maxZ, clamped] 
or
Usage VectorRange[minX, maxX, minY, maxY, clamped]
I removed a bunch of the Hungarian notation while I was refactoring a bunch of stuff. (I hope thats ok)
Dont put this script in an Editor folder or you wont be able to use the [Attribute] in non editor scripts.
The editor only parts are enclosed in #if UNITY_EDITOR so you will still be able to build your project.
    Am I doing this comment thing right? No idea.
    Originally made by: Just a Pixel (Danny Goodayle @DGoodayle) as stated above.
*/

public class VectorRangeAttribute : PropertyAttribute
{
    public readonly float minX = float.MinValue,
                          maxX = float.MaxValue,
                          minY = float.MinValue,
                          maxY = float.MaxValue,
                          minZ = float.MinValue,
                          maxZ = float.MaxValue;
    public readonly bool clamp = true;

    public VectorRangeAttribute(float minX, float maxX, float minY, float maxY, bool clamp)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        this.clamp = clamp;
    }

    public VectorRangeAttribute(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, bool clamp)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        this.minZ = minZ;
        this.maxZ = maxZ;
        this.clamp = clamp;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(VectorRangeAttribute))]
public class Vector2RangeAttributeDrawer : PropertyDrawer
{
    VectorRangeAttribute rangeAttribute { get { return (VectorRangeAttribute)attribute; } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!(property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector3))
            base.OnGUI(position, property, label);

        bool isVector3 = property.propertyType == SerializedPropertyType.Vector3;
        Rect textFieldPosition = position;

        if (isVector3)
            HandleVector3(property, textFieldPosition, label);
        else
            HandleVector2(property, textFieldPosition, label);
    }

    public void HandleVector3(SerializedProperty property, Rect position, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        Vector3 vector = property.vector3Value;
        if (rangeAttribute.clamp)
            vector = ClampVector(vector);

        vector = EditorGUI.Vector3Field(position, label, vector);
        if (EditorGUI.EndChangeCheck())
        {
            if (rangeAttribute.clamp)
                vector = ClampVector(vector);

            property.vector3Value = vector;
        }

        HandleHelpBox(position, vector, true);
    }

    public void HandleVector2(SerializedProperty property, Rect position, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        Vector2 vector = property.vector2Value;
        if (rangeAttribute.clamp)
            vector = ClampVector(vector);

        vector = EditorGUI.Vector2Field(position, label, vector);
        if (EditorGUI.EndChangeCheck())
        {
            if (rangeAttribute.clamp)
                vector = ClampVector(vector);

            property.vector2Value = vector;
        }

        HandleHelpBox(position, vector, false);
    }

    public void HandleHelpBox(Rect position, Vector3 vector, bool isVector3)
    {
        Rect helpPosition = position;
        helpPosition.y += 16;
        helpPosition.height = 16;

        bool isValid = !rangeAttribute.clamp && IsValid(vector);
        if (!isValid)
        {
            Color cachedGUIColor = GUI.color;
            GUI.color = Color.red;
            DrawHelpBox(helpPosition, vector, isVector3);
            GUI.color = cachedGUIColor;
        }
    }

    Vector3 ClampVector(Vector3 vector)
    {
        vector.x = Mathf.Clamp(vector.x, rangeAttribute.minX, rangeAttribute.maxX);
        vector.y = Mathf.Clamp(vector.y, rangeAttribute.minY, rangeAttribute.maxY);
        vector.z = Mathf.Clamp(vector.z, rangeAttribute.minZ, rangeAttribute.maxZ);
        return vector;
    }

    bool IsValid(Vector3 vector)
    {
        return vector.x >= rangeAttribute.minX && vector.x <= rangeAttribute.maxX &&
               vector.y >= rangeAttribute.minY && vector.y <= rangeAttribute.maxY &&
               vector.z >= rangeAttribute.minZ && vector.z <= rangeAttribute.maxZ;
    }

    void DrawHelpBox(Rect position, Vector3 val, bool isVector3)
    {
        string message = isVector3 ? "Invalid Range X:[{0} - {1}] Y:[{2} - {3}] Z:[{4} - {5}]" : "Invalid Range X:[{0} - {1}] Y:[{2} - {3}]";
        EditorGUI.HelpBox(position, string.Format(message, rangeAttribute.minX, rangeAttribute.maxX, rangeAttribute.minY, rangeAttribute.maxY, rangeAttribute.minZ, rangeAttribute.maxZ), MessageType.Error);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        bool isVector3 = property.propertyType == SerializedPropertyType.Vector3;
        Vector3 vector = isVector3 ? property.vector3Value : (Vector3)property.vector2Value;
        if (!rangeAttribute.clamp && !IsValid(vector))
            return 32;
        return base.GetPropertyHeight(property, label);
    }
}

#endif
