public class BusFactory extends VehicleFactory {

    private int passengerCapacity;
    private String routeNumber;

    public BusFactory(int passengerCapacity, String routeNumber) {
        this.passengerCapacity = passengerCapacity;
        this.routeNumber = routeNumber;
    }

    @Override
    public IVehicle createVehicle() {
        return new Bus(passengerCapacity, routeNumber);
    }
}
