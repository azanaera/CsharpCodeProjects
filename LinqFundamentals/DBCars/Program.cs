﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateXml();
            //QueryXml();
            

            //IQueryables
            //Func<int, int> square = x => x * x;
            //Expression<Func<int, int, int>> add = (x, y) => x + y;
            //Func<int, int, int> addI = add.Compile();
            //Console.WriteLine(add); 
            //var result = addI(3, 5);

            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDB>());
            //InsertData();
            QueryData();
            Console.ReadKey();
        }

        private static void QueryData()
        {
            var db = new CarDB();
            db.Database.Log = Console.WriteLine;

            var query =
                //1st
                //from car in db.Cars
                //orderby car.Combined descending, car.Name ascending
                //select car;

                //2nd
                //db.Cars.Where(c => c.Manufacturer == "BMW")
                //.OrderByDescending(c => c.Combined)
                //.ThenBy(c => c.Name)
                //.Take(10)
                //.Select(c => new { Name = c.Name.Split(' '});
                //once u select .ToList() it becomes concrete data, not deferred execution


                //3rd
                //db.Cars.GroupBy(c => c.Manufacturer)
                //        .Select(g => new
                //        {
                //            Name = g.Key,
                //            Cars = g.OrderByDescending(c => c.Combined).Take(2)
                //        });

                //4th
                from car in db.Cars
                group car by car.Manufacturer into manufacturer
                select new
                {
                    Name = manufacturer.Key,
                    Cars = (from car in manufacturer
                            orderby car.Combined descending
                            select car).Take(2)
                };
            //1st
            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Name} : {car.Combined}");
            //}

            //2nd
            //foreach (var item in query)
            //{
            //    Console.WriteLine(item.Name);
            //}

            //3rd
            foreach (var group in query)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDB();
            db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
            }
            db.SaveChanges();

        }

        private static void QueryXml()
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = XDocument.Load("fuel.xml");

            var query =
                from element in document.Element(ns + "Cars")?.Elements(ex + "Car")
                                                       ?? Enumerable.Empty<XElement>()
                where element.Attribute("Manufacturer")?.Value == "BMW"
                select element.Attribute("Name").Value;

            foreach (var name in query)
            {
                Console.WriteLine(name);
            }
        }

        private static void CreateXml()
        {
            var records = ProcessCars("fuel.csv");

            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = new XDocument();
            var cars = new XElement(ns + "Cars",

                from record in records
                select new XElement(ex + "Car",
                                new XAttribute("Name", record.Name),
                                new XAttribute("Combined", record.Combined),
                                new XAttribute("Manufacturer", record.Manufacturer))
            );

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));

            document.Add(cars);
            document.Save("fuel.xml");
        }

        private static List<Car> ProcessCars(string path)
        {
            var query =

                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .ToCar();

            return query.ToList();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                   File.ReadAllLines(path)
                       .Where(l => l.Length > 1)
                       .Select(l =>
                       {
                           var columns = l.Split(',');
                           return new Manufacturer
                           {
                               Name = columns[0],
                               Headquarters = columns[1],
                               Year = int.Parse(columns[2])
                           };
                       });
            return query.ToList();
        }
    }

    public class CarStatistics
    {
        public CarStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;
        }
        
        public CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Combined;
            Max = Math.Max(Max, car.Combined);
            Min = Math.Min(Min, car.Combined);
            return this;
        }

        public CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }

        public int Max { get; set; }
        public int Min { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public double Average { get; set; }

    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}