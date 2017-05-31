﻿using System;
using System.IO;
using System.Xml;

namespace dotNetCoreDemo {
    class Program {
        static void Main (string[] args)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode newNode = CreateNewPanoProjectFile(xmlDoc);

            XmlNode oldNode = GetPanoTemplate();

            WritePanoProjectFile(xmlDoc, newNode, oldNode);

        }

        static XmlNode GetPanoTemplate()
        {
            string path = @"GeneratedPanoProjectDemo.p2vr";
            XmlDocument tempXml = new XmlDocument();
            XmlNode oldNode;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {

                    tempXml.Load(sr);

                    XmlElement oldRootNode = tempXml.DocumentElement;
                    oldNode = oldRootNode.SelectSingleNode("panorama");

                }
            }

           return oldNode;
        }

        static XmlNode CreateNewPanoProjectFile(XmlDocument xmlDoc)
        {
            string path = @"PanoDataTemplate.csv";

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    string line;
                    // Create Root Node 
                    //XmlNode rootNode = xmlDoc.CreateElement("newPano");
                    XmlNode childRootNode = xmlDoc.CreateElement("tour");

                    // xmlDoc.AppendChild(rootNode);

                    Panodata pd = new Panodata();

                    // Skip First Line 
                    sr.ReadLine();

                    while ((line = sr.ReadLine()) != null)
                    {
                        pd = ReadPanoData(pd, line);
                        //childNodes = GeneratePanoXml(xmlDoc, pd);
                        childRootNode.AppendChild(GeneratePanoXml(xmlDoc, pd));

                    }

                    //rootNode.AppendChild(childRootNode);

                    //Console.WriteLine(rootNode.OuterXml);


                    return childRootNode;
                    // WriteXmlfile(xmlDoc);

                }
            }
        }

        static Panodata ReadPanoData(Panodata pd, String panodata) {

            var delimiters = new char[] {'\t'};
            var items = panodata.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            pd.nodeId = items[0];
            pd.filename = items[1];
            pd.title = items[2];
            pd.panoNorth = int.Parse(items[3].ToString());
            pd.viewPan = int.Parse(items[4].ToString());

            pd.forwardSpot = bool.Parse(items[5].ToString());
            pd.backwardSpot = bool.Parse(items[6].ToString());
            pd.leftSpot = bool.Parse(items[7].ToString());
            pd.rightSpot = bool.Parse(items[8].ToString());;

            // Console.WriteLine("Node Id :{0}",pd.nodeId);
            // Console.WriteLine("Image filename :{0}",pd.filename);
            // Console.WriteLine("Pano Title :{0}",pd.title);
            // Console.WriteLine("Pano North :{0}",pd.panoNorth);
            // Console.WriteLine("Viewing Pan :{0}",pd.viewPan);
            // Console.WriteLine("Spot on forward set to {0}",pd.forwardSpot);
            // Console.WriteLine("Spot on backward set to {0}",pd.backwardSpot);
            // Console.WriteLine("Spot on left set to {0}",pd.leftSpot);
            // Console.WriteLine("Spot on right set to {0}",pd.rightSpot);

            return pd;

        }

        static XmlNode GeneratePanoXml(XmlDocument xmlDoc, Panodata pd) {

            // XmlNode childRootNode = xmlDoc.CreateElement("tour");

            XmlNode childNodes = xmlDoc.CreateElement("panorama");

            //Start
            XmlNode startNode = xmlDoc.CreateElement("start");
            startNode.InnerText = "node1";

            XmlNode panoId = xmlDoc.CreateElement("id");
            panoId.InnerText = pd.nodeId;

            // Input
            XmlNode inputNode = xmlDoc.CreateElement("input");
            inputNode.InnerXml = String.Format("<type>auto</type><filename>{0}</filename>", pd.filename);

            
            //Viewingparameter
            XmlNode viewParam = xmlDoc.CreateElement("viewingparameter");
            viewParam.InnerXml = String.Format("<pan><start>{0}</start></pan><tilt><start>0</start></tilt><fov><start>100</start></fov><panonorth>{1}</panonorth><projection>rectilinear</projection>", pd.viewPan, pd.panoNorth);
            
            //Userdata
            XmlNode userdataNode = xmlDoc.CreateElement("userdata");
            userdataNode.InnerXml = String.Format("<title>{0}</title>",pd.title);

            //Hotspots
            XmlNode hotspotsNode = xmlDoc.CreateElement("hotspots");

            string spotxml = hotspotsNode.InnerXml;

            if (pd.forwardSpot) {
                spotxml = pd.fwdXml;
            }
            if (pd.backwardSpot) {
                spotxml += pd.bwdXml;
            }
            if (pd.leftSpot) {
                spotxml += pd.leftXml;
            }
            if (pd.rightSpot) {
                spotxml += pd.rightXml;
            }
            
            hotspotsNode.InnerXml = spotxml;

            childNodes.AppendChild(startNode);
            childNodes.AppendChild(panoId);
            childNodes.AppendChild(inputNode);
            childNodes.AppendChild(viewParam);
            childNodes.AppendChild(userdataNode);
            childNodes.AppendChild(hotspotsNode);

            return childNodes;
        }

        static void WriteXmlfile(XmlDocument xmlDoc)
        {
            string path = @"text.xml";

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var sr = new StreamWriter(fs))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    XmlWriter writer = XmlWriter.Create(sr, settings);
                    xmlDoc.Save(writer);
                }
            }
        }    


        static void WritePanoProjectFile(XmlDocument xmlDoc, XmlNode newNode, XmlNode oldNode) {
            
            //string oldNodeXml = oldNode.InnerXml;

            XmlNode newPanoram = xmlDoc.CreateElement("panorama");
            newPanoram.InnerXml = oldNode.InnerXml;

            xmlDoc.AppendChild(newNode);

            xmlDoc.DocumentElement.AppendChild(newPanoram);
            WriteXmlfile(xmlDoc);
        }

    }




    public class Panodata {
        public string nodeId;
        public string filename;
        public string title;
        public int panoNorth;
        public int viewPan;
        public Boolean forwardSpot;
        public Boolean backwardSpot;
        public Boolean leftSpot;
        public Boolean rightSpot;

        public string fwdXml = "<hotspot><position><pan>0.00</pan><tilt>-35.00</tilt></position><polygon/><type>point</type><id>FwdPoint</id><linktype>node</linktype><target>$fwd</target><skinid>ht_node_forward</skinid></hotspot>";
        
        public string bwdXml = "<hotspot><position><pan>180.00</pan><tilt>-35.00</tilt></position><polygon/><type>point</type><id>BwdPoint</id><linktype>node</linktype><target>$fwd</target><skinid>ht_node_forward</skinid></hotspot>";

        public string leftXml = "<hotspot><position><pan>90.00</pan><tilt>-35.00</tilt></position><polygon/><type>point</type><id>leftPoint</id><linktype>node</linktype><target>$fwd</target><skinid>ht_node_forward</skinid></hotspot>";
        public string rightXml = "<hotspot><position><pan>-90.00</pan><tilt>-35.00</tilt></position><polygon/><type>point</type><id>rightPoint</id><linktype>node</linktype><target>$fwd</target><skinid>ht_node_forward</skinid></hotspot>";

    }

}

