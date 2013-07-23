using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace GoingBeyondTutorial
{
    class XMLParse
    {
        /// <summary>
        /// Returns the value of an attribute of an XElement, or "" if no such
        /// attribute exists
        /// </summary>
        /// <param name="elem">Element to get attribute from</param>
        /// <param name="AttributeName">Name of the attribute to extract</param>
        /// <returns>Value of the attribute, or "" if it doesn't exist</returns>
        protected static string GetAttributeValue(XElement elem, String AttributeName)
        {
            foreach (XAttribute attr in elem.Attributes())
            {
                if (attr.Name.ToString() == AttributeName)
                {
                    return attr.Value;
                }
            }
            return "";
        }

        /// <summary>
        /// Adds a List<Vector3> valued property to an object, based on
        /// an XML node
        /// </summary>
        /// <param name="elem">XML element to read name / value of property from</param>
        /// <param name="obj">Object to set proprerty on</param>
        protected static void AddVector3ListToClassInstance(XElement elem, Object obj)
        {
            List<Vector3> vectorList = new List<Vector3>();
            foreach (XElement pathPoint in elem.Elements())
            {
                Vector3 nextElem = new Vector3(float.Parse(pathPoint.Element("x").Value),
                                               float.Parse(pathPoint.Element("y").Value),
                                               float.Parse(pathPoint.Element("z").Value));
                vectorList.Add(nextElem);
            }
            PropertyInfo propertyInfo = obj.GetType().GetProperty(elem.Name.ToString());
            propertyInfo.SetValue(obj, vectorList, null);

        }

        /// <summary>
        /// Adds a Vector3 valued property to an object, based on
        /// an XML node
        /// </summary>
        /// <param name="elem">XML element to read name / value of property from</param>
        /// <param name="obj">Object to set proprerty on</param>
        protected static void AddVector3ToClassInstance(XElement elem, Object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(elem.Name.ToString());
            Vector3 valueToSet = new Vector3(float.Parse(elem.Element("x").Value),
                                             float.Parse(elem.Element("y").Value),
                                             float.Parse(elem.Element("z").Value));
            propertyInfo.SetValue(obj, valueToSet, null);
        }


        /// <summary>
        /// Adds a float-valued property to an object, based on
        /// an XML node
        /// </summary>
        /// <param name="elem">XML element to read name / value of property from</param>
        /// <param name="obj">Object to set proprerty on</param>
        protected static void AddFloatToClassInstance(XElement elem, Object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(elem.Name.ToString());
            propertyInfo.SetValue(obj, float.Parse(elem.Value), null);
        }

        /// <summary>
        /// Adds a float-valued property to an object, based on
        /// an XML node
        /// </summary>
        /// <param name="elem">XML element to read name / value of property from</param>
        /// <param name="obj">Object to set proprerty on</param>
        protected static void AddIntToClassInstance(XElement elem, Object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(elem.Name.ToString());
            propertyInfo.SetValue(obj, int.Parse(elem.Value), null);
        }

        /// <summary>
        /// Adds a string-valued property to an object, based on
        /// an XML node
        /// </summary>
        /// <param name="elem">XML element to read name / value of property from</param>
        /// <param name="obj">Object to set proprerty on</param>
        protected static void AddStringToClassInstance(XElement elem, Object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(elem.Name.ToString());
            propertyInfo.SetValue(obj, elem.Value, null);
        }

        /// <summary>
        /// Adds a bool-valued property to an object, based on
        /// an XML node
        /// </summary>
        /// <param name="elem">XML element to read name / value of property from</param>
        /// <param name="obj">Object to set proprerty on</param>
        protected static void AddBoolToClassInstance(XElement elem, Object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(elem.Name.ToString());
            string value = elem.Value.Trim().ToLower();
            bool boolVal = value == "true" || value == "t" || value == "1";
            propertyInfo.SetValue(obj, boolVal, null);
        }


        /// <summary>
        /// Adds a property value to an object, based on an XML node.  Use the
        /// 'type' attribute of the XML node to determine what type the element
        /// we are adding is -- float, Vector3, string, etc.
        /// </summary>
        /// <param name="elem">XML element to read name / value of property from</param>
        /// <param name="obj">Object to set proprerty on</param>
        public static void AddValueToClassInstance(XElement elem, Object obj)
        {
            string type = GetAttributeValue(elem, "type").ToLower();
            if (type == "float")
            {
                AddFloatToClassInstance(elem, obj);
            }
            else if (type == "vector3")
            {
                AddVector3ToClassInstance(elem, obj);
            }
            else if (type == "string")
            {
                AddStringToClassInstance(elem, obj);
            }
            else if (type == "bool")
            {
                AddBoolToClassInstance(elem, obj);
            }
            else if (type == "vector3list")
            {
                AddVector3ListToClassInstance(elem, obj);
            }
            else if (type == "int")
            {
                AddIntToClassInstance(elem, obj);
            }

            else
            {
                throw new FormatException("Unknown Type attribute " + type + " in XML file");
            }

        }
    }
}

