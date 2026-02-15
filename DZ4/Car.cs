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
