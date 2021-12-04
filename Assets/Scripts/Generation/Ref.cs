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
  * 
  * 
  * 
  */
    // https://whitriley.com/portfolio/infinite-terrain-generation/
    // https://www.redblobgames.com/maps/terrain-from-noise/
    // https://forum.unity.com/threads/after-playing-minecraft.63149/
}
