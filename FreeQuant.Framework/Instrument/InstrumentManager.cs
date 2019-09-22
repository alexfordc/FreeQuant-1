﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.EventEngin;

namespace FreeQuant.Framework {
    public class InstrumentManager : IComponent {
        private static ConcurrentDictionary<string, Instrument> mInstrumentMap = new ConcurrentDictionary<string, Instrument>();

        public void OnLoad() {
            EventBus.Register(this);
            LogUtil.EnginLog("合约管理组件启动");
        }

        public void OnReady() { }

        [OnEvent]
        private void OnInstrument(BrokerEvent.BrokerInstrumentEvent evt) {
            Instrument inst = evt.Instrument;
            mInstrumentMap.TryAdd(inst.InstrumentID, inst);
            BrokerEvent.InstrumentEvent evtOut = new BrokerEvent.InstrumentEvent(inst);
            EventBus.PostEvent(evtOut);
        }

        public static Instrument GetInstrument(string instrumentId) {
            Instrument inst;
            if (mInstrumentMap.TryGetValue(instrumentId, out inst)) {
                return inst;
            } else {
                return null;
            }
        }
    }
}
