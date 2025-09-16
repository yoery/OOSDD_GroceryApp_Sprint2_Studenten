using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    [QueryProperty(nameof(GroceryList), nameof(GroceryList))]
    public partial class GroceryListItemsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        private readonly IProductService _productService;

        public ObservableCollection<GroceryListItem> MyGroceryListItems { get; set; } = [];
        public ObservableCollection<Product> AvailableProducts { get; set; } = [];

        [ObservableProperty]
        GroceryList groceryList = new(0, "None", DateOnly.MinValue, "", 0);

        public GroceryListItemsViewModel(IGroceryListItemsService groceryListItemsService, IProductService productService)
        {
            _groceryListItemsService = groceryListItemsService;
            _productService = productService;
            Load(groceryList.Id);
        }

        private void Load(int id)
        {
            MyGroceryListItems.Clear();

            foreach (var item in _groceryListItemsService.GetAllOnGroceryListId(id))
                MyGroceryListItems.Add(item);

            GetAvailableProducts();
        }

        private void GetAvailableProducts()
        {
            AvailableProducts.Clear();

            // Alle producten ophalen
            var allProducts = _productService.GetAll();

            // Id’s van producten die al op de boodschappenlijst staan
            var productsOnList = MyGroceryListItems.Select(i => i.ProductId).ToHashSet();

            foreach (var product in allProducts)
            {
                // Alleen tonen als er voorraad is en het product nog niet in de lijst staat
                if (product.Stock > 0 && !productsOnList.Contains(product.Id))
                {
                    AvailableProducts.Add(product);
                }
            }
        }

        partial void OnGroceryListChanged(GroceryList value)
        {
            Load(value.Id);
        }

        [RelayCommand]
        public async Task ChangeColor()
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), GroceryList } };
            await Shell.Current.GoToAsync($"{nameof(ChangeColorView)}?Name={GroceryList.Name}", true, paramater);
        }

        [RelayCommand]
        public void AddProduct(Product product)
        {
            if (product == null || product.Id <= 0)
                return;

            var newItem = new GroceryListItem(0, GroceryList.Id, product.Id, 1){
                Product = product
            };

            // Opslaan via service
            _groceryListItemsService.Add(newItem);

            // Voorraad verlagen en updaten
            product.Stock -= 1;
            _productService.Update(product);

            // Lijsten opnieuw laden
            OnGroceryListChanged(GroceryList);
        }
    }
}
