using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ref
{/*Level of details:
skipping through vertain vertices (going by 1, 2, 4 points)

only 1,2,4,8

((w-1)/i)+1
number of vertices per line = width -1 over i, +1

v=width * height
v= width^2
Limit: 255^2 (16-bit index buffer) https://developer.arm.com/documentation/102448/0100/Triangle-and-polygon-usage#:~:text=In%20Unity%2C%20the%20format%20of,up%20to%204%20billion%20vertices
width <= 255
width = 241
w-1 = 240
divisible by 2,4,6,8,10,12
  * 
  * number of terrain chanks arround viewer that will e
  * 
  * instead add mapdata with callback to queue
  * 
  * 
  * 
  * 
  * 
  * 
  * 
    
took me 2 full days to understand, as i never used Actions, delegate or Threads before: 

When starting the scene: 
EndlessTerrainGenerator is gonna UpdateVisibleChunk, instantiate new TerrainChunk, and finaaly requestTheGeneratedChunk "once it's done". 
so (for each new chunks)

mapGen.RequestMapData(OnMapDataReceived);

OnMapDataReceived is a method passed as an Action. 
it means that the method won't be called before the Action is called.
This Action encapsules a <MapData> as OnMapDataReceived takes a MapData as a parameter. 
I'll explain later why you need to pass it as an Action. Ignore it for now.

public void RequestMapData(Action<MapData> callBack){
 ThreadStart threadStart = delegate{
  MapDataThread (callBack);};
 
 new Thread (threadStart).Start ();
}
ThreadStart is a Type which define what the Thread is gonna do. 
It is required when you declare a new Thread.
Delegate means execute. 
You just can't write smtg like:
new Thread (MapDataThread (callBack)).Start(); 
That's just synthax. :) But that's what it means.

So a new thread exectutes the MapDataThread method. 
so even if it takes 12s to execute, the game continue to Update, the player can still move without waiting those 12s. Uselfull in multi ^^

public void MapDataThread(Action<MapData> callback){
 MapData mapData = GenerateMapData ();
 lock (mapDataThreadInfoQueue) {
  mapDataThreadInfoQueue.Enqueue (new MapThreadInfo<MapData> (callback, mapData));  
 }
}
As we're in another thread, EndlessTerrainGenerator continues to Update and request new chunks. 
I never used Queue before, but, that's easy to understand i guess. Enqueue() adds to the end of list(), Dequeue remove first of the List and returns its content.
But this queue is in the main Thread. and all threads can try to write it at the same time. So you could get inpredictibles and random results, like 2 mapData in the same index. Some kind of mutant map. Not what we expect ^^ so we lock the queue. Because it's in the main thread. Once you get it it becomes evident. 

So it's a list of mapThreadInfo
struct MapThreadInfo<T>{
 public readonly Action<T> callback;
 public readonly T parameter;
 public MapThreadInfo (Action<T> callback, T parameter)
 {
  this.callback = callback;
  this.parameter = parameter;
 } 
}
There are 2 types which will pass throught this struct: MapData, and MeshData. So the Type is generic. T could be string, float, or anything. 
parameter is the content of the type T. So here it will be MapData content. But it could be any string, any float or anything. 

So we Enqueue in the Main Thread queue one new Action of type Mapdata, which has just been generated and will be stored in the parameter. 

Remember, for now, Action<MapData> called Callback has not been executed. It's just stored. 

Next 
void Update(){
if (mapDataThreadInfoQueue.Count>0){
 for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
  MapThreadInfo<MapData> mapThreadInfo = mapDataThreadInfoQueue.Dequeue ();
  mapThreadInfo.callback (mapThreadInfo.parameter);
 }
}
First we store the content of the first task to do in the queue. 
but now we're in the main thread.
There we execute the Action. (stored as callback)
This action is to call " OnMapDataReceived " in the right instance of the TerrainChunk that we are generating. And OnMapDataReceived takes a parameter (MapData) stored in the parameter section of the mapThreadInfo. 

If you're still wondering, "why do we use Action instead of just calling the method?" remember that many frames have passed since the request has been sent, and many terrainChunk have been generated. 
How could we know which is the good terrainChunk right now waiting its MapData? There are probably other ways, but they all seems very complex. 

I hope it can help someone ;) 
  * 
  */
    // https://whitriley.com/portfolio/infinite-terrain-generation/
    // https://www.redblobgames.com/maps/terrain-from-noise/
    // https://www.youtube.com/watch?v=ja63QO1Imck
    // https://forum.unity.com/threads/after-playing-minecraft.63149/
}
