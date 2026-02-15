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
