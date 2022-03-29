namespace CatBot.Services
{
    public static class CatPicService
    {
        static HttpClient client = new HttpClient();
        public static async Task<MemoryStream?> GetRandomCatPictureAsync()
        {
            var response = await client.GetAsync("https://cataas.com/cat");
            if (response.IsSuccessStatusCode)
            {
                MemoryStream picture = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
                return picture;
            }
            else
            {
                return null;
            }
        }

        public static async Task<MemoryStream?> GetRandomCatPictureAsync(string tag)
        {
            var response = await client.GetAsync($"https://cataas.com/cat/{tag}");
            if (response.IsSuccessStatusCode)
            {
                MemoryStream picture = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
                return picture;
            }
            else
            {
                return null;
            }
        }
    }
}
