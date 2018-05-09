using System;
using System.Collections;
using System.Windows.Controls;
using I95Dev.Connector.Base.Common;
using Newtonsoft.Json.Linq;

namespace I95Dev.Connector.UI.Base.Helpers
{
    public class JsonViewerHelper
    {
        /// <summary>
        /// This will parse the json string
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="json"></param>
        public bool LoadJsonToTreeView(ItemsControl treeView, string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return false;
                if (json.StartsWith("[", StringComparison.OrdinalIgnoreCase))
                {
                    var @array = JArray.Parse(json);
                    AddArrayNodes(@array, "root", treeView.Items);
                }
                else
                {
                    var @object = JObject.Parse(json);
                    AddObjectNodes(@object, "root", treeView.Items);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// This will process the total json nodes
        /// </summary>
        /// <param name="object"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void AddObjectNodes(JObject @object, string name, IList parent)
        {
            var node = new TreeViewItem
            {
                Header = name
            };
            parent.Add(node);

            foreach (JProperty property in @object.Properties())
            {
                AddTokenNodes(property.Value, property.Name, node.Items);
            }
        }

        /// <summary>
        /// This will parse the array nodes
        /// </summary>
        /// <param name="array"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void AddArrayNodes(JArray array, string name, IList parent)
        {
            var node = new TreeViewItem
            {
                Header = name
            };
            parent.Add(node);

            for (int i = 0; i < array.Count; i++)
            {
                AddTokenNodes(array[i], string.Format(Constants.DefaultCulture, "[{0}]", i + 1), node.Items);
            }
        }

        /// <summary>
        /// This will parse the normal property nodes
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void AddTokenNodes(JToken token, string name, IList parent)
        {
            var value = token as JValue;
            if (value != null)
            {
                var item = new TreeViewItem
                {
                    Header = string.Format(Constants.DefaultCulture, "{0} : {1}", name, value.Value)
                };
                parent.Add(item);
            }
            else
            {
                var array = token as JArray;
                if (array != null)
                {
                    AddArrayNodes(array, name, parent);
                }
                else
                {
                    var o = token as JObject;
                    if (o != null)
                    {
                        AddObjectNodes(o, name, parent);
                    }
                }
            }
        }
    }
}