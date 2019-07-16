﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FreeQuant.Framework;

namespace FreeQuant.Components {
    [Component]
    internal class StrategyManager{
        public StrategyManager()
        {
            loadStrategy();
            LogUtil.EnginLog("策略管理组件启动");
        }

        //策略添加
        private Dictionary<string, BaseStrategy> mStrategyMap = new Dictionary<string, BaseStrategy>();
        private void loadStrategy() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "\\strategys";
                if (Directory.Exists(dir)) {
                    files = Directory.GetFiles(dir);
                } else {
                    Directory.CreateDirectory(dir);
                }
            } catch (Exception ex) {
                LogUtil.EnginLog(ex.StackTrace);
            }

            //加载策略
            foreach (string f in files) {
                Assembly assembly = Assembly.LoadFrom(f);
                Type[] types = assembly.GetTypes();
                foreach (Type t in types) {
                    if (!t.IsSubclassOf(typeof(BaseStrategy)))
                        continue;

                    BaseStrategy stg = Activator.CreateInstance(t) as BaseStrategy;
                    if (stg == null)
                        continue;

                    addStrategy(t.Name, stg);
                }
            }
        }
        internal void addStrategy(string name, BaseStrategy stg) {
            //策略去重
            if (mStrategyMap.ContainsKey(name))
                return;
            mStrategyMap.Add(name, stg);

            //行情
            stg.OnSubscribeInstrument += Subscribe;

            //交易
            stg.OnSendOrder += sendOrder;
            stg.OnCancelOrder += cancelOrder;
            stg.OnChangePosition += changePosition;
        }

        #region 策略启停

        private void startStrategy() {
            foreach (BaseStrategy stg in mStrategyMap.Values) {
                stg.SendStart();
            }
        }

        private void stopStrategy() {
            foreach (BaseStrategy stg in mStrategyMap.Values) {
                stg.SendStop();
            }
        }

        #endregion

        #region 处理行情
        //获取行情合约
        private TickDispatcher mTickDispatcher = new TickDispatcher();
        internal List<Instrument> InstrumentList => mTickDispatcher.InstrumentList;
        private void Subscribe(BaseStrategy stg, Instrument inst) {
            mTickDispatcher.Map(inst, stg);
            SubscribeInstrumentRequest request = new SubscribeInstrumentRequest(inst);
            EventBus.PostEvent(request);
        }

        [OnEvent]
        private void OnTickEvent(TickEvent evt) {
            mTickDispatcher.Dispatch(evt.Tick);
        }
        #endregion

        #region 处理交易
        private OrderManager mOrderManager = new OrderManager();
        private void sendOrder(Order order) {
            SendOrderRequest request = new SendOrderRequest(order);
            EventBus.PostEvent(request);
            mOrderManager.AddOrder(order);
        }

        private void cancelOrder(Order order) {
            CancelOrderRequest request = new CancelOrderRequest(order);
            EventBus.PostEvent(request);
        }

        private void changePosition(Position position) {
            ChangePositionEvent evt = new ChangePositionEvent(position);
            EventBus.PostEvent(evt);
        }
        #endregion

    }
}