﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIElementsExtensions{

    public static void Add(this VisualElement ve, params VisualElement[] elements){
        for(int i = 0; i < elements.Length; i++){
            ve.Add(elements[i]);
        }
    }

    public static void AddToClassList(this VisualElement ve, params string[] classes){
        for(int i = 0; i < classes.Length; i++){
            ve.AddToClassList(classes[i]);
        }
    }

    public static void Toggle(this VisualElement ve){
        if(ve.style.display.value == DisplayStyle.Flex) ve.style.display = DisplayStyle.None;
        else ve.style.display = DisplayStyle.Flex;
    }
   
}
