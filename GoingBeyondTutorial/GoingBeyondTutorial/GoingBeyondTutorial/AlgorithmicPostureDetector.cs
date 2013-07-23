using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace GoingBeyondTutorial
{
    public class AlgorithmicPostureDetector : PostureDetector
    {
        public float Epsilon { get; set; }
        public float MaxRange { get; set; }

        // absolute val of maximum tilt for posture (in radians)
        public float MaxTilt { get; set; }
        // absolute val of minimum tilt for posture (in radians)
        public float MinTilt { get; set; }

        public float CurrentTiltLR { get; protected set; }
        public float CurrentTiltFB { get; protected set; }

        // Correction to be set during initial calibration of kinect by user
        public float TiltFBCorrection { get; set; }
        public float TiltLRCorrection { get; set; }

        //Vector3? headPosition = null;
        //Vector3? leftHandPosition = null;
        //Vector3? rightHandPosition = null;
        //Vector3? spinePosition = null;
        Vector3? hipCenterPosition = null;
        //Vector3? hipLeftPosition = null;
        //Vector3? hipRightPosition = null;
        Vector3? shoulderCenterPosition = null;
        //Vector3? shoulderRightPosition = null;
        //Vector3? shoulderLeftPosition = null;

        public AlgorithmicPostureDetector()
            : base(2)
        {
            Epsilon = 0.1f;
            MaxRange = 0.25f;

            MaxTilt = 0.4f;
            MinTilt = 0.1f;

            CurrentTiltLR = 0.0f;
            CurrentTiltFB = 0.0f;

            TiltLRCorrection = 0.0f;
            TiltFBCorrection = 0.0f;

            RaisePostureDetected("notilt");
        }

        public override void TrackPostures(Skeleton skeleton)
        {
            if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                return;

            foreach (Joint joint in skeleton.Joints)
            {

                if (joint.TrackingState != JointTrackingState.Tracked)
                    continue;
                switch (joint.JointType)
                {
                    case JointType.ShoulderCenter:
                        shoulderCenterPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                    case JointType.HipCenter:
                        hipCenterPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                }
            }

            //Check LR tilt
            float tiltLR = GetTiltLR(hipCenterPosition, shoulderCenterPosition);
            if (Math.Abs(tiltLR) <= MinTilt)
            {
                RaisePostureDetected("notilt");
                CurrentTiltLR = 0.0f;
                return;
            }
            // tilt right
            else if (tiltLR < 0 && tiltLR > -MaxTilt)
            {
                RaisePostureDetected("tiltRight");
                CurrentTiltLR = tiltLR;
                return;
            }
            // tilt left
            else if (tiltLR > 0 && tiltLR < MaxTilt)
            {
                RaisePostureDetected("tiltLeft");
                CurrentTiltLR = tiltLR;
                return;
            }

            //if (tiltLR < -.1f && tiltLR > -.4f)
            //{
            //    RaisePostureDetected("right");
            //    //currentTiltLR= tilt
            //    return;
            //}
        }
        bool CheckHandOverHead(Vector3? headPosition, Vector3? handPosition)
        {
            if (!handPosition.HasValue || !headPosition.HasValue)
                return false;
            if (handPosition.Value.Y < headPosition.Value.Y)
                return false;
            if (Math.Abs(handPosition.Value.X - headPosition.Value.X) > MaxRange)
                return false;
            if (Math.Abs(handPosition.Value.Z - headPosition.Value.Z) > MaxRange)
                return false;
            return true;
        }

        bool CheckHello(Vector3? headPosition, Vector3? handPosition)
        {
            if (!handPosition.HasValue || !headPosition.HasValue)
                return false;
            if (Math.Abs(handPosition.Value.X - headPosition.Value.X) < MaxRange)
                return false;
            if (Math.Abs(handPosition.Value.Y - headPosition.Value.Y) > MaxRange)
                return false;
            if (Math.Abs(handPosition.Value.Z - headPosition.Value.Z) > MaxRange)
                return false;
            return true;

        }


        /// <summary>
        /// Returns the Left/Right tilt angle of skeleton in radians. Left tilt returns positive value, Right
        /// tilt returns negative value
        /// </summary>
        /// <param name="hipCenterPosition">Position of hipCenter joint</param>
        /// <param name="shoulderCenterPosition">Position of shoulderCenter joint</param>
        /// <returns></returns>
        float GetTiltLR(Vector3? hipCenterPosition, Vector3? shoulderCenterPosition)
        {
            if (!hipCenterPosition.HasValue || !shoulderCenterPosition.HasValue)
                return 0.0f;

            Vector3 leftRight = (new Vector3(shoulderCenterPosition.Value.X - hipCenterPosition.Value.X,
                                            shoulderCenterPosition.Value.Y - hipCenterPosition.Value.Y,
                                            0));
            leftRight.Normalize();

            return ((float)Math.Atan2(leftRight.Y, leftRight.X) - MathHelper.PiOver2) - TiltLRCorrection;
        }

        /// <summary>
        /// Returns the Front/Back tilt angle of skeleton in radians. Front tilt returns positive value, Back
        /// tilt returns negative value
        /// </summary>
        /// <param name="hipCenterPosition">Position of hipCenter joint</param>
        /// <param name="shoulderCenterPosition">Position of shoulderCenter joint</param>
        /// <returns></returns>
        float GetTiltFB(Vector3? hipCenterPosition, Vector3? shoulderCenterPosition)
        {
            if (!hipCenterPosition.HasValue || !shoulderCenterPosition.HasValue)
                return 0.0f;

            Vector3 frontBack = (new Vector3(0,
                                            shoulderCenterPosition.Value.Y - hipCenterPosition.Value.Y,
                                            shoulderCenterPosition.Value.Z - hipCenterPosition.Value.Z));
            frontBack.Normalize();

            return ((float)Math.Atan2(frontBack.Y, frontBack.Z) - MathHelper.PiOver2) - TiltFBCorrection;
        }
    }
}



