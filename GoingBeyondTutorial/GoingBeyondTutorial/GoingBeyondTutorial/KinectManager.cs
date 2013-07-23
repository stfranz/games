using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace GoingBeyondTutorial
{
    class KinectManager
    {
        public AlgorithmicPostureDetector PostureDetector { get; set; }
        
        public KinectSensor KinectSensor { get; set; }
        public string ConnectedStatus = "Kinect Device not connected";

        GraphicsDeviceManager graphics;

        Texture2D kinectRGBVideo;
        Texture2D overlay;
        Texture2D hand;

        public KinectManager(GraphicsDeviceManager graph)
        {
            graphics = graph;
            KinectSensor.KinectSensors.StatusChanged += 
                        new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            this.PostureDetector = new AlgorithmicPostureDetector();
            kinectRGBVideo = new Texture2D(graphics.GraphicsDevice, 1337, 1337);
        }

        public void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (this.KinectSensor == e.Sensor)
            {
                if (e.Status == KinectStatus.Disconnected ||
                    e.Status == KinectStatus.NotPowered)
                {
                    this.KinectSensor = null;
                    this.DiscoverKinectSensor();
                }
            }
        }

        private bool InitializeKinect()
        {
            // Color stream
            KinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            KinectSensor.ColorFrameReady += 
                        new EventHandler<ColorImageFrameReadyEventArgs>(KinectSensor_ColorFrameReady);

            // Skeleton Stream
            KinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Smoothing = 0.5f,
                Correction = 0.1f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });
            KinectSensor.SkeletonFrameReady 
                        += new EventHandler<SkeletonFrameReadyEventArgs>(KinectSensor_SkeletonFrameReady);

            try
            {
                KinectSensor.Start();
            }
            catch
            {
                ConnectedStatus = "Unable to start the Kinect Sensor";
                return false;
            }
            return true;
        }

        void KinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == 
                                                   SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null)
                    {
                        PostureDetector.TrackPostures(playerSkeleton);
                    }


                }
            }
        }

        void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame != null)
                {

                    byte[] pixelsFromFrame = new byte[colorImageFrame.PixelDataLength];

                    colorImageFrame.CopyPixelDataTo(pixelsFromFrame);

                    Color[] color = new Color[colorImageFrame.Height * colorImageFrame.Width];
                    kinectRGBVideo = new Texture2D(graphics.GraphicsDevice, colorImageFrame.Width, colorImageFrame.Height);

                    // Go through each pixel and set the bytes correctly
                    // Remember, each pixel got a Rad, Green and Blue
                    int index = 0;
                    for (int y = 0; y < colorImageFrame.Height; y++)
                    {
                        for (int x = 0; x < colorImageFrame.Width; x++, index += 4)
                        {
                            color[y * colorImageFrame.Width + x] = new Color(pixelsFromFrame[index + 2], pixelsFromFrame[index + 1], pixelsFromFrame[index + 0]);
                        }
                    }

                    // Set pixeldata from the ColorImageFrame to a Texture2D
                    kinectRGBVideo.SetData(color);
                }
            }
        }

        private void DiscoverKinectSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    // Found one, set our sensor to this
                    KinectSensor = sensor;
                    break;
                }
            }

            if (this.KinectSensor == null)
            {
                ConnectedStatus = "Kinect Sensors not detected in USB port";
                return;
            }

            // You can use the KinectSensor.Status to check for status
            // and give the user some kind of feedback
            switch (KinectSensor.Status)
            {
                case KinectStatus.Connected:
                    {
                        ConnectedStatus = "Kinect device connected";
                        break;
                    }
                case KinectStatus.Disconnected:
                    {
                        ConnectedStatus = "Kinect device disconnected";
                        break;
                    }
                case KinectStatus.NotPowered:
                    {
                        ConnectedStatus = "Kinect device not powered";
                        break;
                    }
                default:
                    {
                        ConnectedStatus = "Kinect Sensor Error";
                        break;
                    }
            }

            // Init the found and connected device
            if (KinectSensor.Status == KinectStatus.Connected)
            {
                InitializeKinect();
            }
        }

        public void UnloadContent()
        {
            if (KinectSensor != null)
            {
                KinectSensor.Stop();
                KinectSensor.Dispose();
            }
        }
    }
}
