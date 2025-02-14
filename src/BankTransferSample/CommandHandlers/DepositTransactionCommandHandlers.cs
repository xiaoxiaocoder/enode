﻿using BankTransferSample.Commands;
using BankTransferSample.Domain;
using ECommon.Components;
using ECommon.Utilities;
using ENode.Commanding;
using ENode.Infrastructure;

namespace BankTransferSample.CommandHandlers
{
    /// <summary>银行存款交易相关命令处理
    /// </summary>
    [Component]
    [Code(1001)]
    public class DepositTransactionCommandHandlers :
        ICommandHandler<StartDepositTransactionCommand>,                      //开始交易
        ICommandHandler<ConfirmDepositPreparationCommand>,                    //确认预存款
        ICommandHandler<ConfirmDepositCommand>                                //确认存款
    {
        public void Handle(ICommandContext context, StartDepositTransactionCommand command)
        {
            context.Add(new DepositTransaction(command.AggregateRootId, command.AccountId, command.Amount));
        }
        public void Handle(ICommandContext context, ConfirmDepositPreparationCommand command)
        {
            context.Get<DepositTransaction>(command.AggregateRootId).ConfirmDepositPreparation();
        }
        public void Handle(ICommandContext context, ConfirmDepositCommand command)
        {
            context.Get<DepositTransaction>(command.AggregateRootId).ConfirmDeposit();
        }
    }
}
