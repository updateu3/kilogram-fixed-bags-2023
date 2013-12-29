﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Annotations;
using Telegram.Model.TLWrappers;

namespace Telegram.MTProto.Components {

    public class Dialogs {
        private TelegramSession session;
        private DialogListModel state;
 
        public Dialogs(TelegramSession session) {
            this.session = session;
            state = new DialogListModel();
        }

        public Dialogs(TelegramSession session, BinaryReader reader) {
            this.session = session;
            load(reader);
        }

        public async Task<DialogListModel> DialogsRequest() {
            DialogListModel newState = new DialogListModel();

            int offset = 0;
            while(true) {
                messages_Dialogs dialogsPart = await session.Api.messages_getDialogs(offset, 0, 100);
                offset += newState.ProcessDialogs(dialogsPart);

                if(dialogsPart.Constructor == Constructor.messages_dialogs) {
                    break;
                }
            }

            
            state.Replace(newState);

            return newState;
        }

        public DialogListModel State {
            get {
                return state;
            }
        }

        public void save(BinaryWriter writer) {
            if(state == null) {
                writer.Write(0);
            } else {
                writer.Write(1);
                state.save(writer);
            }
        }

        public void load(BinaryReader reader) {
            int stateExists = reader.ReadInt32();
            if(stateExists != 0) {
                state = new DialogListModel();
                state.load(reader);
            } else {
                state = null;
            }
        }
    }

}