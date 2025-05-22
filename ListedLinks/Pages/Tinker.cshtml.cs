using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OpenAI.RealtimeConversation;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Batch;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.VectorStores;
using System;
using System.Text.Json;
//using OpenAI;
using ChatGPT.Net;
using System.Threading.Tasks;
using ListedLinks.Models;

namespace ListedLinks.Pages
{
    public class TinkerModel : PageModel
    {
        // TODO: OpenAI stuff
        //private OpenAIClient client = new OpenAIClient("");
        //private static ChatGpt _bot = new ChatGpt("");
        public string Header { get; private set; } = "Laughable";

        public string Message { get; private set; } = "";

        public static string? ApiKey { get; set; } = "";

        public async Task OnGet()
        {
            var _bot = new ChatGpt(TinkerModel.ApiKey ?? "");
            var aiResponse = await _bot.Ask("You are a comedian specializing in positive humor related to PatchMyPC, a beloved remote patching software. Your favorite bird is red and you dip your bananas in coffee. You will tell a joke. Your joke must include a reference to PatchMyPC. Tell an edgy tech joke.");

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ip != null && aiResponse != null) {
                using (var dbContext = new ListedLinksContext())
                {
                    if (!dbContext.IPAddressStrings.Where(_ => String.Equals(_.IPAddress, ip)).Any())
                        dbContext.IPAddressStrings.Add(new IPAddressString { IPAddress = ip });

                    dbContext.Comments.Add(new Comment { Text = aiResponse, CreatedAt = DateTime.Now });
                    dbContext.SaveChanges();
                }
            }

            Message += $"{aiResponse ?? "<ai failed>"}";
            Header += $" [{ip ?? "<unknown IP>"}]";
        }
    }

    public class TinkerModelSettings
    {
        public string? ApiKey { get; set; }
    }
}