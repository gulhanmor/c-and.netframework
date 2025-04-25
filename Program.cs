using System;

// Main program class that serves as the entry point
class Program
{
    static void Main(string[] args)
    {
        // Initialize services with dependency injection
        IUserInterface ui = new ConsoleInterface();
        IPackageValidator validator = new PackageValidator();
        IShippingCalculator calculator = new ShippingCalculator();
        
        // Create and run the shipping application
        var shippingApp = new ShippingApplication(ui, validator, calculator);
        shippingApp.Run();
    }
}

// Interface for user interaction
interface IUserInterface
{
    void DisplayMessage(string message);
    double GetNumericInput(string prompt);
}

// Interface for package validation
interface IPackageValidator
{
    bool ValidateWeight(double weight, out string error);
    bool ValidateDimensions(double width, double height, double length, out string error);
}

// Interface for shipping calculations
interface IShippingCalculator
{
    double CalculateShippingCost(Package package);
}

// Data class to hold package information
class Package
{
    public double Weight { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Length { get; set; }
}

// Console implementation of user interface
class ConsoleInterface : IUserInterface
{
    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public double GetNumericInput(string prompt)
    {
        while (true)
        {
            DisplayMessage(prompt);
            if (double.TryParse(Console.ReadLine(), out double result))
            {
                return result;
            }
            DisplayMessage("Invalid input. Please enter a numeric value.");
        }
    }
}

// Implementation of package validator
class PackageValidator : IPackageValidator
{
    private const double MaxWeight = 50;
    private const double MaxDimensionsTotal = 50;

    public bool ValidateWeight(double weight, out string error)
    {
        error = string.Empty;
        if (weight > MaxWeight)
        {
            error = "Package too heavy to be shipped via Package Express. Have a good day.";
            return false;
        }
        return true;
    }

    public bool ValidateDimensions(double width, double height, double length, out string error)
    {
        error = string.Empty;
        if (width + height + length > MaxDimensionsTotal)
        {
            error = "Package too big to be shipped via Package Express.";
            return false;
        }
        return true;
    }
}

// Implementation of shipping calculator
class ShippingCalculator : IShippingCalculator
{
    public double CalculateShippingCost(Package package)
    {
        return (package.Width * package.Height * package.Length * package.Weight) / 100;
    }
}

// Main application class that coordinates the shipping quote process
class ShippingApplication
{
    private readonly IUserInterface _ui;
    private readonly IPackageValidator _validator;
    private readonly IShippingCalculator _calculator;

    public ShippingApplication(IUserInterface ui, IPackageValidator validator, IShippingCalculator calculator)
    {
        _ui = ui;
        _validator = validator;
        _calculator = calculator;
    }

    public void Run()
    {
        // Display welcome message
        _ui.DisplayMessage("Welcome to Package Express. Please follow the instructions below.");

        // Create package object to store dimensions
        var package = new Package();

        // Get and validate weight
        package.Weight = _ui.GetNumericInput("Please enter the package weight:");
        if (!_validator.ValidateWeight(package.Weight, out string weightError))
        {
            _ui.DisplayMessage(weightError);
            return;
        }

        // Get and validate dimensions
        package.Width = _ui.GetNumericInput("Please enter the package width:");
        package.Height = _ui.GetNumericInput("Please enter the package height:");
        package.Length = _ui.GetNumericInput("Please enter the package length:");

        if (!_validator.ValidateDimensions(package.Width, package.Height, package.Length, out string dimensionsError))
        {
            _ui.DisplayMessage(dimensionsError);
            return;
        }

        // Calculate and display shipping cost
        double shippingCost = _calculator.CalculateShippingCost(package);
        _ui.DisplayMessage($"Your estimated total for shipping this package is: ${shippingCost:F2}");
        _ui.DisplayMessage("Thank you!");
    }
}