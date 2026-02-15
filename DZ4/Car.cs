public class Car implements IVehicle {

    private String brand;
    private String model;
    private String fuelType;

    public Car(String brand, String model, String fuelType) {
        this.brand = brand;
        this.model = model;
        this.fuelType = fuelType;
    }

    @Override
    public void drive() {
        System.out.println("Car " + brand + " " + model + " is driving.");
    }

    @Override
    public void refuel() {
        System.out.println("Refueling car with " + fuelType);
    }
}
