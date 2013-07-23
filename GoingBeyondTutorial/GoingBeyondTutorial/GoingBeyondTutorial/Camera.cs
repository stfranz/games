using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GoingBeyondTutorial
{
    class Camera
    {
        Player Subject;

        public Matrix Focus
        {
            get { return mFocus; }
        }
        public Matrix FieldOfView
        {
            get
            {
                return Matrix.CreatePerspectiveFieldOfView(
                          MathHelper.ToRadians(45.0f),
                          aspectRatio,
                          0.2f,
                          1000.0f);
            }
        }
        public Matrix Rotation { get; set; }
        public Vector3 PositionInWorldSpace
        {
            get
            { return PositionInSubjectSpace + Subject.PositionInWorldSpace; }
        }

        private Matrix mFocus;
        private Vector3 PositionInSubjectSpace;
        private Vector3 offsetFromSubject;
        private Vector3 lightDirection = new Vector3(3, -2, 5);
        private Vector3 shakeOffset;
        private Random r;
        private float shakeTime;
        private float aspectRatio;

        public Camera(float aspectRatio)
        {
            this.Subject = null;
            this.offsetFromSubject = new Vector3(0.0f, 0.25f, -1.0f);
            this.PositionInSubjectSpace = offsetFromSubject;
            this.Rotation = Subject.RotationInLocalSpace;
            this.aspectRatio = aspectRatio;
            this.r = new Random();
            UpdateFocus();
        }
        public Camera(float aspectRatio, Player obj)
        {
            this.Subject = obj;
            this.offsetFromSubject = new Vector3(0.0f, 0.25f, -1.0f);
            this.PositionInSubjectSpace = offsetFromSubject;
            this.Rotation = Subject.RotationInLocalSpace;
            this.aspectRatio = aspectRatio;
            this.r = new Random();
            UpdateFocus();
        }

        protected double getAngleDelta(double fromAngle, double toAngle)
        {
            double delta = toAngle - fromAngle;
            if (delta > 0)
            {
                if (delta > Math.PI)
                    return -(2 * Math.PI - delta);
                else
                    return delta;
            }
            else
            {
                if (delta < -Math.PI)
                    return 2 * Math.PI + delta;
                else
                    return delta;
            }
        }

        public void Update(GameTime gameTime)
        {
            Rotation = Subject.RotationInLocalSpace;
            PositionInSubjectSpace = Vector3.Transform(offsetFromSubject, Rotation);
            if (shakeTime > 0)
            {
                shakeOffset = new Vector3(r.Next(-1, 1), r.Next(-1, 1), r.Next(-1, 1));
                shakeOffset /= 10;
                shakeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                shakeOffset = Vector3.Zero;
            }
            UpdateFocus();
        }
        private void UpdateFocus()
        {
            mFocus = Matrix.CreateLookAt(PositionInWorldSpace, Subject.PositionInWorldSpace + shakeOffset, Vector3.Up);
        }
        public void Shake(float time)
        {
            shakeTime = time;
        }
        public void Draw(GameObject obj)
        {
            Matrix rotation = obj.RotationInWorldSpace;
            Matrix translation = Matrix.CreateTranslation(obj.PositionInWorldSpace);
            Matrix[] transforms = new Matrix[obj.Model.Bones.Count];
            obj.Model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in obj.Model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * rotation * translation;
                    effect.View = Focus;
                    effect.Projection = FieldOfView;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
        public void DrawClone(GameObject obj, Vector3 offset)
        {
            Matrix rotation = obj.RotationInWorldSpace;
            Matrix translation = Matrix.CreateTranslation(obj.PositionInWorldSpace + offset);
            Matrix[] transforms = new Matrix[obj.Model.Bones.Count];
            obj.Model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in obj.Model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * rotation * translation;
                    effect.View = Focus;
                    effect.Projection = FieldOfView;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public void DrawCity(GraphicsDevice device, Effect effect, VertexBuffer cityVertexBuffer, Texture2D sceneryTexture)
        {
            
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(Focus);
            effect.Parameters["xProjection"].SetValue(FieldOfView);
            effect.Parameters["xTexture"].SetValue(sceneryTexture);

            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(0.3f);


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(cityVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, cityVertexBuffer.VertexCount / 3);
            }
        }
    }
}

