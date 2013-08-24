﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace HasK.Math.Graph
{
    /// <summary>
    /// Represent class of objects which can auto name itself
    /// </summary>
    public class AutoNameManager
    {
        /// <summary>
        /// First possible char of big name
        /// </summary>
        private char FirstBigChar = 'A';
        /// <summary>
        /// Last possible char of big name
        /// </summary>
        private char LastBigChar = 'Z';
        /// <summary>
        /// First possible char of small name
        /// </summary>
        private char FirstSmallChar = 'a';
        /// <summary>
        /// Last possible char of small name
        /// </summary>
        private char LastSmallChar = 'z';

        /// <summary>
        /// Current big name
        /// </summary>
        private string BigName = "";
        /// <summary>
        /// Current small name
        /// </summary>
        private string SmallName = "";

        /// <summary>
        /// Generate next name for current in specified chars range
        /// </summary>
        /// <param name="current">Current name</param>
        /// <param name="first">First char in name rnange</param>
        /// <param name="last">Last char in name range</param>
        /// <returns>Returns next name for given current name</returns>
        private static string GetNext(string current, char first, char last)
        {
            if (current == "")
                return first.ToString();
            var lc = current[current.Length - 1];
            if (lc < last)
                current = current.Substring(0, current.Length - 1) + (char)(((int)lc) + 1);
            else
                current = GetNext(current.Substring(0, current.Length - 1), first, last) + first;
            return current;
        }

        /// <summary>
        /// Generate next name without saving it
        /// </summary>
        /// <param name="small">Determine if name should be in small letters</param>
        /// <returns>Returns next name without saving it</returns>
        public string PeekNextName(bool small)
        {
            if (small)
                return GetNext(SmallName, FirstSmallChar, LastSmallChar);
            else
                return GetNext(BigName, FirstBigChar, LastBigChar);
        }

        /// <summary>
        /// Generate and save next name
        /// </summary>
        /// <param name="small">Determine if name should be in small letters</param>
        /// <returns>Returns next name</returns>
        public string GetNextName(bool small)
        {
            if (small)
            {
                var next = GetNext(SmallName, FirstSmallChar, LastSmallChar);
                SmallName = next;
                return next;
            }
            else
            {
                var next = GetNext(BigName, FirstBigChar, LastBigChar);
                BigName = next;
                return next;
            }
        }

        /// <summary>
        /// Generate and save next big name
        /// </summary>
        /// <returns>Returns next big name</returns>
        public string GetNextName()
        {
            return GetNextName(false);
        }
    }

    /// <summary>
    /// Represent one vertex of graph
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Name of vertex
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value of vertex
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Graph which owns this vertex
        /// </summary>
        public Graph Graph { get; private set; }

        internal Vertex(Graph graph, string name)
        {
            Graph = graph;
            Name = name;
            Value = 0;
        }

        internal Vertex(Graph graph)
        {
            Graph = graph;
            Name = graph.GetNextName();
            Value = 0;
        }

        internal Vertex(Graph graph, string name, double value)
        {
            Graph = graph;
            Name = name;
            Value = value;
        }

        internal Vertex(Graph graph, double value)
        {
            Graph = graph;
            Name = graph.GetNextName();
            Value = value;
        }

        /// <summary>
        /// Vertex string representation
        /// </summary>
        public override string ToString()
        {
            return String.Format("<Vertex '{0}' of {1}>", Name, Graph);
        }
    }

    /// <summary>
    /// Represent one link of graph between two vertices
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Name of link
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value of link
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Graph which owns this link
        /// </summary>
        public Graph Graph { get; private set; }

        /// <summary>
        /// From-vertex of link
        /// </summary>
        public Vertex From { get; private set; }

        /// <summary>
        /// To-vertex of link
        /// </summary>
        public Vertex To { get; private set; }

        internal Link(Graph graph, string name, Vertex from, Vertex to)
        {
            Graph = graph;
            Name = name;
            From = from;
            To = to;
            Value = 0;
        }

        internal Link(Graph graph, Vertex from, Vertex to)
        {
            Graph = graph;
            Name = graph.GetNextName(true);
            From = from;
            To = to;
            Value = 0;
        }

        internal Link(Graph graph, string name, Vertex from, Vertex to, double value)
        {
            Graph = graph;
            Name = name;
            From = from;
            To = to;
            Value = value;
        }

        internal Link(Graph graph, Vertex from, Vertex to, double value)
        {
            Graph = graph;
            Name = graph.GetNextName(true);
            From = from;
            To = to;
            Value = value;
        }

        /// <summary>
        /// String representation of link
        /// </summary>
        public override string ToString()
        {
            return String.Format("<Link '{0}' of {1}>", Name, Graph);
        }

    }

    /// <summary>
    /// Math graph - list of vertices and links between it
    /// </summary>
    public class Graph: AutoNameManager
    {
        /// <summary>
        /// Name of graph
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates is that graph is undirected
        /// </summary>
        public bool Undirected { get; set; }

        private List<Vertex> vertices = new List<Vertex>();
        private List<Link> links = new List<Link>();

        /// <summary>
        /// Create new graph with given name and undirected attribute
        /// </summary>
        /// <param name="name">Name of graph</param>
        /// <param name="undirected">Undirected attribute</param>
        public Graph(string name, bool undirected)
        {
            Name = name;
            Undirected = undirected;
        }

        /// <summary>
        /// Create new undirected graph with given name
        /// </summary>
        /// <param name="name"></param>
        public Graph(string name)
        {
            Name = name;
            Undirected = false;
        }

        /// <summary>
        /// Create new graph with auto name and undirected attribyte
        /// </summary>
        /// <param name="undirected">Undirected attribute</param>
        public Graph(bool undirected)
        {
            Name = this.GetNextName();
            Undirected = undirected;
        }

        /// <summary>
        /// Create new directed graph with auto name
        /// </summary>
        public Graph()
        {
            Name = this.GetNextName();
            Undirected = false;
        }

        /// <summary>
        /// Checks if vertex with given name presented
        /// </summary>
        /// <param name="name">Name of vertex to search</param>
        /// <returns>Returns rrue if vertex found, otherwise false</returns>
        public bool HasVertex(string name)
        {
            foreach (var vertex in vertices)
                if (vertex.Name == name)
                    return true;
            return false;
        }

        /// <summary>
        /// Add new vertex with given name to graph
        /// </summary>
        /// <param name="name">Name of new vertex</param>
        /// <param name="value">Value of new vertex</param>
        /// <returns>Returns created vertex</returns>
        public Vertex AddVertex(string name, double value)
        {
            if (HasVertex(name))
                throw new ArgumentException("Vertex with given name already presented in graph");
            var v = new Vertex(this, name, value);
            vertices.Add(v);
            return v;
        }

        /// <summary>
        /// Add new vertex with auto name to graph
        /// </summary>
        /// <param name="value">Value of new vertex</param>
        /// <returns>Returns created vertex</returns>
        public Vertex AddVertex(double value)
        {
            string name;
            while (true) {
                name = this.GetNextName();
                foreach (var vertex in vertices)
                    if (vertex.Name == name)
                        continue;
                break;
            }
            var v = new Vertex(this, name, value);
            vertices.Add(v);
            return v;
        }

        /// <summary>
        /// Add new vertex with given name to graph
        /// </summary>
        /// <param name="name">Name of new vertex</param>
        /// <returns>Returns created vertex</returns>
        public Vertex AddVertex(string name)
        {
            return AddVertex(name, 0);
        }

        /// <summary>
        /// Add new vertex with auto name to graph
        /// </summary>
        /// <returns>Returns created vertex</returns>
        public Vertex AddVertex()
        {
            return AddVertex(0);
        }

        /// <summary>
        /// Get all vertices of graph
        /// </summary>
        /// <returns>Returns array of vertices</returns>
        public Vertex[] GetVertices()
        {
            return vertices.ToArray();
        }

        /// <summary>
        /// Get vertex by name
        /// </summary>
        /// <param name="name">Name of vertex</param>
        /// <returns>Returns vertex with given name or null if not found</returns>
        public Vertex GetVertex(string name)
        {
            foreach (var vertex in vertices)
                if (vertex.Name == name)
                    return vertex;
            return null;
        }

        /// <summary>
        /// Remove vertex from graph
        /// </summary>
        /// <param name="vertex">Vertex to remove</param>
        /// <returns>Returns true if successfully removed</returns>
        public bool RemoveVertex(Vertex vertex)
        {
            foreach (var link in GetVertexLinks(vertex))
                RemoveLink(link);
            return vertices.Remove(vertex);
        }

        /// <summary>
        /// Remove vertex by name
        /// </summary>
        /// <param name="name">Name of vertex</param>
        /// <returns>Returns true if successfully removed</returns>
        public bool RemoveVertex(string name)
        {
            var vertex = GetVertex(name);
            if (vertex != null)
                return RemoveVertex(vertex);
            return false;
        }

        /// <summary>
        /// Returns next available link name
        /// </summary>
        private string GetNextLinkName()
        {
            string name;
            while (true)
            {
                name = this.GetNextName(true);
                foreach (var link in links)
                    if (link.Name == name)
                        continue;
                break;
            }
            return name;
        }

        /// <summary>
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="name">Name of new link</param>
        /// <param name="from">Start vertice of link</param>
        /// <param name="to">End vertice of link</param>
        /// <param name="value">Value of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(string name, Vertex from, Vertex to, double value)
        {
            foreach (var link in links)
                if (link.Name == name)
                    throw new ArgumentException("Link with given name already presented in graph");
            if (from.Graph != this)
                throw new ArgumentException("From-vertice not presented in graph");
            if (to.Graph != this)
                throw new ArgumentException("To-vertice not presented in graph");
            if (from == to)
                throw new ArgumentException("Cannot create link to from-vertex");
            if (Undirected)
                // check if link to-from already presented
                foreach (var link in links)
                    if (link.From == to && link.To == from)
                        return null;
            var l = new Link(this, name, from, to, value);
            links.Add(l);
            return l;
        }

        /// <summary>
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="from">Start vertice of link</param>
        /// <param name="to">End vertice of link</param>
        /// <param name="value">Value of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(Vertex from, Vertex to, double value)
        {
            return AddLink(GetNextLinkName(), from, to, value);
        }

        /// <summary>
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="name">Name of new link</param>
        /// <param name="from">Name of from-vertice of link</param>
        /// <param name="to">Name of to-vertice of link</param>
        /// <param name="value">Value of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(string name, string from, string to, double value)
        {
            var vfrom = GetVertex(from);
            if (vfrom == null)
                throw new ArgumentException("From-vertice with given name not presented in graph");
            var vto = GetVertex(to);
            if (vto == null)
                throw new ArgumentException("To-vertice with given name not presented in graph");
            return AddLink(name, vfrom, vto, value);
        }

        /// <summary>
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="from">Name of from-vertice of link</param>
        /// <param name="to">Name of to-vertice of link</param>
        /// <param name="value">Value of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(string from, string to, double value)
        {
            return AddLink(GetNextLinkName(), from, to, value);
        }

        /// <summary>
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="from">Start vertice of link</param>
        /// <param name="to">End vertice of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(Vertex from, Vertex to)
        {
            return AddLink(from, to, 0);
        }

        /// <summary>
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="from">Name of from-vertice of link</param>
        /// <param name="to">Name of to-vertice of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(string from, string to)
        {
            return AddLink(from, to, 0);
        }

        /// <summary>
        /// Get all links of graph
        /// </summary>
        public Link[] GetLinks()
        {
            return links.ToArray();
        }

        /// <summary>
        /// Get link by name
        /// </summary>
        /// <param name="name">Name of link</param>
        /// <returns>Returns link with given name or null if not found</returns>
        public Link GetLink(string name)
        {
            foreach (var link in links)
                if (link.Name == name)
                    return link;
            return null;
        }

        /// <summary>
        /// Get vertices with given value
        /// </summary>
        /// <param name="value">Value of vertices to search</param>
        /// <returns>Returns array of vertices with given value</returns>
        public Vertex[] GetVerticesByValue(double value)
        {
            var found = new List<Vertex>();
            foreach (var vertex in vertices)
                if (vertex.Value == value)
                    found.Add(vertex);
            return found.ToArray();
        }

        /// <summary>
        /// Get links with given value
        /// </summary>
        /// <param name="value">Value of links to search</param>
        /// <returns>Returns array of links with given value</returns>
        public Link[] GetLinksByValue(double value)
        {
            var found = new List<Link>();
            foreach (var link in links)
                if (link.Value == value)
                    found.Add(link);
            return found.ToArray();
        }

        /// <summary>
        /// Remove link from graph
        /// </summary>
        /// <param name="link">Link to remove</param>
        /// <returns>Returns true if successfully removed</returns>
        public bool RemoveLink(Link link)
        {
            return links.Remove(link);
        }

        /// <summary>
        /// Remove link by name
        /// </summary>
        /// <param name="name">Name of link</param>
        /// <returns>Returns true if successfully removed</returns>
        public bool RemoveLink(string name)
        {
            var link = GetLink(name);
            if (link != null)
                return RemoveLink(link);
            return false;
        }

        /// <summary>
        /// Get links from given vertex
        /// </summary>
        /// <param name="vertex">Vertex to find links</param>
        /// <returns>Returns array of links from given vertex</returns>
        public Link[] GetVertexFromLinks(Vertex vertex)
        {
            var found = new List<Link>();
            foreach (var link in links)
                if (link.From == vertex)
                    found.Add(link);
            return found.ToArray();
        }

        /// <summary>
        /// Get links from given vertex
        /// </summary>
        /// <param name="name">Vertex name to find links</param>
        /// <returns>Returns array of links from given vertex</returns>
        public Link[] GetVertexFromLinks(string name)
        {
            var vertex = GetVertex(name);
            if (vertex == null)
                throw new ArgumentException("Vertex with given name not presented in graph");
            return GetVertexFromLinks(vertex);
        }

        /// <summary>
        /// Get links to given vertex
        /// </summary>
        /// <param name="vertex">Vertex to find links</param>
        /// <returns>Returns array of links to given vertex</returns>
        public Link[] GetVertexToLinks(Vertex vertex)
        {
            var found = new List<Link>();
            foreach (var link in links)
                if (link.To == vertex)
                    found.Add(link);
            return found.ToArray();
        }

        /// <summary>
        /// Get links to given vertex
        /// </summary>
        /// <param name="name">Vertex name to find links</param>
        /// <returns>Returns array of links to given vertex</returns>
        public Link[] GetVertexToLinks(string name)
        {
            var vertex = GetVertex(name);
            if (vertex == null)
                throw new ArgumentException("Vertex with given name not presented in graph");
            return GetVertexToLinks(vertex);
        }

        /// <summary>
        /// Get links from and to given vertex
        /// </summary>
        /// <param name="vertex">Vertex to find links</param>
        /// <returns>Returns array of links from and to given vertex</returns>
        public Link[] GetVertexLinks(Vertex vertex)
        {
            var found = new List<Link>();
            foreach (var link in links)
                if (link.From == vertex || link.To == vertex)
                    found.Add(link);
            return found.ToArray();
        }

        /// <summary>
        /// Get links from and to given vertex
        /// </summary>
        /// <param name="name">Vertex name to find links</param>
        /// <returns>Returns array of links from and to given vertex</returns>
        public Link[] GetVertexLinks(string name)
        {
            var vertex = GetVertex(name);
            if (vertex == null)
                throw new ArgumentException("Vertex with given name not presented in graph");
            return GetVertexLinks(vertex);
        }

        /// <summary>
        /// String representation of graph
        /// </summary>
        public override string ToString()
        {
            return String.Format("<Graph '{0}'>", Name);
        }
    }
}