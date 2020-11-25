using System.Linq;
using System.Xml;
using Verse;

namespace Soong
{
    public class PatchOperationReplace_WithReference : Verse.PatchOperationPathed
    {
        /// <summary>
        /// search the node for <xpath> expressions, resolve these with currentNode as base path and replace their result into the node.
        /// </summary>
        /// <param name="node">node to search for xpath expressions to replace</param>
        /// <param name="currentNode">node to use as base for relative xpath expressions</param>
        protected void ResolveReferences(XmlNode node, XmlNode currentNode)
        {
            foreach (XmlElement reference in node.SelectNodes(".//xpath"))
            {
                foreach (XmlNode refNode in currentNode.SelectNodes(reference.InnerText))
                {
                    reference.ParentNode.InsertBefore(reference.OwnerDocument.ImportNode(refNode,true), reference);
                }
                reference.ParentNode.RemoveChild(reference);
            }
        }

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            foreach (XmlNode xmlNode in xml.SelectNodes(this.xpath).Cast<XmlNode>().ToArray<XmlNode>())
            {
                // resolve references in value node
                XmlNode node = this.value.node.Clone();
                ResolveReferences(node, xmlNode);
                // rest of this loop is the same as vanilla PatchOperation
                result = true;
                XmlNode parentNode = xmlNode.ParentNode;
                foreach (object obj in node.ChildNodes)
                {
                    XmlNode node2 = (XmlNode)obj;
                    parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node2, true), xmlNode);
                }
                parentNode.RemoveChild(xmlNode);
            }
            return result;
        }

        // Token: 0x04000B2E RID: 2862
        private XmlContainer value;
    }
}
