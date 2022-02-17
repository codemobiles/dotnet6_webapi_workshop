using dotnet_hero.Data;
using dotnet_hero.Entities;
using dotnet_hero.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet_hero.Services
{
    public class ProductService : IProductService
    {
        private readonly DatabaseContext databaseContext;
        private readonly IUploadFileService uploadFileService;
        public ProductService(DatabaseContext databaseContext, IUploadFileService uploadFileService)
        {
            this.uploadFileService = uploadFileService;
            this.databaseContext = databaseContext;
        }

        public async Task Create(Product product)
        {            

            databaseContext.Products.Add(product);
            await databaseContext.SaveChangesAsync();
        }

        public async Task Delete(Product product)
        {
            databaseContext.Products.Remove(product);
            await databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> FindAll()
        {
            return await databaseContext.Products.Include(p => p.Category)
            .OrderByDescending(p => p.ProductId).ToListAsync();
        }

        public async Task<Product> FindById(int id)
        {
            return await databaseContext.Products.Include(p => p.Category)
                                                 .SingleOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> Search(string name)
        {
            return await databaseContext.Products.Include(p => p.Category)
                .Where(p => p.Name.ToLower().Contains(name)).ToListAsync();
        }

        public async Task Update(Product product)
        {
            databaseContext.Products.Update(product);
            await databaseContext.SaveChangesAsync();
        }

        public async Task<(string errorMessage, string imageName)> UploadImage(List<IFormFile> formFiles)
        {
            string errorMesage = String.Empty;
            string imageName = String.Empty;
            if (uploadFileService.IsUpload(formFiles))
            {
                errorMesage = uploadFileService.Validation(formFiles);
                if (String.IsNullOrEmpty(errorMesage))
                {
                    imageName = (await uploadFileService.UploadImages(formFiles))[0];
                }
            }
            return (errorMesage, imageName);
        }
    }
}