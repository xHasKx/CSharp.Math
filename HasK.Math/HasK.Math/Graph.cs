using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace HasK.Math.Graph
{
    public class Vertex
    {
        /// <summary>
        /// Name of vertex
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Graph which owns this vertex
        /// </summary>
        public Graph Graph { get; private set; }

        internal Vertex(Graph graph, string name)
        {
            Graph = graph;
            Name = name;
        }

        public override string ToString()
        {
            return String.Format("<Vertex '{0}' of {1}>", Name, Graph);
        }
    }

    public class Link
    {
        /// <summary>
        /// Name of link
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Graph which owns this link
        /// </summary>
        public Graph Graph { get; private set; }

        public Vertex From { get; private set; }
        public Vertex To { get; private set; }

        internal Link(Graph graph, string name, Vertex from, Vertex to)
        {
            Graph = graph;
            Name = name;
            From = from;
            To = to;
        }

        public override string ToString()
        {
            return String.Format("<Link '{0}' of {1}>", Name, Graph);
        }

    }

    /// <summary>
    /// Math graph - list of vertices and links between it
    /// </summary>
    public class Graph
    {
        private List<Vertex> vertices = new List<Vertex>();
        private List<Link> links = new List<Link>();

        /// <summary>
        /// Name of graph
        /// </summary>
        public string Name { get; set; }

        public Graph(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Add new vertex with given name to graph
        /// </summary>
        /// <param name="name">Name of new vertex</param>
        /// <returns>Returns created vertex</returns>
        public Vertex AddVertex(string name)
        {
            foreach (var vertex in vertices)
                if (vertex.Name == name)
                    throw new ArgumentException("Vertex with given name already exists in graph");
            var v = new Vertex(this, name);
            vertices.Add(v);
            return v;
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
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="name">Name of new link</param>
        /// <param name="from">Start vertice of link</param>
        /// <param name="to">End vertice of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(string name, Vertex from, Vertex to)
        {
            foreach (var link in links)
                if (link.Name == name)
                    throw new ArgumentException("Link with given name already exists in graph");
            if (from.Graph != this)
                throw new ArgumentException("From-vertice not presented in graph");
            if (to.Graph != this)
                throw new ArgumentException("To-vertice not presented in graph");
            if (from == to)
                throw new ArgumentException("Cannot create link to from-vertex");
            var l = new Link(this, name, from, to);
            links.Add(l);
            return l;
        }

        /// <summary>
        /// Add new link to graph between two vertices
        /// </summary>
        /// <param name="name">Name of new link</param>
        /// <param name="from">Start vertice of link</param>
        /// <param name="to">End vertice of link</param>
        /// <returns>Returns new link</returns>
        public Link AddLink(string name, string from, string to)
        {
            var vfrom = GetVertex(from);
            if (vfrom == null)
                throw new ArgumentException("From-vertice with given name not exists in graph");
            var vto = GetVertex(to);
            if (vto == null)
                throw new ArgumentException("To-vertice with given name not exists in graph");
            return AddLink(name, vfrom, vto);
        }

        /// <summary>
        /// Get all links of graph
        /// </summary>
        /// <returns></returns>
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
        /// <param name="vertex">Vertex name to find links</param>
        /// <returns>Returns array of links from given vertex</returns>
        public Link[] GetVertexFromLinks(string name)
        {
            var vertex = GetVertex(name);
            if (vertex == null)
                throw new ArgumentException("Vertex with given name not exists in graph");
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
        /// <param name="vertex">Vertex name to find links</param>
        /// <returns>Returns array of links to given vertex</returns>
        public Link[] GetVertexToLinks(string name)
        {
            var vertex = GetVertex(name);
            if (vertex == null)
                throw new ArgumentException("Vertex with given name not exists in graph");
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
                throw new ArgumentException("Vertex with given name not exists in graph");
            return GetVertexLinks(vertex);
        }

        public override string ToString()
        {
            return String.Format("<Graph '{0}'>", Name);
        }
    }
}