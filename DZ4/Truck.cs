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
