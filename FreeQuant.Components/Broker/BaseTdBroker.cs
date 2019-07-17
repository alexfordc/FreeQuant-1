﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Components {

    public abstract class BaseTdBroker {
        public BaseTdBroker() {
            EventBus.Register(this);
            LogUtil.EnginLog("交易组件启动");
        }

        #region EventBus事件
        [OnEvent]
        protected void OnTdBrokerLoginRequest(TdBrokerLoginRequest request) {
            Login();
        }

        [OnEvent]
        protected void OnTdBrokerLogoutRequest(TdBrokerLogoutRequest request) {
            Logout();
        }

        [OnEvent]
        protected void OnQueryInstrumentRequest(QueryInstrumentRequest request) {
            QueryInstrument();
        }

        [OnEvent]
        protected void OnQueryPosition(QueryPositionRequest request) {
            QueryPosition();
        }

        [OnEvent]
        protected void OnSendOrderRequest(SendOrderRequest request) {
            SendOrder(request.Order);
        }

        [OnEvent]
        protected void OnCancelOrderRequest(CancelOrderRequest request) {
            CancelOrder(request.Order);
        }
        #endregion

        #region 登录
        //登陆
        protected abstract void Login();
        //登录结果事件
        protected void PostLoginEvent(bool isSuccess = true, string errorMsg = "") {
            TdBrokerLoginEvent evt = new TdBrokerLoginEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        //登出
        protected abstract void Logout();
        //登出结果事件
        protected void PostLogoutEvent(bool isSuccess = true, string errorMsg = "") {
            TdBrokerLogoutEvent evt = new TdBrokerLogoutEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 订单
        //发送订单
        protected abstract void SendOrder(Order order);
        //撤销订单
        protected abstract void CancelOrder(Order order);
        //
        protected void PostOrderEvent(Order order) {
            OrderEvent evt = new OrderEvent(order);
            EventBus.PostEvent(evt);
            //
            order.EmmitChange();
        }

        protected void PostTradeEvent(Order order, long tradeVol) {
            TradeEvent evt = new TradeEvent(order, tradeVol);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 合约
        //请求合约
        protected abstract void QueryInstrument();
        protected void PostInstrumentEvent(Instrument inst) {
            InstrumentEvent evt = new InstrumentEvent(inst);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 持仓
        protected abstract void QueryPosition();

        protected void PostPositionEvent(PositionEvent evt) {
            EventBus.PostEvent(evt);
        }
        #endregion

    }
}
