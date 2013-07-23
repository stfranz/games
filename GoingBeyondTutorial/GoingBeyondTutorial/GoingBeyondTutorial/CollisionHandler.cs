using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GoingBeyondTutorial
{
    class CollisionHandler
    {
        private BoundingBox[] buildings;
        private BoundingBox worldBoundary;
        private List<Target> targets;
        private List<Target> targetsToRemove;
        private BoundingBox lastBuildingCollision;

        public CollisionHandler(ref BoundingBox[] buildingBoxes, 
                                ref BoundingBox cityBox, 
                                ref List<Target> targts,
                                ref List<Target> targToRemove)
        {
            buildings = buildingBoxes;
            worldBoundary = cityBox;
            targets = targts;
            targetsToRemove = targToRemove;
        }

        public World.CollisionType CheckCollisions(Player plyr)
        {
            BoundingSphere sphere = plyr.Sphere;
            // maybe have delegate trigger some kind of animation for visual confirmation of point increase?
            for (int i = 0; i < targets.Count; i++)
            {
                Target temp = targets[i];
                if (temp.Sphere.Contains(sphere) != ContainmentType.Disjoint)
                {
                    targetsToRemove.Add(temp);
                    
                    return World.CollisionType.Target;
                }
            }
            for (int i = 0; i < buildings.Length; i++)
            {
                if (buildings[i].Contains(sphere) != ContainmentType.Disjoint)
                {
                    // ignore collision if colliding with same building as last collision
                    if (buildings[i] == lastBuildingCollision)
                    {
                        return World.CollisionType.None;
                    }
                    else
                    {
                        lastBuildingCollision = buildings[i];
                        return World.CollisionType.Building;
                    }
                }
            }
            if (worldBoundary.Contains(sphere) == ContainmentType.Disjoint)
                return World.CollisionType.Boundary;

            return World.CollisionType.None;
        }
        public World.CollisionType CheckCollisions(BoundingSphere sphere)
        {
            
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Sphere.Contains(sphere) != ContainmentType.Disjoint)
                {
                    return World.CollisionType.Target;
                }
            }
            for (int i = 0; i < buildings.Length; i++)
            {
                if (buildings[i].Contains(sphere) != ContainmentType.Disjoint)
                {
                    return World.CollisionType.Building;
                }
            }
            if (worldBoundary.Contains(sphere) == ContainmentType.Disjoint)
                return World.CollisionType.Boundary;

            return World.CollisionType.None;
        }
    }
}
