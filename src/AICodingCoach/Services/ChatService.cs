﻿using System.IO;
using Ara3D.Utils;
using OpenAI_API;
using OpenAI_API.Chat;

namespace AICodingCoach.Services
{
    /// <summary>
    /// Communicates with the OpenAI API.
    /// Initializes a conversation, sends prompts (both user and system)
    /// and receives input
    /// </summary>
    public class ChatService
    {
        public OpenAIAPI Api { get; }
        public Conversation Conversation { get; }
        public string Prompt { get; set; } = "";
        public string SystemRolePrompt { get; }

        public ChatService(string systemPrompt)
        {
            const string modelName = "gpt-4-1106-preview";
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var apiKeyFile = Path.Combine(folder, "api-keys", "chat-gpt.api.txt");
            var key = File.ReadAllText(apiKeyFile);
            Api = new OpenAIAPI(key);
    
            var chatRequest = new ChatRequest
            {
                Model = modelName
            };
            Conversation = Api.Chat.CreateConversation(chatRequest);

            SendSystemPrompt(SystemRolePrompt = systemPrompt);
        }

        public void SendSystemPrompt(string content)
        {
            Conversation.AppendSystemMessage(content);
        }

        public async Task SendPromptAsync(string prompt, Action<int, string> handler)
        {
            Prompt = prompt;
            await SendPromptAsync(handler);
        }

        public async Task SendPromptAsync(Action<int, string> handler)
        {
            try
            {
                Conversation.AppendUserInput(Prompt);
                await Conversation.StreamResponseFromChatbotAsync(handler);
            }
            catch (Exception ex)
            {
                handler.Invoke(0, $"\n\nAn error occurred {ex}");
            }
        }
    }
}