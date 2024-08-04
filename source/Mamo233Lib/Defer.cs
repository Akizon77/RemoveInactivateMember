using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mamo233Lib
{
    public class Defer : IDisposable
    {
        public Action _action { get; set; }
        public Defer(Action action)
        {
            _action = action;
        }
        public void Dispose()
        {
            if (_action != null)
                try
                {
                    _action.Invoke();
                }
                catch (Exception e)
                {

                    Log.Warning(e, "Defer 执行出错");
                }
                
        }
    }
}
