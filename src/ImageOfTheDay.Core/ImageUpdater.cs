using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmugMug.NET;

namespace ImageOfTheDay.Core
{
    public class ImageUpdater
    {
        private readonly SmugMugAPI _api;
        public ImageUpdater(string consumerKey)
        {
            _api = new SmugMugAPI(LoginType.Anonymous, new OAuthCredentials(consumerKey));
        }

        public async Task UpdateImageOfTheDay(IEnumerable<string> rootNodes, string targetMakerUrl)
        {
            var startingPoints = rootNodes as IList<string> ?? rootNodes.ToList();
            if (rootNodes != null && startingPoints.Any())
            {
                var url = await GetImageOfTheDayUrl(startingPoints);
                if (!string.IsNullOrWhiteSpace(url))
                    await PostImageUrlToIFTTT(url, targetMakerUrl);
            }
        }

        // ReSharper disable once InconsistentNaming
        private static async Task PostImageUrlToIFTTT(string imageUrl, string targetMakerUrl)
        {
            
            var payload = new IFTTTMakerPayload { value1 = imageUrl };
            var postbody = JsonConvert.SerializeObject(payload);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await httpClient.PostAsync(targetMakerUrl, new StringContent(postbody, Encoding.UTF8, "application/json"));
            }
        }
        private readonly Random _rand = new Random();
        async Task<string> GetImageOfTheDayUrl(IEnumerable<string> startingPoints)
        {
            if (startingPoints != null)
            {
                var images = new List<AlbumImage>();
                foreach (var sp in startingPoints)
                {
                    images.AddRange(await ExtractInfo(sp));
                }
                if (images.Any())
                {
                    var img = images[_rand.Next(0, images.Count - 1)];
                    return img.ArchivedUri;
                }
            }

            return null;
        }

        private async Task<IEnumerable<AlbumImage>> ExtractInfo(string nodeid)
        {
            var node = await _api.GetNode(nodeid);
            return await ExtractImages(node);

        }

        private async Task<IEnumerable<AlbumImage>> ExtractImages(Node n)
        {
            switch (n.Type)
            {
                case NodeType.Album:
                    var albumId = n.Uris.Album.Uri.Replace("/api/v2/album/", "");

                    var album = await _api.GetAlbum(albumId);
                    return await ExtractImages(album);
                default:
                    var nodes = await _api.GetChildNodes(n);
                    var retVal = new List<AlbumImage>();
                    foreach (var c in nodes)
                    {
                        retVal.AddRange(await ExtractImages(c));
                    }
                    return retVal;
            }
        }


        private async Task<IEnumerable<AlbumImage>> ExtractImages(Album a)
        {
            var images = await _api.GetAlbumImages(a);
            return images;
        }
    }
}
