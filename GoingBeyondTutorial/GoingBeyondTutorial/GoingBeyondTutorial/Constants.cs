using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoingBeyondTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Xna.Framework;

    class Constants
    {

        // Public properties -- constants available for use
        //  throughout the program

        public Vector3 Gravity { get; private set; }
        public float Friction { get; private set; }

        public int MaxX { get; private set; }
        public int MinX { get; private set; }
        public int MaxY { get; private set; }
        public int MinY { get; private set; }
        public int MaxZ { get; private set; }
        public int MinZ { get; private set; }

        public static Constants GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new Constants();
                //mInstance.LoadConstants("Content/Constants.xml");
            }
            return mInstance;
        }


        // Private methods and data fields, for implementing the static class
        // and reading the constants in from an XML file

        private static Constants mInstance;
        private Constants()
        {
            // Set default values for the constants declared in this class
            // These values can be overriden by the XML file
            Gravity = new Vector3(0, 9.8f, 0);
        }

        protected void AddConstant(XElement elem)
        {
            XMLParse.AddValueToClassInstance(elem, Constants.GetInstance());
        }

        private void LoadConstants(String constantFile)
        {
            using (XmlReader reader = XmlReader.Create(new StreamReader(constantFile)))
            {
                XDocument xml = XDocument.Load(reader);
                XElement root = xml.Root;
                foreach (XElement elem in root.Elements())
                {
                    AddConstant(elem);
                }
            }
        }
    }

}

