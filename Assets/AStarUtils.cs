using Pathfinding;
using System;
using System.Collections.Generic;

public class NetNode : GraphNode
{
    public Connection[] Connections;

    public NetNode(AstarPath astar) : base(astar)
    {
    }

    public override void GetConnections(Action<GraphNode> action)
    {
        if (Connections is null) return;
        foreach (var connection in Connections)
        {
            action(connection.node);
        }
    }

    public override void AddConnection(GraphNode node, uint cost)
    {
        if (node is null) throw new ArgumentNullException();


        if (Connections != null)
        {
            for (var i = 0; i < Connections.Length; i++)
            {
                if (Connections[i].node != node) continue;
                Connections[i].cost = cost;
                return;
            }
        }

        var connLength = Connections?.Length ?? 0;

        var newconns = new Connection[connLength + 1];
        for (var i = 0; i < connLength; i++)
        {
            if (Connections != null) newconns[i] = Connections[i];
        }

        newconns[connLength] = new Connection(node, cost);

        Connections = newconns;
    }

    public override void RemoveConnection(GraphNode node)
    {
        if (Connections is null) return;

        for (var i = 0; i < Connections.Length; i++)
        {
            if (Connections[i].node != node) continue;
            var connLength = Connections.Length;

            var newconns = new Connection[connLength - 1];
            for (var j = 0; j < i; j++)
            {
                newconns[j] = Connections[j];
            }
            for (var j = i + 1; j < connLength; j++)
            {
                newconns[j - 1] = Connections[j];
            }

            Connections = newconns;
            AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
            return;
        }
    }

    public override void ClearConnections(bool alsoReverse)
    {
        if (alsoReverse && Connections != null)
        {
            for (var i = 0; i < Connections.Length; i++)
            {
                Connections[i].node.RemoveConnection(this);
            }
        }

        Connections = null;
    }

    public override void Open(Path path, PathNode pathNode, PathHandler handler)
    {
        if (Connections == null) return;

        for (var i = 0; i < Connections.Length; i++)
        {
            var other = Connections[i].node;

            if (!path.CanTraverse(other)) continue;
            var pathOther = handler.GetPathNode(other);

            if (pathOther.pathID != handler.PathID)
            {
                pathOther.parent = pathNode;
                pathOther.pathID = handler.PathID;

                pathOther.cost = Connections[i].cost;

                pathOther.H = path.CalculateHScore(other);
                pathOther.UpdateG(path);

                handler.heap.Add(pathOther);
            }
            else
            {
                //If not we can test if the path from this node to the other one is a better one then the one already used
                var tmpCost = Connections[i].cost;

                if (pathNode.G + tmpCost + path.GetTraversalCost(other) >= pathOther.G) continue;
                pathOther.cost = tmpCost;
                pathOther.parent = pathNode;

                other.UpdateRecursiveG(path, pathOther, handler);
            }
        }
    }
}

public class NetGraph : NavGraph
{
    public NetNode[] nodes;

    public override void GetNodes(Action<GraphNode> action)
    {
        if (nodes == null) return;
        foreach (var node in nodes)
        {
            action(node);
        }
    }

    protected override IEnumerable<Progress> ScanInternal()
    {
        yield return new Progress(1.0f, "Loaded");
    }
}