using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Day16ProboscideaVolcanium;

internal sealed class Map
{
    private const string StartingNodeLabel = "AA";

    private static readonly Regex lineRegex = new(
        @"Valve (\w+) has flow rate=(\d+); tunnels? leads? to valves? (?:(\w+)(?:,\s)?)+", 
        RegexOptions.Compiled);

    public List<Node> NodeList { get; } = new List<Node>();
    public Dictionary<string, Node> LabelToNodeMap { get; } = new Dictionary<string, Node>();

    public bool PrintDebugMessages { get; set; }

    private Node StartNode { get; set; } = null!;
    private List<int> ShortestPathCostsAscending { get; } = new List<int>();
    private int ShortestPathCost { get; set; }
    private List<int> UpperLimitsOnRemainingFlowRate { get; } = new List<int>();
    private List<Node> NodesWithDescendingFlowRates { get; } = new List<Node>();
    private int MaxNodeFlowRate { get; set; }
    private int IterationCount { get; set; } = 0;

    public int TimeLimit { get; private set; }
    public int ActorCount { get; private set; }
    public int? MaxFlowRate { get; private set; }
    public ulong MaxFlowRateOpenValves { get; private set; }

    public void LoadInput(string path)
    {
        var inputLines = File.ReadAllLines(path);
        foreach (var line in inputLines)
        {
            ParseLine(line);
        }

        StartNode = NodeList.First(n => n.Label == StartingNodeLabel);

        if (PrintDebugMessages)
        {
            Console.WriteLine($"Input loaded from {path}:");
            Console.WriteLine(this);
            Console.WriteLine();
        }
    }

    public void CalculateMaxFlowRate(
        int timeLimit,
        int actorCount)
    {
        TimeLimit = timeLimit;
        ActorCount = actorCount;
        MaxFlowRate = null;
        IterationCount = 0;
        MaxFlowRateOpenValves = 0;
        PrepareCalculateMaxFlowRate();

        Debug.Assert(StartNode.FlowRate == 0);
        ulong openValves = RecordOpenValve(0ul, StartNode);

        if (ActorCount == 1)
        {
            var path = new Stack<Node>();
            path.Push(StartNode);
            CalculateMaxFlowRateOneActorRec(StartNode, timeLimit, path, 0, openValves);
        }
        else if (ActorCount == 2)
        {
            CalculateMaxFlowRateTwoActorsRec(
                new ActorState("me", StartNode, TimeLimit),
                new ActorState("elephant", StartNode, TimeLimit),
                0, 
                openValves);
        }

        if (PrintDebugMessages)
        {
            Console.WriteLine($"The best flow rate is {MaxFlowRate}, " +
                $"with open valves {OpenValvesToString(MaxFlowRateOpenValves)}, " +
                $"found after {IterationCount} iterations.");
        }
    }

    public void MeasureDistances()
    {
        foreach (var node in NodeList)
        {
            var paths = node.Paths;
            paths.Clear();
            paths.Capacity = NodeList.Count;
            for (int i = 0; i < NodeList.Count; i++)
            {
                paths.Add(null!);
            }
        }
        foreach (var node in NodeList)
        {
            MeasureDistances(node);
        }
    }

    public void DropEmptyTunnels()
    {
        if (PrintDebugMessages)
        {
            Console.WriteLine("Dropping empty tunnels.");
        }

        var droppedLabels = new HashSet<string>(NodeList.Count);
        foreach (var fromNode in NodeList)
        {
            if (droppedLabels.Contains(fromNode.Label))
            {
                continue;
            }

            for (int fromEdgeIndex = 0; fromEdgeIndex < fromNode.Edges.Count; fromEdgeIndex++)
            {
                var fromEdge = fromNode.Edges[fromEdgeIndex];
                var toNode = fromEdge.ToNode;
                if (toNode.FlowRate > 0 
                    || toNode.Edges.Count > 2 
                    || toNode.Label == StartingNodeLabel)
                {
                    continue;
                }

                droppedLabels.Add(toNode.Label);
                if (PrintDebugMessages)
                {
                    Console.WriteLine($"\tDropping {toNode.Label} with {toNode.Edges.Count} edge(s).");
                }
                if (toNode.Edges.Count == 1)
                {
                    fromNode.Edges.RemoveAt(fromEdgeIndex);
                    fromEdgeIndex--;
                }
                else
                {
                    var backEdge = toNode.Edges.First(e => e.ToNode == fromNode);
                    var notBackEdge = toNode.Edges.First(e => e.ToNode != fromNode);
                    var newToNode = notBackEdge.ToNode;
                    fromNode.Edges[fromEdgeIndex] = new Edge
                    {
                        Cost = fromEdge.Cost + notBackEdge.Cost,
                        ToNode = newToNode
                    };
                    fromEdgeIndex--;
                    var backEdgeFromNewToNode = newToNode.Edges.First(e => e.ToNode == toNode);
                    int backEdgeFromNewToNodeIndex = newToNode.Edges.IndexOf(backEdgeFromNewToNode);
                    newToNode.Edges[backEdgeFromNewToNodeIndex] = new Edge
                    {
                        Cost = backEdgeFromNewToNode.Cost + backEdge.Cost,
                        ToNode = fromNode
                    };
                }
            }
        }

        NodeList.RemoveAll(n => droppedLabels.Contains(n.Label));
        for (int i = 0; i < NodeList.Count; i++)
        {
            NodeList[i].Index = i;
        }

        if (PrintDebugMessages)
        {
            Console.WriteLine(this);
            Console.WriteLine();
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var node in NodeList)
        {
            result.AppendLine(node.ToString());
        }
        
        return result.ToString();
    }

    private void CalculateMaxFlowRateTwoActorsRec(
        ActorState actorToMove,
        ActorState? otherActor,
        int flowRate,
        ulong openValves)
    {
        int timeLeft = actorToMove.TimeLeft;
        var node = actorToMove.Node;
        
        bool foundNext = false;
        if (timeLeft >= ShortestPathCost + 2)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                IterationCount++;
                if (IsOpen(openValves, i))
                {
                    continue;
                }

                int distance = node.Distances[i];
                int newTimeLeft = timeLeft - (distance + 1);
                if (newTimeLeft < 1)
                {
                    continue;
                }
                if (newTimeLeft < TimeLimit / 2
                    && MaxFlowRate.HasValue
                    && MaxFlowRate.Value >= flowRate + UpperLimitsOnRemainingFlowRate[timeLeft])
                {
                    continue;
                }

                var nextNode = NodeList[i];
                foundNext = true;
                int newFlowRate = flowRate + newTimeLeft * nextNode.FlowRate;
                ulong newOpenValves = RecordOpenValve(openValves, nextNode);
                
                var newActorState = new ActorState(actorToMove.Name, nextNode, newTimeLeft);
                ActorState nextActorToMove;
                ActorState? nextOtherActor;
                if (otherActor == null || newTimeLeft > otherActor.TimeLeft)
                {
                    nextActorToMove = newActorState;
                    nextOtherActor = otherActor;
                }
                else
                {
                    nextActorToMove = otherActor;
                    nextOtherActor = newActorState;
                }
                CalculateMaxFlowRateTwoActorsRec(nextActorToMove, nextOtherActor, newFlowRate, newOpenValves);
            }
        }

        if (!foundNext)
        {
            if (otherActor != null)
            {
                CalculateMaxFlowRateTwoActorsRec(otherActor, null, flowRate, openValves);
            }
            else if (!MaxFlowRate.HasValue || flowRate > MaxFlowRate.Value)
            {
                MaxFlowRate = flowRate;
                MaxFlowRateOpenValves = openValves;

                if (PrintDebugMessages)
                {
                    Console.WriteLine($"Best flow rate so far is {MaxFlowRate}, " +
                        $"with open valves {OpenValvesToString(openValves)}.");
                }
            }
        }
    }

    private void CalculateMaxFlowRateOneActorRec(
        Node node,
        int timeLeft,
        Stack<Node> path,
        int flowRate,
        ulong openValves)
    {
        bool foundNext = false;
        if (timeLeft >= ShortestPathCost + 2)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                IterationCount++;
                if (IsOpen(openValves, i))
                {
                    continue;
                }

                int distance = node.Distances[i];
                int newTimeLeft = timeLeft - (distance + 1);
                if (newTimeLeft < 1)
                {
                    continue;
                }
                if (newTimeLeft < TimeLimit / 2
                    && MaxFlowRate.HasValue
                    && MaxFlowRate.Value >= flowRate + UpperLimitsOnRemainingFlowRate[timeLeft])
                {
                    continue;
                }

                var nextNode = NodeList[i];
                Node? isBlockedBy = null;
                foreach (var blocker in node.Blockers[i])
                {
                    if (IsOpen(openValves, blocker.Index))
                    {
                        continue;
                    }

                    if (blocker.FlowRate * newTimeLeft >= nextNode.FlowRate * (newTimeLeft - blocker.Distances[i]))
                    {
                        isBlockedBy = blocker;
                        break;
                    }
                }
                if (isBlockedBy != null)
                {
                    continue;
                }

                foundNext = true;
                path.Push(nextNode);
                int newFlowRate = flowRate + newTimeLeft * nextNode.FlowRate;
                ulong newOpenValves = RecordOpenValve(openValves, nextNode);
                CalculateMaxFlowRateOneActorRec(nextNode, newTimeLeft, path, newFlowRate, newOpenValves);
            }
        }

        if (!foundNext)
        {
            if (!MaxFlowRate.HasValue || flowRate > MaxFlowRate.Value)
            {
                MaxFlowRate = flowRate;
                MaxFlowRateOpenValves = openValves;

                if (PrintDebugMessages)
                {
                    Console.WriteLine($"Best flow rate so far is {MaxFlowRate}, " +
                        $"with open valves {OpenValvesToString(openValves)}.");
                }
            }
        }

        path.Pop();
    }

    private string OpenValvesToString(ulong openValvesToPrint)
    {
        var resultBuilder = new StringBuilder();
        for (int i = 0; i < NodeList.Count; i++)
        {
            if (IsOpen(openValvesToPrint, i))
            {
                if (resultBuilder.Length > 0)
                {
                    resultBuilder.Append(", ");
                }
                resultBuilder.Append(NodeList[i].Label);
            }
        }
        return resultBuilder.ToString();
    }

    private void PrepareCalculateMaxFlowRate()
    {
        PopulateShortestPathCosts();
        PopulateFlowRatesDescending();
        PopulateUpperLimitsOnRemainingFlowRate();
        FindPathBlockers();
    }

    private void FindPathBlockers()
    {
        foreach (var node in NodeList)
        {
            node.IsPotentialBlocker = (ShortestPathCost + 1) * node.FlowRate >= MaxNodeFlowRate;
        }

        foreach (var fromNode in NodeList)
        {
            var blockers = fromNode.Blockers;
            blockers.Clear();
            blockers.Capacity = NodeList.Count;
            for (int i = 0; i < NodeList.Count; i++)
            {
                blockers.Add(new List<Node>());
            }

            foreach (var toNode in NodeList)
            {
                var current = fromNode;
                while (current != toNode)
                {
                    var next = current.Paths[toNode.Index];
                    if (next == toNode)
                    {
                        break;
                    }

                    if (next.IsPotentialBlocker)
                    {
                        blockers[toNode.Index].Add(next);
                    }
                    current = next;
                }
            }

            if (PrintDebugMessages)
            {
                Console.WriteLine($"Blockers from {fromNode.Label}:");
                foreach (var toNode in NodeList)
                {
                    Console.WriteLine($"\tTo {toNode.Label}: {string.Join(", ", blockers[toNode.Index].Select(n => n.Label))}");
                }
            }
        }
    }

    private void PopulateShortestPathCosts()
    {
        ShortestPathCostsAscending.Clear();
        foreach (var node in NodeList)
        {
            foreach (var edge in node.Edges)
            {
                if (node.Index < edge.ToNode.Index)
                {
                    ShortestPathCostsAscending.Add(edge.Cost);
                }
            }
        }
        ShortestPathCostsAscending.Sort();
        ShortestPathCost = ShortestPathCostsAscending[0];
    }

    private void PopulateFlowRatesDescending()
    {
        NodesWithDescendingFlowRates.Clear();
        NodesWithDescendingFlowRates.Capacity = NodeList.Count;
        NodesWithDescendingFlowRates.AddRange(NodeList.OrderByDescending(n => n.FlowRate));
        MaxNodeFlowRate = NodesWithDescendingFlowRates[0].FlowRate;
    }

    private void PopulateUpperLimitsOnRemainingFlowRate()
    {
        UpperLimitsOnRemainingFlowRate.Clear();
        UpperLimitsOnRemainingFlowRate.Capacity = TimeLimit + 1;
        for (int timeLeft = 0; timeLeft <= TimeLimit; timeLeft++)
        {
            int time = 0;
            int flowRate = 0;
            for (int i = 0; i < NodeList.Count; i += ActorCount)
            {
                time += ShortestPathCostsAscending[i] + 1;
                if (time > timeLeft)
                {
                    break;
                }
                for (int j = i; j < i + ActorCount; j++)
                {
                    if (j == NodeList.Count)
                    {
                        break;
                    }
                    flowRate += NodesWithDescendingFlowRates[j].FlowRate * (timeLeft - time);
                }
            }
            UpperLimitsOnRemainingFlowRate.Add(flowRate);
        }
    }

    private static ulong RecordOpenValve(ulong openValves, Node node)
    {
        return openValves | (1ul << node.Index);
    }

    private static bool IsOpen(ulong openValves, int index)
    {
        return (openValves & (1ul << index)) != 0;
    }

    private void MeasureDistances(Node fromNode)
    {
        var visited = new BitArray(NodeList.Count);
        var distances = fromNode.Distances;
        distances.Clear();
        distances.Capacity = NodeList.Count;
        for (int i = 0; i < NodeList.Count; i++)
        {
            distances.Add(int.MaxValue);
        }
        distances[fromNode.Index] = 0;
        var queue = new PriorityQueue<Node, int>();
        queue.Enqueue(fromNode, 0);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (visited[node.Index])
            {
                continue;
            }
            visited[node.Index] = true;
            int distanceHere = distances[node.Index];
            foreach (var edge in node.Edges)
            {
                var nextNode = edge.ToNode;
                int oldDistance = distances[nextNode.Index];
                int newDistance = distanceHere + edge.Cost;
                if (newDistance >= oldDistance)
                {
                    continue;
                }
                distances[nextNode.Index] = newDistance;
                nextNode.Paths[fromNode.Index] = node;
                queue.Enqueue(nextNode, newDistance);
            }
        }

        if (PrintDebugMessages)
        {
            Console.WriteLine($"Distances from {fromNode.Label}:");
            foreach (var toNode in NodeList)
            {
                Console.WriteLine($"\tTo {toNode.Label}: {distances[toNode.Index]}; " +
                    $"last step from: {toNode.Paths[fromNode.Index]?.Label}");
            }
            Console.WriteLine();
        }
    }

    private void ParseLine(string line)
    {
        var match = lineRegex.Match(line);
        if (!match.Success)
        {
            Console.WriteLine(line);
            return;
        }

        int flowRate = int.Parse(match.Groups[2].Value);
        var node = GetNode(match.Groups[1].Value);
        node.FlowRate = flowRate;
        foreach (Capture capture in match.Groups[3].Captures)
        {
            string nextLabel = capture.Value;
            var nextNode = GetNode(nextLabel);
            var edge = new Edge
            {
                ToNode = nextNode
            };
            node.Edges.Add(edge);
        }
    }

    private Node GetNode(string label)
    {
        if (!LabelToNodeMap.TryGetValue(label, out var node))
        {
            node = new Node
            {
                Label = label,
                Index = NodeList.Count
            };
            LabelToNodeMap.Add(label, node);
            NodeList.Add(node);
        }
        return node;
    }

    private record ActorState(
        string Name,
        Node Node,
        int TimeLeft);
}
