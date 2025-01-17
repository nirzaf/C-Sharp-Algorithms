﻿/***
 * The Undirected Sparse Graph Data Structure.
 * 
 * Definition:
 * A sparse graph is a graph G = (V, E) in which |E| = O(|V|).
 * A weighted graph is a graph where each edge has a weight (zero weights mean there is no edge).
 * 
 * An adjacency-list weighted graph representation. Shares a good deal of implemention details 
 * with the Undirected Sparse version (UndirectedSparseGraph<T>). Edges are instances of WeightedEdge<T> class. 
 * Implements both interfaces: IGraph<T> and IWeightedGraph<T>.
 */

using System;
using System.Collections.Generic;
using DataStructures.Common;
using DataStructures.Lists;

namespace DataStructures.Graphs;

public class UndirectedWeightedSparseGraph<T> : IGraph<T>, IWeightedGraph<T> where T : IComparable<T>
{
    /// <summary>
    /// INSTANCE VARIABLES
    /// </summary>
    private const long EMPTY_EDGE_VALUE = 0;
    protected virtual int _edgesCount { get; set; }
    protected virtual T _firstInsertedNode { get; set; }
    protected virtual Dictionary<T, DLinkedList<WeightedEdge<T>>> _adjacencyList { get; set; }


    /// <summary>
    /// CONSTRUCTOR
    /// </summary>
    public UndirectedWeightedSparseGraph() : this(10) { }

    public UndirectedWeightedSparseGraph(uint initialCapacity)
    {
        _edgesCount = 0;
        _adjacencyList = new Dictionary<T, DLinkedList<WeightedEdge<T>>>((int)initialCapacity);
    }


    /// <summary>
    /// Helper function. Returns edge object from source to destination, if exists; otherwise, null.
    /// </summary>
    protected virtual WeightedEdge<T> _tryGetEdge(T source, T destination)
    {
        var success = false;
        WeightedEdge<T> edge = null;

        var sourceToDestinationPredicate = new Predicate<WeightedEdge<T>>((item) => item.Source.IsEqualTo<T>(source) && item.Destination.IsEqualTo<T>(destination));
        var destinationToSourcePredicate = new Predicate<WeightedEdge<T>>((item) => item.Source.IsEqualTo<T>(destination) && item.Destination.IsEqualTo<T>(source));

        if(_adjacencyList.ContainsKey(source))
            success = _adjacencyList[source].TryFindFirst(sourceToDestinationPredicate, out edge);

        if(!success && _adjacencyList.ContainsKey(destination))
            _adjacencyList[destination].TryFindFirst(destinationToSourcePredicate, out edge);

        // Could return a null object.
        return edge;
    }

    /// <summary>
    /// Helper function. Checks if edge exist in graph.
    /// </summary>
    protected virtual bool _doesEdgeExist(T source, T destination)
    {
        return _tryGetEdge(source, destination) != null;
    }

    /// <summary>
    /// Helper function. Gets the weight of a undirected edge.
    /// Presumes edge does already exist.
    /// </summary>
    private long _getEdgeWeight(T source, T destination)
    {
        return _tryGetEdge(source, destination).Weight;
    }


    /// <summary>
    /// Returns true, if graph is undirected; false otherwise.
    /// </summary>
    public virtual bool IsDirected => false;

    /// <summary>
    /// Returns true, if graph is weighted; false otherwise.
    /// </summary>
    public virtual bool IsWeighted => true;

    /// <summary>
    /// Gets the count of vetices.
    /// </summary>
    public int EdgesCount => _edgesCount;

    /// <summary>
    /// Gets the count of edges.
    /// </summary>
    public int VerticesCount => _adjacencyList.Count;

    /// <summary>
    /// Returns the list of Vertices.
    /// </summary>
    public IEnumerable<T> Vertices
    {
        get
        {
            foreach (var vertex in _adjacencyList)
                yield return vertex.Key;
        }
    }


    IEnumerable<IEdge<T>> IGraph<T>.Edges => Edges;

    IEnumerable<IEdge<T>> IGraph<T>.IncomingEdges(T vertex)
    {
        return IncomingEdges(vertex);
    }

    IEnumerable<IEdge<T>> IGraph<T>.OutgoingEdges(T vertex)
    {
        return OutgoingEdges(vertex);
    }


    /// <summary>
    /// An enumerable collection of all weighted edges in Graph.
    /// </summary>
    public virtual IEnumerable<WeightedEdge<T>> Edges
    {
        get
        {
            var seen = new HashSet<KeyValuePair<T, T>>();

            foreach (var vertex in _adjacencyList)
            {
                foreach (var edge in vertex.Value)
                {
                    var incomingEdge = new KeyValuePair<T, T>(edge.Destination, edge.Source);
                    var outgoingEdge = new KeyValuePair<T, T>(edge.Source, edge.Destination);

                    if (seen.Contains(incomingEdge) || seen.Contains(outgoingEdge))
                        continue;
                    seen.Add(outgoingEdge);

                    yield return edge;
                }
            }//end-foreach
        }
    }

    /// <summary>
    /// Get all incoming weighted edges to a vertex
    /// </summary>
    public virtual IEnumerable<WeightedEdge<T>> IncomingEdges(T vertex)
    {
        if (!HasVertex(vertex))
            throw new KeyNotFoundException("Vertex doesn't belong to graph.");
            
        foreach(var edge in _adjacencyList[vertex])
            yield return new WeightedEdge<T>(edge.Destination, edge.Source, edge.Weight);
    }

    /// <summary>
    /// Get all outgoing weighted edges from a vertex.
    /// </summary>
    public virtual IEnumerable<WeightedEdge<T>> OutgoingEdges(T vertex)
    {
        if (!HasVertex(vertex))
            throw new KeyNotFoundException("Vertex doesn't belong to graph.");

        foreach(var edge in _adjacencyList[vertex])
            yield return edge;
    }


    /// <summary>
    /// Obsolete. Another AddEdge function is implemented with a weight parameter.
    /// </summary>
    [Obsolete("Use the AddEdge method with the weight parameter.")]
    public bool AddEdge(T source, T destination)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Connects two vertices together with a weight, in the direction: first->second.
    /// </summary>
    public bool AddEdge(T source, T destination, long weight)
    {
        // Check existence of nodes, the validity of the weight value, and the non-existence of edge
        if (weight == EMPTY_EDGE_VALUE)
            return false;
        if (!HasVertex(source) || !HasVertex(destination))
            return false;
        if (_doesEdgeExist(source, destination))
            return false;

        // Add edge from source to destination
        var sourceDdge = new WeightedEdge<T>(source, destination, weight);
        var destinationEdge = new WeightedEdge<T>(destination, source, weight);

        _adjacencyList[source].Append(sourceDdge);
        _adjacencyList[destination].Append(destinationEdge);

        // Increment edges count
        ++_edgesCount;

        return true;
    }

    /// <summary>
    /// Removes edge, if exists, from source to destination.
    /// </summary>
    public virtual bool RemoveEdge(T source, T destination)
    {
        // Check existence of nodes and non-existence of edge
        if (!HasVertex(source) || !HasVertex(destination))
            return false;

        WeightedEdge<T> edge1, edge2;

        var sourceToDestinationPredicate = new Predicate<WeightedEdge<T>>((item) => item.Source.IsEqualTo<T>(source) && item.Destination.IsEqualTo<T>(destination));
        _adjacencyList[source].TryFindFirst(sourceToDestinationPredicate, out edge1);

        var destinationToSourcePredicate = new Predicate<WeightedEdge<T>>((item) => item.Source.IsEqualTo<T>(destination) && item.Destination.IsEqualTo<T>(source));
        _adjacencyList[destination].TryFindFirst(destinationToSourcePredicate, out edge2);

        // If edge doesn't exist, return false
        if (edge1 == null && edge2 == null)
            return false;

        // If edge exists in the source neighbors, remove it
        if (edge1 != null)
            _adjacencyList[source].Remove(edge1);

        // If edge exists in the destination neighbors, remove it.
        if(edge2 != null)
            _adjacencyList[destination].Remove(edge2);

        // Decrement the edges count
        --_edgesCount;

        return true;
    }

    public bool UpdateEdgeWeight(T source, T destination, long weight)
    {
        // Check existence of vertices and validity of the weight value
        if (weight == EMPTY_EDGE_VALUE)
            return false;
        if (!HasVertex(source) || !HasVertex(destination))
            return false;

        // Status flag of updating an edge
        var status = false;

        // Check the source neighbors
        foreach (var edge in _adjacencyList[source])
        {
            if (edge.Destination.IsEqualTo(destination))
            {
                edge.Weight = weight;
                status |= true;
                break;
            }
        }

        // Check the destination neighbors
        foreach (var edge in _adjacencyList[destination])
        {
            if (edge.Destination.IsEqualTo(source))
            {
                edge.Weight = weight;
                status |= true;
                break;
            }
        }

        return status;
    }

    /// <summary>
    /// Get edge object from source to destination.
    /// </summary>
    public virtual WeightedEdge<T> GetEdge(T source, T destination)
    {
        if (!HasVertex(source) || !HasVertex(destination))
            throw new KeyNotFoundException("Either one of the vertices or both of them don't exist.");

        var edge = _tryGetEdge(source, destination);

        // Check the existence of edge
        if (edge == null)
            throw new Exception("Edge doesn't exist.");

        // Try get edge
        return edge;
    }

    /// <summary>
    /// Returns the edge weight from source to destination.
    /// </summary>
    public virtual long GetEdgeWeight(T source, T destination)
    {
        return GetEdge(source, destination).Weight;
    }

    /// <summary>
    /// Add a collection of vertices to the graph.
    /// </summary>
    public virtual void AddVertices(IList<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException();

        foreach (var vertex in collection)
            AddVertex(vertex);
    }

    /// <summary>
    /// Add vertex to the graph
    /// </summary>
    public virtual bool AddVertex(T vertex)
    {
        if (_adjacencyList.ContainsKey(vertex))
            return false;

        if (_adjacencyList.Count == 0)
            _firstInsertedNode = vertex;

        _adjacencyList.Add(vertex, new DLinkedList<WeightedEdge<T>>());

        return true;
    }

    /// <summary>
    /// Removes the specified vertex from graph.
    /// </summary>
    public virtual bool RemoveVertex(T vertex)
    {
        // Check existence of vertex
        if (!_adjacencyList.ContainsKey(vertex))
            return false;

        // Remove vertex from graph
        _adjacencyList.Remove(vertex);

        // Remove destination edges to this vertex
        foreach (var adjacent in _adjacencyList)
        {
            var edge = _tryGetEdge(adjacent.Key, vertex);

            if (edge != null)
            {
                adjacent.Value.Remove(edge);
                --_edgesCount;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks whether there is an edge from source to destination.
    /// </summary>
    public virtual bool HasEdge(T source, T destination)
    {
        return _adjacencyList.ContainsKey(source) && _adjacencyList.ContainsKey(destination) && _doesEdgeExist(source, destination);
    }

    /// <summary>
    /// Checks whether a vertex exists in the graph
    /// </summary>
    public virtual bool HasVertex(T vertex)
    {
        return _adjacencyList.ContainsKey(vertex);
    }

    /// <summary>
    /// Returns the neighbours doubly-linked list for the specified vertex.
    /// </summary>
    public virtual DLinkedList<T> Neighbours(T vertex)
    {
        if (!HasVertex(vertex))
            return null;

        var neighbors = new DLinkedList<T>();
        var adjacents = _adjacencyList[vertex];

        foreach (var adjacent in adjacents)
            neighbors.Append(adjacent.Destination);

        return neighbors;
    }

    /// <summary>
    /// Returns the neighbours of a vertex as a dictionary of nodes-to-weights.
    /// </summary>
    public Dictionary<T, long> NeighboursMap(T vertex)
    {
        if (!HasVertex(vertex))
            return null;

        var neighbors = _adjacencyList[vertex];
        var map = new Dictionary<T, long>(neighbors.Count);

        foreach (var adjacent in neighbors)
            map.Add(adjacent.Destination, adjacent.Weight);

        return map;
    }

    /// <summary>
    /// Returns the degree of the specified vertex.
    /// </summary>
    public virtual int Degree(T vertex)
    {
        if (!HasVertex(vertex))
            throw new KeyNotFoundException();

        return _adjacencyList[vertex].Count;
    }

    /// <summary>
    /// Returns a human-readable string of the graph.
    /// </summary>
    public virtual string ToReadable()
    {
        string output = string.Empty;

        foreach (var node in _adjacencyList)
        {
            var adjacents = string.Empty;

            output = String.Format("{0}\r\n{1}: [", output, node.Key);

            foreach (var adjacentNode in node.Value)
                adjacents = String.Format("{0}{1}({2}), ", adjacents, adjacentNode.Destination, adjacentNode.Weight);

            if (adjacents.Length > 0)
                adjacents = adjacents.TrimEnd(new char[] { ',', ' ' });

            output = String.Format("{0}{1}]", output, adjacents);
        }

        return output;
    }

    /// <summary>
    /// A depth first search traversal of the graph starting from the first inserted node.
    /// Returns the visited vertices of the graph.
    /// </summary>
    public virtual IEnumerable<T> DepthFirstWalk()
    {
        return DepthFirstWalk(_firstInsertedNode);
    }

    /// <summary>
    /// A depth first search traversal of the graph, starting from a specified vertex.
    /// Returns the visited vertices of the graph.
    /// </summary>
    public virtual IEnumerable<T> DepthFirstWalk(T source)
    {
        // Check for existence of source
        if (VerticesCount == 0)
            return new ArrayList<T>(0);
        if (!HasVertex(source))
            throw new KeyNotFoundException("The source vertex doesn't exist.");

        var visited = new HashSet<T>();
        var stack = new Lists.Stack<T>();
        var listOfNodes = new ArrayList<T>(VerticesCount);

        stack.Push(source);

        while (!stack.IsEmpty)
        {
            var current = stack.Pop();

            if (!visited.Contains(current))
            {
                listOfNodes.Add(current);
                visited.Add(current);

                foreach (var adjacent in Neighbours(current))
                    if (!visited.Contains(adjacent))
                        stack.Push(adjacent);
            }
        }

        return listOfNodes;
    }

    /// <summary>
    /// A breadth first search traversal of the graphstarting from the first inserted node.
    /// Returns the visited vertices of the graph.
    /// </summary>
    public virtual IEnumerable<T> BreadthFirstWalk()
    {
        return BreadthFirstWalk(_firstInsertedNode);
    }

    /// <summary>
    /// A breadth first search traversal of the graph, starting from a specified vertex.
    /// Returns the visited vertices of the graph.
    /// </summary>
    public virtual IEnumerable<T> BreadthFirstWalk(T source)
    {
        // Check for existence of source
        if (VerticesCount == 0)
            return new ArrayList<T>(0);
        if (!HasVertex(source))
            throw new KeyNotFoundException("The source vertex doesn't exist.");

        var visited = new HashSet<T>();
        var queue = new Lists.Queue<T>();
        var listOfNodes = new ArrayList<T>(VerticesCount);

        listOfNodes.Add(source);
        visited.Add(source);

        queue.Enqueue(source);

        while (!queue.IsEmpty)
        {
            var current = queue.Dequeue();
            var neighbors = Neighbours(current);

            foreach (var adjacent in neighbors)
            {
                if (!visited.Contains(adjacent))
                {
                    listOfNodes.Add(adjacent);
                    visited.Add(adjacent);
                    queue.Enqueue(adjacent);
                }
            }
        }

        return listOfNodes;
    }

    /// <summary>
    /// Clear this graph.
    /// </summary>
    public virtual void Clear()
    {
        _edgesCount = 0;
        _adjacencyList.Clear();
    }
}