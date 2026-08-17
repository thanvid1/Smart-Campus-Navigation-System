import React, { useEffect, useState } from "react";
import "./App.css";

function App() {
  const [graph, setGraph] = useState(null);
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [path, setPath] = useState([]);

  useEffect(() => {
    fetch("/graph.json")
      .then(res => res.json())
      .then(data => setGraph(data));
  }, []);

  // 🔥 Heuristic (Euclidean)
  const heuristic = (a, b) => {
    return Math.sqrt((a.x - b.x) ** 2 + (a.y - b.y) ** 2);
  };

  // 🔥 A* Algorithm
  const aStar = (startId, goalId) => {
    let openSet = [startId];
    let cameFrom = {};

    let gScore = {};
    let fScore = {};

    graph.nodes.forEach(n => {
      gScore[n.id] = Infinity;
      fScore[n.id] = Infinity;
    });

    gScore[startId] = 0;

    const getNode = id => graph.nodes.find(n => n.id === id);

    fScore[startId] = heuristic(getNode(startId), getNode(goalId));

    while (openSet.length > 0) {
      let current = openSet.reduce((a, b) =>
        fScore[a] < fScore[b] ? a : b
      );

      if (current === goalId) {
        let path = [];
        while (current) {
          path.unshift(current);
          current = cameFrom[current];
        }
        return path;
      }

      openSet = openSet.filter(n => n !== current);

      let neighbors = graph.edges.filter(e => e.from === current);

      for (let edge of neighbors) {
        let temp = gScore[current] + edge.weight;

        if (temp < gScore[edge.to]) {
          cameFrom[edge.to] = current;
          gScore[edge.to] = temp;
          fScore[edge.to] =
            temp + heuristic(getNode(edge.to), getNode(goalId));

          if (!openSet.includes(edge.to)) openSet.push(edge.to);
        }
      }
    }
    return [];
  };

  const handleFindPath = () => {
    if (start && end) {
      const result = aStar(start, end);
      setPath(result);
    }
  };

  if (!graph) return <div>Loading...</div>;

  return (
  <div className="container">
    <div className="title">Smart Campus Navigation</div>

    <div className="controls">
      <select onChange={e => setStart(e.target.value)}>
        <option>Select Start</option>
        {graph.nodes.map(n => (
          <option key={n.id}>{n.id}</option>
        ))}
      </select>

      <select onChange={e => setEnd(e.target.value)}>
        <option>Select Destination</option>
        {graph.nodes.map(n => (
          <option key={n.id}>{n.id}</option>
        ))}
      </select>

      <button onClick={handleFindPath}>Find Path</button>
    </div>

    <div className="map-container">
      <svg width="600" height="500">
        
        {/* Edges */}
        {graph.edges.map((e, i) => {
          const from = graph.nodes.find(n => n.id === e.from);
          const to = graph.nodes.find(n => n.id === e.to);

          const isPath =
            path.includes(e.from) &&
            path.includes(e.to) &&
            path.indexOf(e.to) - path.indexOf(e.from) === 1;

          return (
            <line
              key={i}
              x1={from.x * 40 + 300}
              y1={500 - from.y * 40}
              x2={to.x * 40 + 300}
              y2={500 - to.y * 40}
              stroke={isPath ? "#e74c3c" : "#bdc3c7"}
              strokeWidth={isPath ? 5 : 2}
              strokeLinecap="round"
            />
          );
        })}

        {/* Nodes */}
        {graph.nodes.map((n, i) => (
          <g key={i}>
            <circle
              cx={n.x * 40 + 300}
              cy={500 - n.y * 40}
              r="10"
              fill="#3498db"
            />
            <text
              x={n.x * 40 + 305}
              y={500 - n.y * 40 - 12}
            >
              {n.id}
            </text>
          </g>
        ))}
      </svg>
    </div>
  </div>
);
}
export default App;