﻿using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models.Requests;
using System.Net;
using System.Text.Json;

namespace Magnus.Futtbot.Connections.Connection.Trading.Sell
{
    public class SellConnection
    {
        private readonly HttpClient _httpClient;

        public SellConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConnectionResponseType> SellCard(string username, SellCardRequest sellCardRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://utas.mob.v2.fut.ea.com/ut/game/fc24/auctionhouse");
            request.SetCommonHeaders(username);
            var content = new StringContent(JsonSerializer.Serialize(sellCardRequest), null, "application/json");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.UnavailableForLegalReasons
                || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(1000);
                return ConnectionResponseType.PauseForAWhile;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return ConnectionResponseType.Unauthorized;

            if (!response.IsSuccessStatusCode)
                return ConnectionResponseType.PauseForAWhile;

            return ConnectionResponseType.Success;
        }
    }
}
