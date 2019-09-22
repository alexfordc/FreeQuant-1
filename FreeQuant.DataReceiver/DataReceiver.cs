﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using FreeQuant.Framework;
using FreeQuant.EventEngin;
using IComponent = FreeQuant.Framework.IComponent;

namespace FreeQuant.DataReceiver {
    internal class DataReceiver:IComponent {
        public void OnLoad() {
            EventBus.Register(this);
        }

        public void OnReady() {}

        //bar生成器
        Dictionary<string, BarGenerator> GeneratorMap = new Dictionary<string, BarGenerator>();

        //数据
        [OnEvent]
        private void OnTick(BrokerEvent.TickEvent evt) {
            Tick tick = evt.Tick;
            BarGenerator generator;
            if (!GeneratorMap.TryGetValue(tick.Instrument.InstrumentID, out generator)) {
                generator = new BarGenerator(tick.Instrument, BarSizeType.Min1);
                generator.OnBarTick += _onBarTick;
                GeneratorMap.Add(tick.Instrument.InstrumentID, generator);
            }
            //
            generator.addTick(tick);

        }

        //合约
        [OnEvent]
        private void OnInstrument(BrokerEvent.InstrumentEvent evt) {
            //建表
            string[] products = DataBaseConfig.Config.Instruments.Split(',');
            foreach (string product in products) {
                if (product.Equals(RegexUtils.TakeProductName(evt.Instrument.InstrumentID))) {
                    string name = RegexUtils.TakeShortInstrumentID(evt.Instrument.InstrumentID);
                    mWriter.CreateTable(name);
                }
            }
        }

        //
        IDataWriter mWriter = new SqlServerWriter();
        private void _onBarTick(Bar bar, List<Tick> ticks) {
            mWriter.InsertBar(bar);
            mWriter.InsertTicks(ticks);
        }
    }
}
