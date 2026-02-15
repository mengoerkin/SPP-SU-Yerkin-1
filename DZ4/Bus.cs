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
