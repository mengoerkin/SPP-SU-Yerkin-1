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
