using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class UIHelpers{

        private static Texture2D boxTexture;
        private static Texture2D BoxTexture{
            get{
                if( boxTexture == null ){
                    boxTexture = new Texture2D( 1, 1 );
                    boxTexture.SetPixel( 0, 0, Color.white );
                    boxTexture.Apply();
                }
                return boxTexture;
            }
        }

        public static Bounds GetViewportBounds(Camera camera, Vector3 screenPositionition1, Vector3 screenPositionition2 ){
            Vector3 v1 = Camera.main.ScreenToViewportPoint( screenPositionition1 );
            Vector3 v2 = Camera.main.ScreenToViewportPoint( screenPositionition2 );
            Vector3 min = Vector3.Min( v1, v2 );
            Vector3 max = Vector3.Max( v1, v2 );
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public static Rect GetScreenRect(Vector3 pos1, Vector3 pos2){
            Vector3 screenPosition1 = GetScreenRectPosition(pos1);
            Vector3 screenPosition2 = GetScreenRectPosition(pos2);

            Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
            Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);

            return Rect.MinMaxRect(topLeft.x,topLeft.y,bottomRight.x, bottomRight.y);
        }

        public static Vector3 GetScreenRectPosition(Vector3 pos){
            Vector3 screenPosition = new Vector3(pos.x,pos.y,pos.z);
            screenPosition.y = Screen.height - screenPosition.y;
            return screenPosition;
        }

        public static void DrawScreenRect(Rect rect, Color color ){
            GUI.color = color;
            GUI.DrawTexture( rect, BoxTexture );
            GUI.color = Color.white;
        }

        public static void DrawScreenRectBorder( Rect rect, float thickness, Color color ){
            // Top
            DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
            // Left
            DrawScreenRect( new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
            // Right
            DrawScreenRect( new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color);
            // Bottom
            DrawScreenRect( new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
        }
    
    }
}