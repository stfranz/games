using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GoingBeyondTutorial
{
    class Player : GameObject
    {
        public HUD Display;
        public HudData hData;

        public int StartingHealth { get; private set; }
        public Vector3 StartingPosition { get; set; }
        public KinectManager Kinect { get; set; }
        public float MaxTilt { get; protected set; }
        public float Speed { get { return mSpeed; } set { mSpeed = value; scoreIncrement = value / 4; } }

        public bool OutOfBounds = false;

        private float mSpeed;
        private float scoreIncrement;

        public Player()
            : base()
        {
        }
        public Player(Model model)
            : base(model)
        {
        }
        public Player(KinectManager kinectMan)
            : base()
        {
            Kinect = kinectMan;
        }

        /// <summary>
        /// Initialize the BoundingSphere for 
        /// </summary>
        public void InitSphere()
        {
            Matrix transform = new Matrix(1,   0, 0, 0,
                                          0, .5f, 0, 0,
                                          0,   0, 1, 0,
                                          0,   0, 0, 1);
            mSphere = new BoundingSphere(StartingPosition, sphereRadius);
            mSphere.Transform(transform);
        }

        public void ToStart()
        {
            PositionInWorldSpace = StartingPosition;
            mSphere.Center = StartingPosition;
            hData = new HudData(StartingHealth);
        }
        public void LoopPosition(int cityLengthZ)
        {
            float x = PositionInWorldSpace.X;
            float z = PositionInWorldSpace.Z - cityLengthZ;
            PositionInWorldSpace = new Vector3(x, StartingPosition.Y, z);
        }
        public void TargetHit(int points)
        {
            hData.TargetsHit++;
            hData.Score += points;
            Display.Update(hData);
        }
        public void BuildingHit()
        {
            hData.Health -= 50;
            Display.Update(hData);
        }
        
        public void Update(GameTime gameTime, KeyboardState keys)
        {
            float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 500.0f * Speed;
            string posture = Kinect.PostureDetector.CurrentPosture;

            if (posture == "notilt" || keys.IsKeyDown(Keys.None))
            {
                RotationZ *= 0.9f;
            }
            else if (posture == "tiltLeft" || keys.IsKeyDown(Keys.Left))
            {
                RotationZ -= RotationRate;
                if (RotationZ < -MaxTilt) RotationZ = -MaxTilt;
                PositionInWorldSpace += Vector3.Right * moveSpeed;
            }
            else if (posture == "tiltRight" || keys.IsKeyDown(Keys.Right))
            {
                RotationZ += RotationRate;
                if (RotationZ > MaxTilt) RotationZ = MaxTilt;
                PositionInWorldSpace -= Vector3.Right * moveSpeed;
            }
            else
            {
                RotationZ *= 0.9f;
            }
            hData.Score += scoreIncrement;
            PositionInWorldSpace += Vector3.Backward * moveSpeed;
            Display.Update(hData);
        }
    }

    public struct HudData
    {
        public int Health;
        public int TargetsHit;
        public float Score;

        public HudData(int health)
        {
            Health = health;
            TargetsHit = 0;
            Score = 0;
        }
    }
}


