using System;
using System.Xml.Schema;
using System.Xml;
using System.Runtime.Remoting.Contexts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    public class Program
    {
        // These URLs will be read by the autograder, please keep the variable name un-changed and link to the correct xml/xsd files.
        public static string xmlURL = "https://www.public.asu.edu/~kkhari/Hotels.xml"; //Q1.2
        public static string xmlErrorURL = "https://www.public.asu.edu/~kkhari/HotelsErrors.xml"; //Q1.3
        public static string xsdURL = "https://www.public.asu.edu/~kkhari/Hotels.xsd"; //Q1.1
        private static string xmlContent;
        private static string xsdContent;

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine("Verification Result 1:");
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine("Verification Result 2:");
            Console.WriteLine(result);

            result = Xml2Json(xmlURL); // Corrected method call here
            Console.WriteLine("Xml2Json Result 1:");
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine("Xml2Json Result 2:");
            Console.WriteLine(result);

            // Testing with custom XML and XSD
            string customXml = "<root><item>value</item></root>";
            string customXsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'><xs:element name='root'><xs:complexType><xs:sequence><xs:element name='item' type='xs:string'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
            result = VerificationWithCustomInput(customXml, customXsd); // Corrected method name here
            Console.WriteLine("Verification Result with Custom Input:");
            Console.WriteLine(result);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); // Wait for user input before exiting
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, xsdUrl);
                settings.ValidationType = ValidationType.Schema;

                XmlReader reader = XmlReader.Create(xmlUrl, settings);
                XmlDocument document = new XmlDocument();
                document.Load(reader);

                ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
                document.Validate(eventHandler);

                reader.Close();
                return "No Errors Found!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e) 
        {
            throw new Exception($"Validation Error: {e.Message}");
        }

        // Q2.2
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Download XML content
                using (var client = new System.Net.WebClient())
                {
                    xmlContent = client.DownloadString(xmlUrl);
                }

                // Debugging: Print XML content to verify if it's retrieved correctly
                Console.WriteLine("XML Content:");
                Console.WriteLine(xmlContent);

                // Load XML content from string
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(xmlContent);

                    // Debugging: Print XML document to verify if it's loaded correctly
                    Console.WriteLine("XML Document:");
                    Console.WriteLine(doc.OuterXml);
                }
                catch (Exception ex)
                {
                    return $"Error loading XML content: {ex.Message}";
                }

                // Convert XML to JSON
                string jsonText = "";
                try
                {
                    jsonText = JsonConvert.SerializeXmlNode(doc);
                    return jsonText;
                }
                catch (Exception ex)
                {
                    return $"Error converting XML to JSON: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                return $"Error downloading XML content: {ex.Message}";
            }
        }

        public static string VerificationWithCustomInput(string xmlContent, string xsdContent)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, XmlReader.Create(new System.IO.StringReader(xsdContent)));
                settings.ValidationType = ValidationType.Schema;

                XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlContent), settings);
                XmlDocument document = new XmlDocument();
                document.Load(reader);

                ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
                document.Validate(eventHandler);

                reader.Close();
                return "No Errors Found!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

