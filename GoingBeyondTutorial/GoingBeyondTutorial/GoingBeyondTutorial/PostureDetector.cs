using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;


namespace GoingBeyondTutorial
{
    public abstract class PostureDetector
    {
        public event Action<string> PostureDetected;
        
        protected readonly int accumulatorTarget;
        protected int accumulator;
        protected string accumulatedPosture = "";
        protected string previousPosture = "";

        public string CurrentPosture
        {
            get 
            {
                if (previousPosture != "")
                {
                    string temp = previousPosture;
                    Reset();
                    return temp;
                }
                else
                {
                    return previousPosture;
                }
            }
        }

        protected PostureDetector(int accumulators)
        {
            accumulatorTarget = accumulators;
        }
        public abstract void TrackPostures(Skeleton skeleton);

        protected void RaisePostureDetected(string posture)
        {
            if (accumulator < accumulatorTarget)
            {
                if (accumulatedPosture != posture)
                {
                    accumulator = 0;
                    accumulatedPosture = posture;
                }

                accumulator++;
                return;
            }
            if (previousPosture == posture)
                return;

            previousPosture = posture;
            if (PostureDetected != null)
                PostureDetected(posture);

            accumulator = 0;
        }

        protected void Reset()
        {
            previousPosture = "";
            accumulator = 0;
        }
    }
}

