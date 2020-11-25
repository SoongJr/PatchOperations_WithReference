using System.Collections;
using System.Xml;
using Verse;

namespace Soong
{
    public class PatchOperationInsert_WithReference : Verse.PatchOperationPathed
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
                    XmlNode addNode;
                    XmlNode wrapWith = reference.Attributes.GetNamedItem("WrapWith");
                    if (wrapWith == null || wrapWith.Value.NullOrEmpty())
                    {
                        addNode = reference.OwnerDocument.ImportNode(refNode, true);
                    }
                    else
                    {
                        // if the xpath node has an attribute "WrapWith", create an element with that name as a parent for each matched node before adding it.
                        addNode = reference.OwnerDocument.CreateNode("element", wrapWith.Value, "");
                        addNode.AppendChild(reference.OwnerDocument.ImportNode(refNode, true));
                    }
                    reference.ParentNode.InsertBefore(addNode, reference);
                }
                reference.ParentNode.RemoveChild(reference);
            }
        }

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            foreach (object obj in xml.SelectNodes(this.xpath))
            {
                result = true;
                XmlNode xmlNode = obj as XmlNode;
                // resolve references in value node
                XmlNode node = this.value.node.Clone();
                ResolveReferences(node, xmlNode);
                // rest of this loop is the same as vanilla PatchOperation
                XmlNode parentNode = xmlNode.ParentNode;
                if (this.order == PatchOperationInsert_WithReference.Order.Append)
                {
                    IEnumerator enumerator2 = node.ChildNodes.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        object obj2 = enumerator2.Current;
                        XmlNode node2 = (XmlNode)obj2;
                        parentNode.InsertAfter(parentNode.OwnerDocument.ImportNode(node2, true), xmlNode);
                    }
                    goto IL_E0;
                    goto IL_98;
                }
                goto IL_98;
            IL_E0:
                if (xmlNode.NodeType == XmlNodeType.Text)
                {
                    parentNode.Normalize();
                    continue;
                }
                continue;
            IL_98:
                if (this.order == PatchOperationInsert_WithReference.Order.Prepend)
                {
                    for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
                    {
                        parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node.ChildNodes[i], true), xmlNode);
                    }
                    goto IL_E0;
                }
                goto IL_E0;
            }
            return result;
        }

        private XmlContainer value;

        private PatchOperationInsert_WithReference.Order order = PatchOperationInsert_WithReference.Order.Prepend;

        private enum Order
        {
            Append,
            Prepend
        }
    }
}
