# CHGame

CHGame is a Unity convex hull game based on framework structure [Ruler](https://github.com/kbuchin/ruler).

This is the final project of Group 16 for 2IMA15 Geometric algorithms, developed by: @HSJMDMG @onlymmc @litianchao1996

## How to play 

Here is a demo video.

https://youtu.be/GqbcDBgz_dc

Or
https://www.bilibili.com/video/BV1yi4y1P7H2/

### 1-Player mode

You are a new landlord expanding your territory. You are permitted to choose at most m wastelands from $n (m <= n)$ to build your new castles. Every wasteland owns 1 unit food source. To encourage the expansion of the kingdom, you are allowed to own the whole area bounded by $m$ chosen wastelands, or in another word, the convex hull area of $m$ wastelands. As an ambitious landlord, you hope to :
 1. Get a biggest territory (maximum area)  Or
 2. Get maximum food source (maximum number of wasteland inside territory)
You will be ennobled as 1-star(60% optimal), 2-star(80% optimal) even 3-star(95% optimal) distinguished landlord based on your achievements in land expanding. You need to at least reach 1-star to join the next mission on another continent.



### 2-Player mode

You are an ambitious ruler of a kingdom, you  and your neighbor discovered  a lost land at the same time. After discussion  both of you  decided to take turns explore and build castles there. 
There are $m$ unowned wasteland to explore. In each turn, you can only take one of the following operations:
1. Build a long wall (segment) between 2 unowned or your wastelands.
2. Unite 2 owned region into a bigger region.
Once your wall create a closed polygon, you will automatically own the region covered by the convex hull of those wastelands. The union of two region is the convex hull of the wastelands within two original regions.

Here are some rules to follow when you exploring:
1. You are not allowed to build a wall intersecting other walls.
2. You are not allowed to build a wall from the inside or boundary of a region
3. You are not allowed to build duplicate walls between same wastelands
4. Regions can be united only once. People get confused when too many changes happens.
5. United Regions cannot be merged later.
6. Regions that overlap with other regions could not be merged.

Remember, your goal is to expanding your territory, try to get a maximum region area in this competition.


