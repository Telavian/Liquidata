using Blazored.LocalStorage;
using Bogus;
using Liquidata.Emporium.Models;
using Liquidata.Emporium.Services.Interfaces;
using Liquidata.Common.Extensions;
using Bogus.Distributions.Gaussian;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Liquidata.Emporium.Services;

public class EmporiumService(ILocalStorageService localStorage) : IEmporiumService
{
    private const string EmporiumDataKey = "EmporiumData";

    public async Task GenerateDataAsync()
    {
        await Task.Yield();
        var data = await localStorage.GetItemAsync<EmporiumData>(EmporiumDataKey);

        if (data is not null)
        {
            return;
        }

        var faker = new Faker();
        var categories = faker.Commerce.Categories(25);
        var items = new List<EmporiumItem>();

        for (var x = 0; x < 10; x++)
        {
            await Task.Yield();

            var category = faker.Random.ArrayElement(categories);
            var item = GenerateDataItem(faker, category);
            items.Add(item);
        }

        data = new EmporiumData
        {
            AllCategories = categories,
            AllItems = items
                .ToArray()
        };

        var json = JsonSerializer.Serialize(data);

        Console.WriteLine(json.Length);
        await localStorage.SetItemAsync(EmporiumDataKey, data);
    }

    private EmporiumItem GenerateDataItem(Faker faker, string category)
    {
        var item = new EmporiumItem
        {
            Category = category,
            ImageLink = faker.Image.PicsumUrl(300, 300),            
            Manufacturer = faker.Company.CompanyName(),
            Name = faker.Commerce.ProductName(),
            Price = float.Parse(faker.Commerce.Price()),
            Quantity = faker.Random.Number(0, 1000),
            Description = faker.Commerce.ProductDescription()
        };

        item.Reviews = Enumerable.Range(0, faker.Random.Number(10, 100))
            .Select(x => new EmporiumReview
            {
                Reviewer = new Bogus.Person().FullName,
                StarRating = faker.Random.Float(0, 5).RoundDownToNearest(0.5F),
                Review = faker.Rant.Review(item.Name)
            })
            .ToArray();

        item.StarRating = item.Reviews
            .Average(x => x.StarRating)
            .RoundDownToNearest(0.5F);

        if (faker.Random.Bool())
        {
            item.Attributes.Add("Color", faker.Commerce.Color());
        }

        if (faker.Random.Bool())
        {
            item.Attributes.Add("Material", faker.Commerce.ProductMaterial());
        }

        if (faker.Random.Bool())
        {
            item.Attributes.Add("Country", faker.Address.Country());
        }

        if (faker.Random.Bool())
        {
            item.Attributes.Add("Product Number", faker.Commerce.Ean13());
        }

        if (faker.Random.Bool())
        {
            item.Attributes.Add("Support address", faker.Internet.Url());
        }

        if (faker.Random.Bool())
        {
            item.Attributes.Add("Support number", faker.Phone.PhoneNumber());
        }

        return item;
    }
}
