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
            Vector3 screenPosition1 = new Vector3(pos1.x,pos1.y,pos1.z);
            screenPosition1.y = Screen.height - screenPosition1.y;
            Vector3 screenPosition2 = new Vector3(pos2.x,pos2.y,pos2.z);
            screenPosition2.y = Screen.height - screenPosition2.y;

            Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
            Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);

            return Rect.MinMaxRect(topLeft.x,topLeft.y,bottomRight.x, bottomRight.y);
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