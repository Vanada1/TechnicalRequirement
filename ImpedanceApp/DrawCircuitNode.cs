﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Impedance;

namespace ImpedanceApp
{
	/// <summary>
	/// Node class for drawing elements
	/// </summary>
	class DrawCircuitNode
	{
		/// <summary>
		/// Element width
		/// </summary>
		private readonly int _width = 50;

		/// <summary>
		/// Element height
		/// </summary>
		private readonly int _height = 50;

		/// <summary>
		/// Adding distance for correct display X coordinate
		/// </summary>
		private readonly int _addX = 5;

		/// <summary>
		/// Adding distance for correct display Y coordinate
		/// </summary>
		private readonly int _addY = 5;

		/// <summary>
		/// check start
		/// </summary>
		private bool _isStart = true;

		/// <summary>
		/// Adding distance for correct display Y coordinate
		/// </summary>
		private readonly Point _startPoint = new Point(0,0);

		/// <summary>
		/// Set and return <see cref="ElementPoint"/>
		/// </summary>
		public Point ElementPoint { get; set; }

		/// <summary>
		/// Set and return element size
		/// </summary>
		public Size Size { get; set; }

		/// <summary>
		/// Set and return connect position to the element of left side
		/// </summary>
		public Point ConnectToLeft { get; set; }

		/// <summary>
		/// Set and return connect position to the element of right side
		/// </summary>
		public Point ConnectToRight { get; set; }

		/// <summary>
		/// Set and return element parent 
		/// </summary>
		public DrawCircuitNode Parent { get; set; }

		/// <summary>
		/// Set and return element <see cref="SubNodes"/>
		/// </summary>
		public List<DrawCircuitNode> SubNodes { get; set; } =  new List<DrawCircuitNode>();

		/// <summary>
		/// Set and return element <see cref="ISegment"/>
		/// </summary>
		public ISegment Segment { get; set; }

		/// <summary>
		/// DrawCircuitNode constructor 
		/// </summary>
		/// <param name="parent">Node <see cref="Parent"/></param>
		/// <param name="segment">Segment of this node</param>
		public DrawCircuitNode(DrawCircuitNode parent, ISegment segment)
		{
			Parent = parent;
			Segment = segment;

			if (segment is Element)
			{
				Size = new Size(_width,_height);
			}
		}

		/// <summary>
		/// Calculate position for this Node
		/// </summary>
		public void CalculatePosition(Size prevPoint = new Size())
		{
			var startPoint = _startPoint;

			if (Parent != null)
			{
				startPoint = Parent.ElementPoint;
				if (Parent.Segment is ParallelCircuit)
				{
					startPoint.Y += prevPoint.Height;
				}
				else
				{
					startPoint.X += prevPoint.Width;
				}
			}

			ElementPoint = startPoint;

			if (Segment is Element) return;

			if(Segment.SubSegments.Count == 0) return;

			SubNodes[0].CalculatePosition();
			SubNodes[0].ElementPoint = Segment is ParallelCircuit ? 
				new Point(startPoint.X + prevPoint.Width, startPoint.Y ) :
				new Point(startPoint.X , startPoint.Y + prevPoint.Height);

			for (var i = 1; i < Segment.SubSegments.Count; i++)
			{
				var newSegmentSize = 
					new Size(SubNodes[i - 1].Size.Width * i, SubNodes[i - 1].Size.Height * i) 
					+ new Size(_addX * i, _addY * i);
				SubNodes[i].CalculatePosition(newSegmentSize);
				//if (Segment is ParallelCircuit && Segment.SubSegments.Count > 1)
				//{
				//	SubNodes[i].ElementPoint = new Point(startPoint.X + prevPoint.Width,
				//		SubNodes[i - 1].ElementPoint.Y + SubNodes[i - 1].Size.Height + _addY);
				//}
				//else
				//{
				//	SubNodes[i].ElementPoint = new Point(
				//		SubNodes[i - 1].ElementPoint.X + SubNodes[i - 1].Size.Width + _addX,
				//		startPoint.Y + prevPoint.Height);
				//}
			}

			Size = GetSizeSegment(this);

			if (_isStart)
			{
				_isStart = false;
				CalculatePosition();
			}
		}

		/// <summary>
		/// Calculates segment size
		/// </summary>
		/// <param name="node">Segment for calculation</param>
		/// <returns>Segment size</returns>
		private Size GetSizeSegment(DrawCircuitNode node)
		{
			Size segmentSize;

			switch (node.Segment)
			{
				case Element _:
				{
					segmentSize = node.Size;
					break;
				}
				case ParallelCircuit _:
				{
					var startSegment = node.SubNodes[0].ElementPoint.Y;
					var endSegment = node.SubNodes[SubNodes.Count - 1].ElementPoint.Y;
					var height = Math.Abs(endSegment - startSegment) + _height;
					segmentSize = new Size(FindMaxWidth(node), height);
					break;
				}
				default:
				{
					var startSegment = node.SubNodes[0].ElementPoint.X;
					var endSegment = node.SubNodes[SubNodes.Count - 1].ElementPoint.X;
					var width = Math.Abs(endSegment - startSegment) + _width;
					segmentSize = new Size(width, FindMaxHeight(node));
					break;
				}
			}

			return segmentSize;
		}

		/// <summary>
		/// Finds the maximum Width of an segment
		/// </summary>
		/// <param name="node">The element in which the search occurs</param>
		/// <returns>Maximum Width</returns>
		private static int FindMaxWidth(DrawCircuitNode node)
		{
			var maxWidth = node.SubNodes[0].Size.Width;

			foreach (var subNode in node.SubNodes)
			{
				if (maxWidth < subNode.Size.Width)
				{
					maxWidth = subNode.Size.Width;
				}
			}
			
			return maxWidth;
		}

		/// <summary>
		/// Finds the maximum Height of an segment
		/// </summary>
		/// <param name="node">The element in which the search occurs</param>
		/// <returns>Maximum Height</returns>
		private static int FindMaxHeight(DrawCircuitNode node)
		{
			var maxHeight = node.SubNodes[0].Size.Height;

			foreach (var subNode in node.SubNodes)
			{
				if (maxHeight < subNode.Size.Height)
				{
					maxHeight = subNode.Size.Height;
				}
			}

			return maxHeight;
		}
	}
}
