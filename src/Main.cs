using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using Mono.Options;
using System.Text;

namespace com.wolfired.u3dot_converter
{
    [XmlRoot("Project")]
    public class Project
    {
        public static Project Load(string filesystemPath, string defaultNamespace = null)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project), defaultNamespace);
            FileStream fileStream = new FileStream(filesystemPath, FileMode.Open);
            Project project = xmlSerializer.Deserialize(fileStream) as Project;
            fileStream.Close();
            return project;
        }

        [XmlRoot("PropertyGroup")]
        public class PropertyGroup
        {
            [XmlRoot("LangVersion")]
            public class LangVersion
            {
                [XmlText]
                public string value;
            }
            [XmlRoot("DefineConstants")]
            public class DefineConstants
            {
                [XmlText]
                public string value;
            }
            [XmlRoot("TargetFramework")]
            public class TargetFramework
            {
                [XmlText]
                public string value;
            }

            [XmlElement("TargetFramework")]
            public List<TargetFramework> listTargetFramework;

            [XmlElement("LangVersion")]
            public List<LangVersion> listLangVersion;
            [XmlElement("DefineConstants")]
            public List<DefineConstants> listDefineConstants;
        }

        [XmlRoot("ItemGroup")]
        public class ItemGroup
        {
            [XmlRoot("ItemGroup")]
            public class Reference
            {
                [XmlRoot("HintPath")]
                public class HintPath
                {
                    [XmlText]
                    public string value;
                }

                [XmlAttribute("Include")]
                public string Include;

                [XmlElement("HintPath")]
                public List<HintPath> listHintPath;
            }

            [XmlRoot("PackageReference")]
            public class PackageReference
            {
                [XmlAttribute("Include")]
                public string Include;
                [XmlAttribute("Version")]
                public string Version;
            }

            [XmlRoot("ProjectReference")]
            public class ProjectReference
            {
                [XmlAttribute("Include")]
                public string Include;
            }

            [XmlElement("Reference")]
            public List<Reference> listReference;

            [XmlElement("PackageReference")]
            public List<PackageReference> listPackageReference;

            [XmlElement("ProjectReference")]
            public List<ProjectReference> listProjectReference;
        }

        [XmlAttribute("Sdk")]
        public string sdk;

        [XmlElement("PropertyGroup")]
        public List<PropertyGroup> listPropertyGroup;

        [XmlElement("ItemGroup")]
        public List<ItemGroup> listItemGroup;

        public void Save(string filesystemPath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project));
            xmlSerializer.Serialize(new StreamWriter(filesystemPath, false, Encoding.UTF8), this, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var help = false;
            string csproj_file_src = null;
            string namespace_src = null;

            string csproj_file_dst = null;
            string namespace_dst = null;

            List<string> skipReferences = new List<string>();

            var os = new OptionSet
            {
                {"h|help", "Show help message", v => help = null != v},
                {"cfsrc=", "Untiy csproj file", v => csproj_file_src = v},
                {"nssrc=", "Untiy csproj file xml namespace", v => namespace_src = v},
                {"cfdst=", "Dotnet csproj file", v => csproj_file_dst = v},
                {"nsdst=", "Dotnet csproj file xml namespace", v => namespace_dst = v},
                {"skips=", "Skip Reference", v => skipReferences.Add(v)},
            };

            os.Parse(args);

            if (help)
            {
                os.WriteOptionDescriptions(Console.Out);
                return;
            }

            var prj_u3d = Project.Load(csproj_file_src, namespace_src);
            var prj_dot = Project.Load(csproj_file_dst, namespace_dst);

            foreach (var skipReference in skipReferences)
            {
                foreach (var ig in prj_u3d.listItemGroup)
                {
                    for (int i = ig.listReference.Count - 1; i >= 0; --i)
                    {
                        if (ig.listReference[i].Include == skipReference)
                        {
                            ig.listReference.RemoveAt(i);
                        }
                    }
                    for (int i = ig.listProjectReference.Count - 1; i >= 0; --i)
                    {
                        if (ig.listProjectReference[i].Include == skipReference)
                        {
                            ig.listProjectReference.RemoveAt(i);
                        }
                    }
                }
            }

            prj_dot.listItemGroup.AddRange(prj_u3d.listItemGroup);
            prj_dot.listPropertyGroup.AddRange(prj_u3d.listPropertyGroup);

            prj_dot.Save(csproj_file_dst);
            // File.WriteAllText(csproj_file_dst, prj_dot.ToXMLString());
        }
    }
}
