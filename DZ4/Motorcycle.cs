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
