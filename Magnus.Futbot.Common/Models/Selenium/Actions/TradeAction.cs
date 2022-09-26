﻿using Magnus.Futbot.Common.Models.DTOs.Trading;

namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class TradeAction
    {
        public TradeAction(
            Action action, 
            bool isBuy, 
            BuyCardDTO? buyCardDTO, 
            SellCardDTO? sellCardDTO, 
            CancellationTokenSource cancellationTokenSource)
        {
            Id = Guid.NewGuid().ToString();
            Action = action;
            IsBuy = isBuy;
            BuyCardDTO = buyCardDTO;
            SellCardDTO = sellCardDTO;
            CancellationTokenSource = cancellationTokenSource;
        }

        public string Id { get; }
        public Action Action { get; }
        public bool IsBuy { get; }
        public BuyCardDTO? BuyCardDTO { get; }
        public SellCardDTO? SellCardDTO { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
    }
}
