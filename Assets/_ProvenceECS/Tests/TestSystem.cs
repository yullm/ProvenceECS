using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Ransacked;
using Ransacked.Mainframe;

namespace ProvenceECS{

    public class TestEvent : ProvenceEventArgs{
        public int index;

        public TestEvent(){
            this.index = 0;
        }

        public TestEvent(int index){
            this.index = index;
        }
    }

    public class TestSystem : ProvenceSystem {
 
        public Vector3 size = new Vector3(5,5,5);
        public Vector3 offset;

        protected Texture2D[] hitMaps;

        public Vector3 pos;
        public float radialDistance = 3;

                
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<WakeSystemEvent>(Awaken);    
            //world.eventManager.AddListener<WorldDrawGizmosUpdateEvent>(Tick);
            world.eventManager.RegisterReturnMethod<TestEvent,bool>(ReturnTest);     
        }

        protected bool ReturnTest(TestEvent args){
            return false;
        }

        public override void Awaken(WakeSystemEvent args){
            Debug.Log(world.eventManager.RaiseReturn<TestEvent,bool>(new TestEvent()));
        }

        // protected void InitTexture(out Texture2D[] textures){
        //     textures = new Texture2D[Mathf.FloorToInt(size.y)];
        //     for(int y = 0; y < size.y; y++){
        //         textures[y] = new Texture2D(Mathf.FloorToInt(size.x),Mathf.FloorToInt(size.z),TextureFormat.Alpha8,0,false);
        //         for(int texX = 0; texX < textures[y].width; texX++){
        //         for(int texY = 0; texY < textures[y].height; texY++){
        //                 textures[y].SetPixel(texX,texY,new Color(0,0,0,0));
        //             }                
        //         } 
        //     }            
        // }

        // public override void Awaken(WakeSystemEvent args){
        //     InitTexture(out hitMaps);
        //     LayerMask mask = (1 << LayerMask.NameToLayer("Walkable")) | (1 << LayerMask.NameToLayer("Obstacle"));
        //     int floorX = Mathf.FloorToInt(offset.x);
        //     int floorY = Mathf.FloorToInt(offset.y);
        //     int floorZ = Mathf.FloorToInt(offset.z);
        //     for(int x = floorX; x < size.x + floorX; x++){
        //         for(int y = floorY; y < size.y + floorY; y++){
        //             for(int z = floorZ; z < size.z + floorZ; z++){
        //                 if(Physics.CheckBox(new Vector3(x + 0.5f, y + 1f, z + 0.5f), new Vector3(0.4f,0.4f,0.4f),Quaternion.identity,mask)){
        //                     int yPos = y - floorY;
        //                     int xPos = x - floorX;
        //                     int zPos = z - floorZ;
        //                     hitMaps[yPos].SetPixel(xPos, zPos, new Color(1, 1, 1, 1));                          
        //                 }
        //             }
        //         }
        //     }
        // }

        // protected void Tick(WorldDrawGizmosUpdateEvent args){

        //     Vector3 roundOffset = ProvenceMath.FloorVector3(offset);
        //     Vector3 roundSize = ProvenceMath.FloorVector3(size);
        //     Vector3 center = new Vector3(roundOffset.x  + (roundSize.x/2), roundOffset.y + (roundSize.y/2), roundOffset.z + (roundSize.z/2));
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawWireCube(center, roundSize);
        //     if(hitMaps == null) return;

        //     /* for(int x = 0; x < roundSize.x; x++){
        //         for(int y = 0; y < roundSize.y; y++){
        //             for(int z = 0; z < roundSize.z; z++){
        //                 if(hitMaps[y].GetPixel(x,z).a == 1)
        //                     Gizmos.DrawWireCube(new Vector3(x,y,z) + roundOffset + (Vector3.one * 0.5f), Vector3.one);
        //             }
        //         }
        //     } */

        //     Gizmos.color = new Color(1,1,1,0.1f);
        //     HashSet<Vector3> points = new HashSet<Vector3>();
        //     Vector3 start = ProvenceMath.FloorVector3(pos - roundOffset + (Vector3.up * 0.4f));

        //     for(int y = (int)start.y; y >= (start.y - radialDistance); y--){
        //         for(int x = (int)(start.x - radialDistance); x <= (start.x + radialDistance); x++){
        //             for(int z = (int)(start.z - radialDistance); z <= (int)(start.z + radialDistance); z++){

        //                 if((x > (int)(start.x - radialDistance) && x < (start.x + radialDistance)) &&
        //                     (z > (int)(start.z - radialDistance) && z < (start.z + radialDistance))) continue;

        //                 Vector3 end = new Vector3(x,y,z);
        //                 if(points.Contains(end)) continue;

        //                 int steps = Mathf.FloorToInt(ProvenceMath.DiagonalDistance(start, end));
        //                 for(int i = 0; i <= steps; i++){
        //                     float t = steps == 0 ? 0f : (float)i / steps;
        //                     Vector3 point = ProvenceMath.FloorVector3(Vector3.Lerp(start,end,t));

        //                     if(points.Contains(point)) continue;
        //                     if(!point.y.IsInRange(0, roundSize.y - 1) || !point.x.IsInRange(0, roundSize.x - 1) || !point.z.IsInRange(0, roundSize.z - 1)) continue;
        //                     if(hitMaps[(int)point.y].GetPixel((int)point.x, (int)point.z).a == 1) break;
        //                     if((point - start).magnitude > radialDistance) break;

        //                     points.Add(point);
        //                 }
        //             }
        //         }
        //     }

        //     foreach(Vector3 point in points){
        //         Gizmos.DrawWireCube(point + roundOffset + (Vector3.one * 0.5f), Vector3.one);
        //     }

        //}


        // protected void InitTexture(out Texture2D[] textures){
        //     textures = new Texture2D[Mathf.FloorToInt(size.y)];
        //     for(int y = 0; y < size.y; y++){
        //         textures[y] = new Texture2D(Mathf.FloorToInt(size.x),Mathf.FloorToInt(size.z),TextureFormat.Alpha8,0,false);
        //         for(int texX = 0; texX < textures[y].width; texX++){
        //         for(int texY = 0; texY < textures[y].height; texY++){
        //                 textures[y].SetPixel(texX,texY,new Color(0,0,0,0));
        //             }                
        //         } 
        //     }            
        // }

        // public override void Awaken(WakeSystemEvent args){
        //     InitTexture(out hitMaps);
        //     LayerMask mask = (1 << LayerMask.NameToLayer("Walkable")) | (1 << LayerMask.NameToLayer("Obstacle"));
        //     int floorX = Mathf.FloorToInt(offset.x);
        //     int floorY = Mathf.FloorToInt(offset.y);
        //     int floorZ = Mathf.FloorToInt(offset.z);
        //     for(int x = floorX; x < size.x + floorX; x++){
        //         for(int y = floorY; y < size.y + floorY; y++){
        //             for(int z = floorZ; z < size.z + floorZ; z++){
        //                 if(Physics.CheckBox(new Vector3(x + 0.5f, y + 1f, z + 0.5f), new Vector3(0.4f,0.4f,0.4f),Quaternion.identity,mask)){
        //                     int yPos = y - floorY;
        //                     int xPos = x - floorX;
        //                     int zPos = z - floorZ;
        //                     hitMaps[yPos].SetPixel(xPos, zPos, new Color(1, 1, 1, 1));                          
        //                 }
        //             }
        //         }
        //     }
        // }
        
        // protected void Tick(WorldDrawGizmosUpdateEvent args){
            
        //     Vector3 roundOffset = ProvenceMath.FloorVector3(offset);
        //     Vector3 roundSize = ProvenceMath.FloorVector3(size);
        //     Vector3 center = new Vector3(roundOffset.x  + (roundSize.x/2), roundOffset.y + (roundSize.y/2), roundOffset.z + (roundSize.z/2));
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawWireCube(center, roundSize);
        //     if(hitMaps == null) return;

        //     /* for(int x = 0; x < roundSize.x; x++){
        //         for(int y = 0; y < roundSize.y; y++){
        //             for(int z = 0; z < roundSize.z; z++){
        //                 if(hitMaps[y].GetPixel(x,z).a == 1)
        //                     Gizmos.DrawWireCube(new Vector3(x,y,z) + roundOffset + (Vector3.one * 0.5f), Vector3.one);
        //             }
        //         }
        //     } */

        //     Gizmos.color = new Color(1,1,1,0.1f);
        //     HashSet<Vector3> points = new HashSet<Vector3>();
        //     Vector3 start = ProvenceMath.FloorVector3(pos - roundOffset + (Vector3.up * 0.4f));

        //     for(int y = (int)start.y; y >= (start.y - radialDistance); y--){
        //         for(int x = (int)(start.x - radialDistance); x <= (start.x + radialDistance); x++){
        //             for(int z = (int)(start.z - radialDistance); z <= (int)(start.z + radialDistance); z++){

        //                 if((x > (int)(start.x - radialDistance) && x < (start.x + radialDistance)) &&
        //                     (z > (int)(start.z - radialDistance) && z < (start.z + radialDistance))) continue;

        //                 Vector3 end = new Vector3(x,y,z);
        //                 if(points.Contains(end)) continue;

        //                 int steps = Mathf.FloorToInt(ProvenceMath.DiagonalDistance(start, end));
        //                 for(int i = 0; i <= steps; i++){
        //                     float t = steps == 0 ? 0f : (float)i / steps;
        //                     Vector3 point = ProvenceMath.FloorVector3(Vector3.Lerp(start,end,t));

        //                     if(points.Contains(point)) continue;
        //                     if(!point.y.IsInRange(0, roundSize.y - 1) || !point.x.IsInRange(0, roundSize.x - 1) || !point.z.IsInRange(0, roundSize.z - 1)) continue;
        //                     if(hitMaps[(int)point.y].GetPixel((int)point.x, (int)point.z).a == 1) break;
        //                     if((point - start).magnitude > radialDistance) break;

        //                     points.Add(point);
        //                 }
        //             }
        //         }
        //     }

        //     foreach(Vector3 point in points){
        //         Gizmos.DrawWireCube(point + roundOffset + (Vector3.one * 0.5f), Vector3.one);
        //     }
            
        //}

        protected override void GatherCache(){}

    }

}