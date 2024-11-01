import React, { useEffect, useRef, useState } from 'react';
import * as d3 from 'd3';

const Graph = () => {
  const [data, setData] = useState(null); // Holds the graph data
  const [failedNodes, setFailedNodes] = useState([]); // List of failed validation IDs
  const svgRef = useRef(null); // Reference to the SVG element

  // Fetch graph data on initial load
  useEffect(() => {
    fetch('http://localhost:5255/graph')
      .then(response => response.json())
      .then(graphData => setData(graphData));
  }, []);

  // Function to call validation endpoint
  const validateStars = async () => {
    const response = await fetch('http://localhost:5255/validateStars');
    const failedIds = await response.json(); // Expected to be an array of failed star URIs
    setFailedNodes(failedIds); // Update failed nodes, triggers re-render
  };

  function stripDatatype(literal) {
    const datatypeIndex = literal.indexOf('^^');
    return datatypeIndex === -1 ? literal : literal.substring(0, datatypeIndex);
  };

  function getNodeColor(type) {
    switch (type) {
      case 'http://www.w3.org/2002/07/owl#Class':
        return '#090c9b'; // Blue for owl:Class
      case 'http://www.w3.org/2002/07/owl#NamedIndividual':
        return '#ed7d3a'; // Orange for owl:NamedIndividual
      case 'http://www.w3.org/2002/07/owl#DatatypeProperty':
        return '#31e981'; // Green for owl:DatatypeProperty
      case 'http://www.w3.org/2002/07/owl#ObjectProperty':
        return '#9467bd'; // Purple for owl:ObjectProperty
      case 'http://www.w3.org/2000/01/rdf-schema#Literal':
        return '#6b818c'; // Grey for rdfs:Literal
      default:
        return '#d81e5b'; // Red for unknown
    }
  };
  

  // Render the D3 graph
  const renderGraph = (data, failedNodes) => {
    const width = window.innerWidth * 0.8;
    const height = window.innerHeight;

    const svg = d3.select(svgRef.current);
    svg.selectAll('*').remove(); // Clear any previous content
    svg.attr('width', width).attr('height', height);

    const simulation = d3.forceSimulation(data.nodes)
      .force("link", d3.forceLink(data.links).id(d => d.id).distance(100))
      .force("charge", d3.forceManyBody().strength(-200))
      .force("center", d3.forceCenter(width / 2, height / 2))
      .force("x", d3.forceX(width / 2).strength(0.05))
      .force("y", d3.forceY(height / 2).strength(0.05));

    const link = svg.selectAll('.link')
      .data(data.links)
      .enter()
      .append('line')
      .attr('class', 'link')
      .attr('stroke', '#999')
      .attr('stroke-opacity', 0.6)
      .attr('stroke-width', 2);

    const node = svg.selectAll('.node')
      .data(data.nodes)
      .enter()
      .append('circle')
      .attr('class', 'node')
      .attr('r', 10)
      .attr('fill', d => getNodeColor(d.type))
      .attr("stroke", d => failedNodes.includes(d.id) ? "#d81e5b" : "none")
      .attr("stroke-width", d => failedNodes.includes(d.id) ? 3 : 0)
      .call(drag(simulation)); // Apply drag behavior here

    const label = svg.selectAll('.label')
      .data(data.nodes)
      .enter()
      .append('text')
      .attr('class', 'label')
      .attr('text-anchor', 'middle')
      .attr('dy', -15)
      .text(d => stripDatatype(d.label))
      .style('font-size', '12px')
      .style('fill', '#333');

    const linkLabels = svg.selectAll('.link-label')
    .data(data.links)
    .enter()
    .append('text')
    .attr('class', 'link-label')
    .attr('text-anchor', 'middle')
    .attr('dy', -5) // Position slightly above the line
    .style('font-size', '10px')
    .style('fill', '#666')
    .text(d => d.label); // Use the label property for known predicates

    simulation.on('tick', () => {
      link
        .attr('x1', d => clamp(d.source.x, 0, width))
        .attr('y1', d => clamp(d.source.y, 0, height))
        .attr('x2', d => clamp(d.target.x, 0, width))
        .attr('y2', d => clamp(d.target.y, 0, height));

      node
        .attr('cx', d => clamp(d.x, 0, width))
        .attr('cy', d => clamp(d.y, 0, height));

      label
        .attr('x', d => clamp(d.x, 0, width))
        .attr('y', d => clamp(d.y, 0, height));

        linkLabels
        .attr('x', d => (d.source.x + d.target.x) / 2)
        .attr('y', d => (d.source.y + d.target.y) / 2);
    });

    function clamp(value, min, max) {
      return Math.max(min, Math.min(value, max));
    };
  
    // Drag functionality with width and height for clamping
    function drag(simulation) {
      return d3.drag()
        .on('start', (event, d) => {
          if (!event.active) simulation.alphaTarget(0.3).restart();
          d.fx = d.x;
          d.fy = d.y;
        })
        .on('drag', (event, d) => {
          d.fx = clamp(event.x, 0, width);
          d.fy = clamp(event.y, 0, height);
        })
        .on('end', (event, d) => {
          if (!event.active) simulation.alphaTarget(0);
          d.fx = null;
          d.fy = null;
        });
    }
};

  // Re-render the graph when data or failedNodes changes
  useEffect(() => {
    if (data) renderGraph(data, failedNodes);
  }, [data, failedNodes]);

  return (
    <div>
      <button onClick={validateStars}>Validate Stars</button>
      <svg ref={svgRef}></svg>
    </div>
  );
};

export default Graph;
