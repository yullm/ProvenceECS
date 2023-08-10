using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIElementsExtensions{

    public static void Add(this VisualElement ve, params VisualElement[] elements){
        for(int i = 0; i < elements.Length; i++){
            ve.Add(elements[i]);
        }
    }

    public static T AddToClassList<T>(this T ve, params string[] classes) where T : VisualElement{
        for(int i = 0; i < classes.Length; i++){
            ve.AddToClassList(classes[i]);
        }
        return ve;
    }

    public static void RemoveFromClassList(this VisualElement ve, params string[] classes){
        for(int i = 0; i < classes.Length; i++){
            ve.RemoveFromClassList(classes[i]);
        }
    }

    public static void Toggle(this VisualElement ve){
        if(ve.style.display.value == DisplayStyle.Flex) ve.style.display = DisplayStyle.None;
        else ve.style.display = DisplayStyle.Flex;
    }

    public static Rect AbsoluteRect(this VisualElement ve){
        float x = ve.style.left.value.value;
        float y = ve.style.top.value.value;

        Rect rect = new Rect(x,y,ve.resolvedStyle.width,ve.resolvedStyle.height);
        return rect;
    }

    public static Vector2 PositionToVector2(this VisualElement ve){
        return new Vector2(ve.style.left.value.value,ve.style.top.value.value);
    }

    public static void SetPosition(this VisualElement ve, Vector2 position){
        ve.style.left = position.x;
        ve.style.top = position.y;
    }
   
}
