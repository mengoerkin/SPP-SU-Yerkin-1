using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Choose vehicle type:");
        Console.WriteLine("car / motorcycle / truck / bus");

        string choice = Console.ReadLine()?.ToLower();
        VehicleFactory factory = null;

        switch (choice)
        {
            case "car":
                Console.Write("Brand: ");
                string brand = Console.ReadLine();

                Console.Write("Model: ");
                string model = Console.ReadLine();

                Console.Write("Fuel type: ");
                string fuel = Console.ReadLine();

                factory = new CarFactory(brand, model, fuel);
                break;

            case "motorcycle":
                Console.Write("Type: ");
                string type = Console.ReadLine();

                Console.Write("Engine capacity: ");
                int capacity = int.Parse(Console.ReadLine());

                factory = new MotorcycleFactory(type, capacity);
                break;

            case "truck":
                Console.Write("Load capacity: ");
                double load = double.Parse(Console.ReadLine());

                Console.Write("Axles: ");
                int axles = int.Parse(Console.ReadLine());

                factory = new TruckFactory(load, axles);
                break;

            case "bus":
                Console.Write("Passenger capacity: ");
                int passengers = int.Parse(Console.ReadLine());

                Console.Write("Route number: ");
                string route = Console.ReadLine();

                factory = new BusFactory(passengers, route);
                break;

            default:
                Console.WriteLine("Invalid type!");
                return;
        }

        IVehicle vehicle = factory.CreateVehicle();
        vehicle.Drive();
        vehicle.Refuel();
    }
}
