﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TZ1
{
	class Capacitor : IElement
	{
		private double _value;
		public string Name { get; set; }
		public double Value 
		{ 
			get => _value; 
			set
			{
				_value = value;
				Changed.Invoke(this, 
					nameof(Capacitor) + " value has been change");
			}
		}

		public event IElement.ValueChanged Changed;

		public Complex CalculateZ(double frequency)
		{
			double result = -1 / (2 * Math.PI * frequency * Value);
			return new Complex(0, result);
		}
	}
}
