using System;
using ExpectedObjects;
using Lab.Entities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAdvanceDesignTests
{
	public class Comparer : IComparer<Employee>
	{
		public Comparer(Func<Employee, string> firstKeySelector, Comparer<string> firstKeyComparer)
		{
			FirstKeySelector = firstKeySelector;
			FirstKeyComparer = firstKeyComparer;
		}

		public Func<Employee, string> FirstKeySelector { get; private set; }
		public Comparer<string> FirstKeyComparer { get; private set; }

		public int Compare(Employee x, Employee y)
		{
			return FirstKeyComparer.Compare(FirstKeySelector(x), FirstKeySelector(y));
		}
	}

	[TestFixture]
    public class JoeyOrderByTests
    {
	    [Test]
        public void orderBy_lastName_and_firstName()
        {
	        var employees = new[]
	        {
		        new Employee {FirstName = "Joey", LastName = "Wang"},
		        new Employee {FirstName = "Tom", LastName = "Li"},
		        new Employee {FirstName = "Joseph", LastName = "Chen"},
		        new Employee {FirstName = "Joey", LastName = "Chen"},
	        };

	        var actual = JoeyOrderBy(employees, new Comparer(employee => employee.LastName, Comparer<string>.Default), employee1 => employee1.FirstName, Comparer<string>.Default);

	        var expected = new[]
	        {
		        new Employee {FirstName = "Joey", LastName = "Chen"},
		        new Employee {FirstName = "Joseph", LastName = "Chen"},
		        new Employee {FirstName = "Tom", LastName = "Li"},
		        new Employee {FirstName = "Joey", LastName = "Wang"},
	        };

	        expected.ToExpectedObject().ShouldMatch(actual);
        }
		private IEnumerable<Employee> JoeyOrderBy(IEnumerable<Employee> employees,
			IComparer<Employee> comparer,
			Func<Employee, string> secondKeySelector,
			Comparer<string> secondKeyComparer)
        {

	        //bubble sort
	        var elements = employees.ToList();
	        while (elements.Any())
	        {
		        var minElement = elements[0];
		        var index = 0;
		        for (int i = 1; i < elements.Count; i++)
		        {
			        var employee = elements[i];
			        if (comparer.Compare(employee, minElement) < 0)
			        {
				        minElement = employee;
				        index = i;
			        }
			        else if (comparer.Compare(employee, minElement) == 0)
			        {
				        if (secondKeyComparer.Compare(secondKeySelector(employee), secondKeySelector(minElement)) < 0)
				        {
					        minElement = employee;
					        index = i;
				        }
			        }
		        }

		        elements.RemoveAt(index);
		        yield return minElement;
	        }
        }
    }
}