using Blazored.LocalStorage;
using Bogus;
using Liquidata.Common.Extensions;
using System.Text.Json;
using System.Reflection;
using Liquidata.Client.Services.Interfaces;
using Liquidata.Client.Emporium.Models;

namespace Liquidata.Client.Services;

public class EmporiumService(ILocalStorageService localStorage) : IEmporiumService
{
    private const string EmporiumDataKey = "EmporiumData";
    private static string EmporiumItemKey(Guid productId) => $"EmporiumItem_{productId}";

    public async Task<EmporiumData> GenerateDataAsync(Func<Task> initialAction, Func<int, int, Task> refreshAction)
    {
        await Task.Yield();
        var data = await localStorage.GetItemAsync<EmporiumData>(EmporiumDataKey);

        if (data is not null)
        {
            return data;
        }

        await initialAction();

        var iconFields = typeof(MudBlazor.Icons.Material.TwoTone)
            .GetFields(BindingFlags.Public | BindingFlags.Static);

        var faker = new Faker();
        var categories = faker.Commerce.Categories(100)
            .Distinct()
            .Take(25)
            .OrderBy(x => x)
            .Select(x => new EmporiumCategory
            {
                Name = x,
                Icon = (string)faker.Random.ArrayElement(iconFields).GetRawConstantValue()!
            })
            .ToArray();

        var items = new List<EmporiumItem>();
        var totalCount = 1000;

        await Parallel.ForAsync(0, totalCount,
            async (x, c) =>
            {
                await refreshAction(x, totalCount);

                var category = faker.Random.ArrayElement(categories);
                var result = GenerateDataItem(faker, category);
                lock (items) { items.Add(result); }
            });

        data = new EmporiumData
        {
            AllCategories = categories,
            AllItems = items
                .ToArray()
        };

        var json = JsonSerializer.Serialize(data);
        await localStorage.SetItemAsync(EmporiumDataKey, data);

        return data;
    }

    public async Task<EmporiumData?> LoadDataAsync()
    {
        return await localStorage.GetItemAsync<EmporiumData>(EmporiumDataKey);
    }

    public async Task<EmporiumItem> LoadDataItemAsync(Guid productId, Func<Task> initialAction, Func<int, int, Task> refreshAction)
    {
        var productKey = EmporiumItemKey(productId);
        var product = await localStorage.GetItemAsync<EmporiumItem>(productKey);

        if (product is not null)
        {
            return product;
        }

        var data = await localStorage.GetItemAsync<EmporiumData>(EmporiumDataKey)
            ?? throw new Exception("No data found");

        product = data.AllItems
            .FirstOrDefault(x => x.ProductId == productId);

        if (product is null)
        {
            throw new Exception("Product not found");
        }

        await initialAction();

        var faker = new Faker();

        var items = new List<EmporiumReview>();
        var totalCount = faker.Random.Number(10, 500);

        await Parallel.ForAsync(0, totalCount,
            async (x, c) =>
            {
                await refreshAction(x, totalCount);

                var review = new EmporiumReview
                {
                    Reviewer = new Bogus.Person().FullName,
                    StarRating = GetRandomStarRating(product.StarRating),
                    Review = faker.Rant.Review(product.Name)
                };

                lock (items) { items.Add(review); }
            });

        product.Reviews = items.ToArray();
        await localStorage.SetItemAsync(productKey, product);
        return product;
    }

    private float GetRandomStarRating(float targetValue)
    {
        var scale = targetValue * 2;
        var result = (float)(Random.Shared.NextDouble() * scale - scale / 2 + targetValue);
        result = result.RoundDownToNearest(0.5F);

        if (result >= 10)
        {
            return 10F;
        }

        return result;
    }

    private EmporiumItem GenerateDataItem(Faker faker, EmporiumCategory category)
    {
        var item = new EmporiumItem
        {
            Category = category,
            ImageLink = $"https://picsum.photos/id/{Random.Shared.Next(0, 1084)}/300/300",
            Manufacturer = faker.Company.CompanyName(),
            Name = faker.Commerce.ProductName(),
            Price = float.Parse(faker.Commerce.Price()),
            Quantity = faker.Random.Number(0, 1000),
            Description = faker.Commerce.ProductDescription(),
            StarRating = faker.Random.Number(0, 10)
        };

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
