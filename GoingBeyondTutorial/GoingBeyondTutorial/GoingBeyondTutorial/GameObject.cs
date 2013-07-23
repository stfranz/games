using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GoingBeyondTutorial
{
    class GameObject
    {
        public GameObject Parent { get; set; }
        public Vector3 PositionInWorldSpace
        {
            get
            {
                Vector3 worldPosition = PositionInParentSpace;
                GameObject parent = Parent;
                while (parent != null)
                {
                    worldPosition = VecMath.rotateZ(worldPosition, parent.RotationY);
                    worldPosition = worldPosition + parent.PositionInParentSpace;
                    parent = parent.Parent;
                }
                return worldPosition;
            }
            set
            {
                PositionInParentSpace = worldToLocal(value, Parent);
            }
        }
        public Vector3 PositionInParentSpace { get; set; }

        public Matrix RotationInWorldSpace
        {
            get
            {
                // Get the rotation of the object in world space
                //float rotation = RotationY;
                Matrix rotation = RotationInLocalSpace;
                GameObject tmpPar = Parent;
                while (tmpPar != null)
                {
                    rotation *= tmpPar.RotationInLocalSpace;
                    tmpPar = tmpPar.Parent;
                }
                return rotation;
            }
            //set
            //{
            //    //float ancestorRotation = 0.0f;
            //    Matrix ancestorRotation = new Matrix();
            //    GameObject parent = Parent;
            //    while (parent != null)
            //    {
            //        ancestorRotation *= parent.RotationInParentSpace;
            //        parent = parent.Parent;
            //    }
            //    RotationY = value - ancestorRotation;
            //}
        }
        public Matrix RotationInLocalSpace
        {
            get
            {
                return Matrix.CreateRotationX(RotationX) *
                       Matrix.CreateRotationY(RotationY) *
                       Matrix.CreateRotationZ(RotationZ);
            }
        }
        public float RotationRate { get; set; }

        // set in degrees, get in radians
        protected float RotationX { get; set; }
        protected float RotationY { get; set; }
        protected float RotationZ { get; set; }

        public Model Model { get; set; }
        public String ModelName { get; set; }
        public BoundingSphere Sphere
        {
            get
            {
                mSphere.Center = PositionInWorldSpace;
                mSphere.Transform(RotationInWorldSpace);
                return mSphere;
            }
            set
            { mSphere = value; }
        }
        public float SphereRadius { get { return sphereRadius; } set { sphereRadius = value; } }

        protected BoundingSphere mSphere;
        protected float sphereRadius;

        public GameObject()
        {
            Model = null;
            Parent = null;
            PositionInParentSpace = Vector3.Zero;
            RotationX = 0.0f;
            RotationY = 0.0f;
            RotationZ = 0.0f;
            RotationRate = 0.05f;
            mSphere = new BoundingSphere();
        }
        public GameObject(Model model)
            : this()
        {
            Model = model;
        }
        public GameObject(Model model, GameObject parent)
            : this(model)
        {
            Parent = parent;
        }
        public GameObject(Model model, GameObject parent, Vector3 posInParentSpc)
            : this(model, parent)
        {
            PositionInParentSpace = posInParentSpc;
        }

        public Vector3 worldToLocal(Vector3 worldPos, GameObject obj)
        {
            if (obj == null)
            {
                return worldPos;
            }
            Vector3 parentPos = worldToLocal(worldPos, obj.Parent);
            Vector3 localPos = VecMath.rotateY(parentPos - obj.PositionInParentSpace, -obj.RotationY);
            return localPos;
        }
    }

    static class VecMath
    {
        // For RIGHT handed coordinate systems
        // Rotate about z axis
        public static Vector3 rotateX(Vector3 v, float theta)
        {
            return new Vector3(v.X * 1 + 0 + 0,
                                (float)(v.Y * Math.Cos(theta) + 0 - v.Z * Math.Sin(theta)),
                                (float)(v.Y * Math.Sin(theta) + 0 + v.Z * Math.Cos(theta)));
        }
        // Rotate about y axis
        public static Vector3 rotateY(Vector3 v, float theta)
        {
            return new Vector3((float)(v.X * Math.Cos(theta) + 0 - v.Z * Math.Sin(theta)),
                                     0 + v.Y * 1 + 0,
                                     (float)(v.X * Math.Sin(theta) + 0 + v.Z * Math.Cos(theta)));
        }
        // Rotate about z axis
        public static Vector3 rotateZ(Vector3 v, float theta)
        {
            return new Vector3((float)(v.X * Math.Cos(theta) + 0 - v.Y * Math.Sin(theta)),
                                     (float)(v.X * Math.Sin(theta) + 0 + v.Y * Math.Cos(theta)),
                                     0 + 0 + v.Z * 1);
        }
    }
}

