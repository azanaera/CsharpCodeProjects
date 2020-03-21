using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            //1st example

            var cars = ProcessCars("fuel.csv");
            //var manufacturers = ProcessManufacturers("manufacturers.csv");


            //    foreach (var item in collection)
            //{

            //}
            //var document = new XDocument();
            //var cars = new XElement("Cars");

            //foreach (var record in records)
            //{
            //    var name = new XElement("Name", record.Name);
            //    var combined = new XElement("Combined", record.Combined);
            //    var car = new XElement("Car");
            //    car.Add(name);
            //    car.Add(combined);
            //    cars.Add(car);
            //}


            //2nd example

            //foreach (var record in records)
            //{
            //    var car = new XElement("Car",
            //        new XElement("Name", record.Name),
            //        new XElement("Combined", record.Combined),
            //        new XElement("Manufacturer", record.Manufacturer));

            //    cars.Add(car);
            //}

            //3 Linq XML
            //var elements =
            //    from record in records
            //    select new XElement("Car",
            //        new XElement("Name", record.Name),
            //        new XElement("Combined", record.Combined),
            //        new XElement("Manufacturer", record.Manufacturer));

            //cars.Add(elements);
            //document.Add(cars);
            //document.Save("fuel.xml");

            //extract method 
            CreateXML();
            QueryXML();
            Console.ReadKey();
        }

        private static void QueryXML()
        {
            //add namespace to every element

            var document = XDocument.Load("fuel.xml");
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            //document.Element("Cars").Elements("Car");

            var query =
                from element in document.Element(ns+"Cars")?.Elements(ex+"Car") 
                                                           ?? Enumerable.Empty<XElement>()
                //where element.Attribute("Manufacturer2")?.Value == "BMW"
                where element.Attribute("Manufacturer").Value == "BMW"
                select element.Attribute("Name").Value;
            foreach (var name in query)
            {
                Console.WriteLine(name);
            }
        }

        private static void CreateXML()
        {
            var records = ProcessCars("fuel.csv");
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            var document = new XDocument();
            var cars = new XElement(ns + "Cars",
                    from record in records
                        select new XElement(ex + "Car",
                                new XAttribute("Name", record.Name),
                                new XAttribute("Combined", record.Combined),
                                new XAttribute("Manufacturer", record.Manufacturer)));

            cars.Add(new XAttribute(XNamespace.Xmlns+"ex", ex));
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

            return query.ToList(); // concrete data
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
