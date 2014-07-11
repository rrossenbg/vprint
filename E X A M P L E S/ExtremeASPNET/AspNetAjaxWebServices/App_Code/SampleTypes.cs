using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Web.Script.Services;

//class App
//{
//    static void Main()
//    {
//        Person p = new Person();
//        p.Name = "Bob Smith";
//        p.Age = 33;
//        p.Married = true;

//        string serializedPerson = JavaScriptObjectSerializer.Serialize(p);
//        Console.WriteLine(serializedPerson);
//    }
//}

namespace MsdnMagazine.SampleTypes
{
    public class Person
    {
        private string _firstName;
        private string _lastName;
        private int _age;
        private bool _married;

        public bool Married
        {
            get { return _married; }
            set { _married = value; }
        }

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
    }

    public class Company
    {
        private string _name;
        private List<Person> _employees = new List<Person>();

        public List<Person> Employees
        {
            get { return _employees; }
        }
	
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }	
    }

    public class Company2
    {
        private string _name;
        private ArrayList _employees = new ArrayList();

        public ArrayList Employees
        {
            get { return _employees; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}