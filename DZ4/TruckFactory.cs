public class TruckFactory extends VehicleFactory {

    private double loadCapacity;
    private int axles;

    public TruckFactory(double loadCapacity, int axles) {
        this.loadCapacity = loadCapacity;
        this.axles = axles;
    }

    @Override
    public IVehicle createVehicle() {
        return new Truck(loadCapacity, axles);
    }
}
