﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The class that is responsible for controlling the editing of the 
	/// Xml tree view.
	/// </summary>
	public class XmlTreeEditor
	{
		IXmlTreeView view;
		XmlDocument document;
		XmlCompletionDataProvider completionDataProvider;
		
		public XmlTreeEditor(IXmlTreeView view, XmlCompletionDataProvider completionDataProvider)
		{
			this.view = view;
			this.completionDataProvider = completionDataProvider;
		}
		
		/// <summary>
		/// Loads the xml into the editor.
		/// </summary>
		public void LoadXml(string xml)
		{
			try {
				document = new XmlDocument();
				document.LoadXml(xml);
				XmlElement documentElement = document.DocumentElement;
				view.DocumentElement = documentElement;
			} catch (XmlException ex) {
				view.ShowXmlIsNotWellFormedMessage(ex);
			}
		}
		
		/// <summary>
		/// Gets the Xml document being edited.
		/// </summary>
		public XmlDocument Document {
			get {
				return document;
			}
		}
		
		/// <summary>
		/// The selected xml element in the view has changed.
		/// </summary>
		public void SelectedElementChanged()
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				view.ShowAttributes(selectedElement.Attributes);
			} else {
				view.ClearAttributes();
			}
		}
		
		/// <summary>
		/// The selected xml text node in the view has changed.
		/// </summary>
		public void SelectedTextNodeChanged()
		{
			XmlText selectedTextNode = view.SelectedTextNode;
			if (selectedTextNode != null) {
				view.ShowTextContent(selectedTextNode.InnerText);
			} else {
				view.ShowTextContent(String.Empty);
			}
		}
		
		/// <summary>
		/// The attribute value has changed.
		/// </summary>
		public void AttributeValueChanged()
		{
			view.IsDirty = true;
		}
		
		/// <summary>
		/// Adds one or more new attribute to the selected element.
		/// </summary>
		public void AddAttribute()
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				string[] attributesNames = GetMissingAttributes(selectedElement);
				string[] selectedAttributeNames = view.SelectNewAttributes(attributesNames);
				if (selectedAttributeNames.Length > 0) {
					foreach (string attributeName in selectedAttributeNames) {
						selectedElement.SetAttribute(attributeName, String.Empty);
					}
					view.IsDirty = true;
					view.ShowAttributes(selectedElement.Attributes);
				}
			}
		}
		
		/// <summary>
		/// Removes the selected attribute from the xml document.
		/// </summary>
		public void RemoveAttribute()
		{
			XmlElement selectedElement = view.SelectedElement;
			if (selectedElement != null) {
				string attribute = view.SelectedAttribute;
				if (attribute != null) {
					selectedElement.RemoveAttribute(attribute);
					view.IsDirty = true;
					view.ShowAttributes(selectedElement.Attributes);
				}
			}
		}
		
		/// <summary>
		/// The text content has been changed in the view.
		/// </summary>
		public void TextContentChanged()
		{
			XmlText textNode = view.SelectedTextNode;
			if (textNode != null) {
				view.IsDirty = true;
				textNode.Value = view.TextContent;
			}
		}
		
		/// <summary>
		/// Gets the missing attributes for the specified element based 
		/// on its associated schema.
		/// </summary>
		string[] GetMissingAttributes(XmlElement element)
		{
			XmlElementPath elementPath = GetElementPath(element);

			List<string> attributes = new List<string>();
			XmlSchemaCompletionData schemaCompletionData = completionDataProvider.FindSchema(elementPath);
			if (schemaCompletionData != null) {
				ICompletionData[] completionData = schemaCompletionData.GetAttributeCompletionData(elementPath);
				foreach (ICompletionData attributeCompletionData in completionData) {					
					// Ignore existing attributes.
					string attributeName = attributeCompletionData.Text;
					if (!element.HasAttribute(attributeName)) {
						attributes.Add(attributeName);
					}
				}
			}
			return attributes.ToArray();
		}
		
		/// <summary>
		/// Returns the path to the specified element starting from the
		/// root element.
		/// </summary>
		XmlElementPath GetElementPath(XmlElement element)
		{
			XmlElementPath path = new XmlElementPath();
			XmlElement parentElement = element;
			while (parentElement != null) {
				QualifiedName name = new QualifiedName(parentElement.LocalName, parentElement.NamespaceURI, parentElement.Prefix);
				path.Elements.Insert(0, name);
				
				// Move to parent element.
				parentElement = parentElement.ParentNode as XmlElement;
			}
			return path;
		}
	}
}
