﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace DotNetOpenId.Yadis {
	class ServiceElement : XrdsNode, IComparable<ServiceElement> {
		public ServiceElement(XPathNavigator serviceElement, XrdElement parent) :
			base(serviceElement, parent) {
		}

		public XrdElement Xrd {
			get { return (XrdElement)ParentNode; }
		}

		public int Priority {
			get { return Node.SelectSingleNode("@priority", XmlNamespaceResolver).ValueAsInt; }
		}

		public IEnumerable<UriElement> UriElements {
			get {
				List<UriElement> uris = new List<UriElement>();
				foreach (XPathNavigator node in Node.Select("xrd:URI", XmlNamespaceResolver)) {
					uris.Add(new UriElement(node.Clone(), this));
				}
				uris.Sort();
				return uris;
			}
		}

		public IEnumerable<TypeElement> TypeElements {
			get {
				foreach (XPathNavigator node in Node.Select("xrd:Type", XmlNamespaceResolver)) {
					yield return new TypeElement(node.Clone(), this);
				}
			}
		}

		public string[] TypeElementUris {
			get {
				XPathNodeIterator types = Node.Select("xrd:Type", XmlNamespaceResolver);
				string[] typeUris = new string[types.Count];
				int i = 0;
				foreach(XPathNavigator type in types) {
					typeUris[i++] = type.Value;
				}
				return typeUris;
			}
		}

		public Identifier ProviderLocalIdentifier {
			get {
				var n = Node.SelectSingleNode("xrd:LocalID", XmlNamespaceResolver) 
					?? Node.SelectSingleNode("openid10:Delegate", XmlNamespaceResolver);
				return (n != null) ? n.Value : null;
			}
		}

		#region IComparable<ServiceElement> Members

		public int CompareTo(ServiceElement other) {
			return Priority.CompareTo(other.Priority);
		}

		#endregion
	}
}
