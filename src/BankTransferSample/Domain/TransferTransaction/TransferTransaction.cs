﻿using System;
using ENode.Domain;
using ENode.Infrastructure;

namespace BankTransferSample.Domain
{
    /// <summary>聚合根，表示一笔银行内账户之间的转账交易
    /// </summary>
    [Serializable]
    [Code(12)]
    public class TransferTransaction : AggregateRoot<string>
    {
        #region Private Variables

        private TransferTransactionInfo _transactionInfo;
        private TransactionStatus _status;
        private bool _isSourceAccountValidatePassed;
        private bool _isTargetAccountValidatePassed;
        private bool _isTransferOutPreparationConfirmed;
        private bool _isTransferInPreparationConfirmed;
        private bool _isTransferOutConfirmed;
        private bool _isTransferInConfirmed;

        #endregion

        #region Constructors

        /// <summary>构造函数
        /// </summary>
        public TransferTransaction(string transactionId, TransferTransactionInfo transactionInfo)
            : base(transactionId)
        {
            ApplyEvent(new TransferTransactionStartedEvent(this, transactionInfo));
        }

        #endregion

        #region Public Methods

        /// <summary>确认账户验证通过
        /// </summary>
        public void ConfirmAccountValidatePassed(string accountId)
        {
            if (_status == TransactionStatus.Started)
            {
                if (accountId == _transactionInfo.SourceAccountId)
                {
                    if (!_isSourceAccountValidatePassed)
                    {
                        ApplyEvent(new SourceAccountValidatePassedConfirmedEvent(this, _transactionInfo));
                        if (_isTargetAccountValidatePassed)
                        {
                            ApplyEvent(new AccountValidatePassedConfirmCompletedEvent(this, _transactionInfo));
                        }
                    }
                }
                else if (accountId == _transactionInfo.TargetAccountId)
                {
                    if (!_isTargetAccountValidatePassed)
                    {
                        ApplyEvent(new TargetAccountValidatePassedConfirmedEvent(this, _transactionInfo));
                        if (_isSourceAccountValidatePassed)
                        {
                            ApplyEvent(new AccountValidatePassedConfirmCompletedEvent(this, _transactionInfo));
                        }
                    }
                }
            }
        }
        /// <summary>确认预转出
        /// </summary>
        public void ConfirmTransferOutPreparation()
        {
            if (_status == TransactionStatus.AccountValidateCompleted)
            {
                if (!_isTransferOutPreparationConfirmed)
                {
                    ApplyEvent(new TransferOutPreparationConfirmedEvent(this, _transactionInfo));
                }
            }
        }
        /// <summary>确认预转入
        /// </summary>
        public void ConfirmTransferInPreparation()
        {
            if (_status == TransactionStatus.AccountValidateCompleted)
            {
                if (!_isTransferInPreparationConfirmed)
                {
                    ApplyEvent(new TransferInPreparationConfirmedEvent(this, _transactionInfo));
                }
            }
        }
        /// <summary>确认转出
        /// </summary>
        public void ConfirmTransferOut()
        {
            if (_status == TransactionStatus.PreparationCompleted)
            {
                if (!_isTransferOutConfirmed)
                {
                    ApplyEvent(new TransferOutConfirmedEvent(this, _transactionInfo));
                    if (_isTransferInConfirmed)
                    {
                        ApplyEvent(new TransferTransactionCompletedEvent(this));
                    }
                }
            }
        }
        /// <summary>确认转入
        /// </summary>
        public void ConfirmTransferIn()
        {
            if (_status == TransactionStatus.PreparationCompleted)
            {
                if (!_isTransferInConfirmed)
                {
                    ApplyEvent(new TransferInConfirmedEvent(this, _transactionInfo));
                    if (_isTransferOutConfirmed)
                    {
                        ApplyEvent(new TransferTransactionCompletedEvent(this));
                    }
                }
            }
        }
        /// <summary>取消转账交易
        /// </summary>
        public void Cancel()
        {
            ApplyEvent(new TransferTransactionCanceledEvent(this));
        }

        #endregion

        #region Handler Methods

        private void Handle(TransferTransactionStartedEvent evnt)
        {
            _id = evnt.AggregateRootId;
            _transactionInfo = evnt.TransactionInfo;
            _status = TransactionStatus.Started;
        }
        private void Handle(SourceAccountValidatePassedConfirmedEvent evnt)
        {
            _isSourceAccountValidatePassed = true;
        }
        private void Handle(TargetAccountValidatePassedConfirmedEvent evnt)
        {
            _isTargetAccountValidatePassed = true;
        }
        private void Handle(AccountValidatePassedConfirmCompletedEvent evnt)
        {
            _status = TransactionStatus.AccountValidateCompleted;
        }
        private void Handle(TransferOutPreparationConfirmedEvent evnt)
        {
            _isTransferOutPreparationConfirmed = true;
        }
        private void Handle(TransferInPreparationConfirmedEvent evnt)
        {
            _isTransferInPreparationConfirmed = true;
            _status = TransactionStatus.PreparationCompleted;
        }
        private void Handle(TransferOutConfirmedEvent evnt)
        {
            _isTransferOutConfirmed = true;
        }
        private void Handle(TransferInConfirmedEvent evnt)
        {
            _isTransferInConfirmed = true;
        }
        private void Handle(TransferTransactionCompletedEvent evnt)
        {
            _status = TransactionStatus.Completed;
        }
        private void Handle(TransferTransactionCanceledEvent evnt)
        {
            _status = TransactionStatus.Canceled;
        }

        #endregion
    }
}
