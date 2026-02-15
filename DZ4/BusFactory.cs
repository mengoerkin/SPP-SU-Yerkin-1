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
