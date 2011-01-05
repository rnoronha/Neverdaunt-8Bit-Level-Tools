using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8Parser.Tronics.TronicMacros
{
    public class MessageScreen
    {
        public static TronicSequence GetMessageScreen(N8BlockFactory blocks = null, string separator = " === ")
        {
            TronicSequence ts = TronicSequence.StartFromKeyboard(blocks);
            DataBlock CurrentMessage = ts.data.First();
            DataBlock Separator = ts.NewDataBlock("Separator", separator);
            DataBlock MessageArchive = ts.NewDataBlock("MessageArchive", "Archive begins: ");

            ts.And(MessageArchive.In, Separator.In, MessageArchive.Out, "Separator Append")
              .And(MessageArchive.In, CurrentMessage.In, MessageArchive.Out, "Message Append")
              .Display(CurrentMessage.In, "Message Display");

            return ts;
        }
    }
}
