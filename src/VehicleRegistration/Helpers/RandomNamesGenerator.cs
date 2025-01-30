using Bogus;
using Bogus.Extensions.Extras;

public class RandomNamesGenerator
{
    private static readonly Dictionary<string, string[]> CarModels = new()
    {
        ["Volkswagen"] = ["Golf", "Polo", "Passat", "Up", "Tiguan"],
        ["Toyota"] = ["Yaris", "Corolla", "RAV4", "Aygo", "C-HR"],
        ["Peugeot"] = ["208", "2008", "308", "3008", "508"],
        ["Renault"] = ["Clio", "Captur", "Megane", "Kadjar", "Scenic"],
        ["BMW"] = ["1 Serie", "3 Serie", "5 Serie", "X1", "X3"],
        ["Opel"] = ["Corsa", "Astra", "Mokka", "Crossland", "Grandland"],
        ["Ford"] = ["Fiesta", "Focus", "Puma", "Kuga", "Mondeo"],
        ["Volvo"] = ["V40", "V60", "V90", "XC40", "XC60"],
        ["Audi"] = ["A1", "A3", "A4", "Q2", "Q3"],
        ["Skoda"] = ["Fabia", "Octavia", "Superb", "Kamiq", "Karoq"],
        ["Tesla"] = ["Model 3", "Model Y", "Model S", "Model X"]
    };

    readonly Random random = Random.Shared;
    readonly Faker faker = new("nl"); // Using Dutch locale

    public (string Brand, string Model) GenerateRandomCar()
    {
        var brand = CarModels.Keys.ElementAt(random.Next(CarModels.Count));
        var models = CarModels[brand];
        var model = models[random.Next(models.Length)];

        return (brand, model);
    }

    public (string FirstName, string LastName, string email, string creditCard) GenerateRandomUser()
    {
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();

        return (firstName, lastName, faker.Internet.Email(firstName, lastName), faker.Finance.CreditCardNumberObfuscated());
    }
}