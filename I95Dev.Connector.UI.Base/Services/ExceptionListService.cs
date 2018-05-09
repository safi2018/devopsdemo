using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.Services
{
    internal static class ExceptionListService
    {
        /// <summary>
        /// Loads the exceptions.
        /// </summary>
        /// <returns></returns>
        internal static IList<ExceptionModel> LoadExceptions()
        {
            return ReflectionSearch();
        }

        /// <summary>
        /// Reflections the search.
        /// </summary>
        /// <returns></returns>
        private static IList<ExceptionModel> ReflectionSearch()
        {
            IList<ExceptionModel> exceptions = new List<ExceptionModel>();
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            LoadAssemblies();
            AssemblyName[] assemblyNames = thisAssembly.GetReferencedAssemblies();

            Regex regex = new Regex(@"^(?<path>.*)\.(?<exc>.*?)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            foreach (AssemblyName assemblyName in assemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                foreach (Module module in assembly.GetModules())
                {
                    var moduleList = new SortedList<string, string>();
                    foreach (Type t in module.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(Exception)))
                        {
                            moduleList.Add(t.Namespace + ".1" + t.Name, t.FullName);
                        }
                    }

                    if (moduleList.Count <= 0) continue;
                    string lastPath = "";

                    string xmlFileName = string.Format(Constants.DefaultCulture, @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\en\{0}.xml", module.Name.Substring(0, module.Name.Length - 4));
                    if (File.Exists(xmlFileName))
                    {
                        var xd = new XmlDocument();
                        xd.Load(xmlFileName);
                    }

                    foreach (string excName in moduleList.Values)
                    {
                        Match match = regex.Match(excName);
                        if (!match.Success) continue;
                        string path = match.Groups["path"].Value;
                        if (!string.Equals(path, lastPath, StringComparison.OrdinalIgnoreCase))
                        {
                            lastPath = path;
                        }

                        string fullClassName = string.Format(Constants.DefaultCulture, "{0}.{1}", path, match.Groups["exc"].Value);
                        exceptions.Add(new ExceptionModel { FullName = fullClassName, Module = module.Name, Namespace = path });
                    }
                }
            }
            return exceptions;
        }

        private static void LoadAssemblies()
        {
            //new Sage.SData.Client.Atom.AtomEntry();
        }
    }
}