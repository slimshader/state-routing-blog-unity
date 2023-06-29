using Bravasoft.Functional;
using Bravasoft.Functional.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public sealed class Person
{
    public int Id;
    public int Age;
    public string Name { get; set; }
    public Option<string> MiddleName { get; set; }
}

// Option jest zawsze w 1 z 2 stanów:
// Some(value)
// None



public class Main : MonoBehaviour
{
    public List<int> ints;
    public string number1;

    public UniOption<string> number2;


    Option<int> Parse(string input)
    {
        if (Int32.TryParse(input, out var result))
            return result;

        return Option.None;
    }

    Option<int> Divide(int a, int b)
    {
        if (b == 0)
            return Option.None;

        return a / b;
    }
    public void Start()
    {
        // base
        //var firstThree = ints.TryFirst(x => x % 3 == 0);
        //var firstFive = ints.TryFirst(x => x % 5 == 0);

        //if (firstThree.TryGetValue(out var three) && firstFive.TryGetValue(out var five))
        //{
        //    var sum = five + three;

        //    Debug.Log($"Sum is {sum}");
        //}
        //else
        //{
        //    Debug.Log($"Sum impossible");
        //}


        //var three = ints.TryFirst(x => x % 3 == 0).IfNone(3);
        //var firstFive = ints.TryFirst(x => x % 5 == 0);

        //if (firstFive.TryGetValue(out var five))
        //{
        //    var sum = five + three;

        //    Debug.Log($"Sum is {sum}");
        //}
        //else
        //{
        //    Debug.Log($"Sum impossible");
        //}


        // LINQ
        //var firstThree = ints.TryFirst(x => x % 3 == 0);
        //var firstFive = ints.TryFirst(x => x % 5 == 0);

        //var optSum = from three in firstThree
        //            from five in firstFive
        //            select three + five;

        //foreach (var sum in optSum.ToEnumerable()) 
        //{
        //    Debug.Log($"Sum is {sum}");
        //};

        //if (optSum.TryGetValue(out var sum))
        //{
        //    Debug.Log($"Sum is {sum}");
        //}
        //else
        //{
        //    Debug.Log($"Sum impossible");
        //}

        //var optPerson = GetPersonById(1);

        //if (optPerson.TryGetValue(out var person))
        //{
        //    PrintPersonInfo(person);
        //}

        var div = from n1 in Parse(number1)
                  from u2 in number2.Option
                  from n2 in Parse(u2)
                  from result in Divide(n1, n2)
                  select result;



        Debug.Log($"Result: {div}");
    }

    void PrintPersonInfo(Person person)
    {
        Debug.Log($"Person's age: {person.Age}");
    }

    private Option<Person> GetPersonById(int id)
    {
        if (id == 0)
            return Option.None;

        return new Person() { Id = id, Age = id, Name = $"Name {id}" };
    }

    private void Update()
    {
        var optCube = from hit in Bravasoft.Functional.Unity.Physics.TryRaycast(Camera.main.ScreenPointToRay(Input.mousePosition))
                      where hit.transform.name == "Cube1"
                      from c in hit.transform.TryGetComponent<Cube>()                      
                      select c;

        if (optCube.TryGetValue(out var cube))
        {
            cube.In();
        }
    }
}
