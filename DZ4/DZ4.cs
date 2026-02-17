public interface IVehicle
{
    void Drive();
    void Refuel();
}

public class Car : IVehicle
{
    private string brand;
    private string model;
    private string fuelType;

    public Car(string brand, string model, string fuelType)
    {
        this.brand = brand;
        this.model = model;
        this.fuelType = fuelType;
    }

    public void Drive()
    {
        Console.WriteLine($"Car {brand} {model} is driving.");
    }

    public void Refuel()
    {
        Console.WriteLine($"Refueling car with {fuelType}.");
    }
}

public class Motorcycle : IVehicle
{
    private string type;
    private int engineCapacity;

    public Motorcycle(string type, int engineCapacity)
    {
        this.type = type;
        this.engineCapacity = engineCapacity;
    }

    public void Drive()
    {
        Console.WriteLine($"{type} motorcycle with {engineCapacity}cc is driving.");
    }

    public void Refuel()
    {
        Console.WriteLine("Refueling motorcycle.");
    }
}

public class Truck : IVehicle
{
    private double loadCapacity;
    private int axles;

    public Truck(double loadCapacity, int axles)
    {
        this.loadCapacity = loadCapacity;
        this.axles = axles;
    }

    public void Drive()
    {
        Console.WriteLine($"Truck with {loadCapacity} tons capacity is driving.");
    }

    public void Refuel()
    {
        Console.WriteLine("Refueling truck.");
    }
}

public abstract class VehicleFactory
{
    public abstract IVehicle CreateVehicle();
}

public class CarFactory : VehicleFactory
{
    private string brand;
    private string model;
    private string fuelType;

    public CarFactory(string brand, string model, string fuelType)
    {
        this.brand = brand;
        this.model = model;
        this.fuelType = fuelType;
    }

    public override IVehicle CreateVehicle()
    {
        return new Car(brand, model, fuelType);
    }
}

public class MotorcycleFactory : VehicleFactory
{
    private string type;
    private int engineCapacity;

    public MotorcycleFactory(string type, int engineCapacity)
    {
        this.type = type;
        this.engineCapacity = engineCapacity;
    }

    public override IVehicle CreateVehicle()
    {
        return new Motorcycle(type, engineCapacity);
    }
}

public class TruckFactory : VehicleFactory
{
    private double loadCapacity;
    private int axles;

    public TruckFactory(double loadCapacity, int axles)
    {
        this.loadCapacity = loadCapacity;
        this.axles = axles;
    }

    public override IVehicle CreateVehicle()
    {
        return new Truck(loadCapacity, axles);
    }
}

public class Bus : IVehicle
{
    private int passengerCapacity;
    private string routeNumber;

    public Bus(int passengerCapacity, string routeNumber)
    {
        this.passengerCapacity = passengerCapacity;
        this.routeNumber = routeNumber;
    }

    public void Drive()
    {
        Console.WriteLine($"Bus on route {routeNumber} is driving.");
    }

    public void Refuel()
    {
        Console.WriteLine("Refueling bus.");
    }
}

public class BusFactory : VehicleFactory
{
    private int passengerCapacity;
    private string routeNumber;

    public BusFactory(int passengerCapacity, string routeNumber)
    {
        this.passengerCapacity = passengerCapacity;
        this.routeNumber = routeNumber;
    }

    public override IVehicle CreateVehicle()
    {
        return new Bus(passengerCapacity, routeNumber);
    }
}

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
