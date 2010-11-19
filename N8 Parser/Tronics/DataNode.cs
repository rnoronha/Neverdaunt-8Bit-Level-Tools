using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8Parser.Tronics
{
    public class DataBlock : N8Tronic
    {
        private DataNodeIn _in;
        private DataNodeOut _out;

        public DataNodeIn In
        {
            get
            {
                return _in;
            }
        }

        public DataNodeOut Out
        {
            get
            {
                return _out;
            }
        }

        public DataBlock(int id, string name="Data", string data="", string type="cdata") : base(id, type, name, data){ 
        
            _in = new DataNodeIn(this);
            _out = new DataNodeOut(this);
        }
    }
    //The innness or outness of a data node is relative to the tronic, not the node.
    public class DataNodeIn
    {
        public DataBlock Parent
        {
            get
            {
                return _parent;
            }
        }
        DataBlock _parent;
        public DataNodeIn(DataBlock parent)
        {
            this._parent = parent;
        }
    }

    public class DataNodeOut
    {
        public DataBlock Parent
        {
            get
            {
                return _parent;
            }
        }
        DataBlock _parent;
        public DataNodeOut(DataBlock parent)
        {
            this._parent = parent;
        }


    }
}
