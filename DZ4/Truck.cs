public class Truck implements IVehicle {

    private double loadCapacity;
    private int axles;

    public Truck(double loadCapacity, int axles) {
        this.loadCapacity = loadCapacity;
        this.axles = axles;
    }

    @Override
    public void drive() {
        System.out.println("Truck with " + loadCapacity + " tons capacity is driving.");
    }

    @Override
    public void refuel() {
        System.out.println("Refueling truck.");
    }
}
