import java.util.Scanner;

public class Main {

    public static void main(String[] args) {

        Scanner scanner = new Scanner(System.in);
        VehicleFactory factory = null;

        System.out.println("Choose vehicle type: car, motorcycle, truck, bus");
        String choice = scanner.nextLine();

        switch (choice.toLowerCase()) {

            case "car":
                System.out.print("Brand: ");
                String brand = scanner.nextLine();

                System.out.print("Model: ");
                String model = scanner.nextLine();

                System.out.print("Fuel type: ");
                String fuel = scanner.nextLine();

                factory = new CarFactory(brand, model, fuel);
                break;

            case "motorcycle":
                System.out.print("Type: ");
                String type = scanner.nextLine();

                System.out.print("Engine capacity: ");
                int capacity = scanner.nextInt();

                factory = new MotorcycleFactory(type, capacity);
                break;

            case "truck":
                System.out.print("Load capacity (tons): ");
                double load = scanner.nextDouble();

                System.out.print("Number of axles: ");
                int axles = scanner.nextInt();

                factory = new TruckFactory(load, axles);
                break;

            case "bus":
                System.out.print("Passenger capacity: ");
                int passengers = scanner.nextInt();
                scanner.nextLine();

                System.out.print("Route number: ");
                String route = scanner.nextLine();

                factory = new BusFactory(passengers, route);
                break;

            default:
                System.out.println("Invalid type!");
                return;
        }

        IVehicle vehicle = factory.createVehicle();
        vehicle.drive();
        vehicle.refuel();
    }
}
