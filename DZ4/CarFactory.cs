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
