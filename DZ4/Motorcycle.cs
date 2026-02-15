public class Motorcycle implements IVehicle {

    private String type;
    private int engineCapacity;

    public Motorcycle(String type, int engineCapacity) {
        this.type = type;
        this.engineCapacity = engineCapacity;
    }

    @Override
    public void drive() {
        System.out.println(type + " motorcycle with " + engineCapacity + "cc is driving.");
    }

    @Override
    public void refuel() {
        System.out.println("Refueling motorcycle.");
    }
}
