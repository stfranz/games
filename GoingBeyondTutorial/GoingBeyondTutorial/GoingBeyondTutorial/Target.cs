using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GoingBeyondTutorial
{
    class Target : GameObject
    {
        public static int PointValue;
        public bool IsMoving { get; set; }
        public bool ToRemove { get; set; }
        public List<Vector3> Path { get; set; }
        public bool ReverseOnPathEnd { get; set; }
        public float Speed { get; set; }

        protected int direction = 1;
        protected int targetIndex;

        public Target() : base()
        {
            PointValue = 50;
            IsMoving = false;
            ToRemove = false;
            ReverseOnPathEnd = false;
            Path = null;
            Speed = 0;
        }

        protected void incrementTargetIndex()
        {
            targetIndex += direction;
            if (targetIndex >= Path.Count)
            {
                if (ReverseOnPathEnd)
                {
                    direction = -1;
                    targetIndex += direction + direction;
                }
                else
                {
                    targetIndex = 0;
                }
            }
            else if (targetIndex < 0)
            {
                targetIndex = 1;
                direction = 1;
            }
        }

        public void Update(GameTime time)
        {
            if (IsMoving)
            {
                Vector3 target = Path[targetIndex];
                float moveDistance = Speed * (float)time.ElapsedGameTime.TotalSeconds;
                if (moveDistance == 0)
                {
                    return;
                }
                while ((target - PositionInWorldSpace).LengthSquared() < moveDistance * moveDistance)
                {
                    PositionInWorldSpace = target;
                    incrementTargetIndex();
                    target = Path[targetIndex];
                }
                Vector3 moveVector = (target - PositionInWorldSpace);
                moveVector.Normalize();
                moveVector = moveVector * moveDistance;
                PositionInWorldSpace = PositionInWorldSpace + moveVector;

            }
        }
    }
}

