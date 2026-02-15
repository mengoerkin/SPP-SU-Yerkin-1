public class Bus implements IVehicle {

    private int passengerCapacity;
    private String routeNumber;

    public Bus(int passengerCapacity, String routeNumber) {
        this.passengerCapacity = passengerCapacity;
        this.routeNumber = routeNumber;
    }

    @Override
    public void drive() {
        System.out.println("Bus on route " + routeNumber + " is driving.");
    }

    @Override
    public void refuel() {
        System.out.println("Refueling bus.");
    }
}
