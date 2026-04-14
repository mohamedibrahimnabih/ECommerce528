namespace ECommerce528.Services
{
    public class ProductService
    {
        /// <summary>
        /// Saves the provided image file to the server and returns the generated file name.
        /// </summary>
        /// <remarks>The image is saved in the 'wwwroot/images/product_logo' directory of the application's
        /// root. The file name is generated using the current date and a unique identifier to avoid
        /// collisions.</remarks>
        /// <param name="logo">The image file to be saved. Must not be null and should contain a valid file name and content.</param>
        /// <returns>The generated file name of the saved image if the operation succeeds; otherwise, null.</returns>
        public string? SaveImg(IFormFile logo)
        {
            try
            {
                var fileName = $"{DateTime.Now.ToString("dd_MM_yyyy")}_{Guid.NewGuid()}{Path.GetExtension(logo.FileName)}";
                //14_4_2026_509ksdfjskl59034509.png

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_logo", fileName);

                //if (!System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    logo.CopyTo(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Removes an image file with the specified name from the product logo images directory.
        /// </summary>
        /// <remarks>If the specified file does not exist, the method returns true. Any errors encountered
        /// during file deletion will result in a return value of false.</remarks>
        /// <param name="logoName">The name of the image file to remove. This should include the file extension. Cannot be null or empty.</param>
        /// <returns>true if the image file was successfully removed or did not exist; otherwise, false.</returns>
        public bool RemoveImg(string logoName)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_logo", logoName);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                Console.WriteLine($"Remove old img from wwwroot");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
