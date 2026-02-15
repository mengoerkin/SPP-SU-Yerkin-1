public class MotorcycleFactory extends VehicleFactory {

    private String type;
    private int engineCapacity;

    public MotorcycleFactory(String type, int engineCapacity) {
        this.type = type;
        this.engineCapacity = engineCapacity;
    }

    @Override
    public IVehicle createVehicle() {
        return new Motorcycle(type, engineCapacity);
    }
}
