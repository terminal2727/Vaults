using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultsII.Core {
    public static class WindowSizeChange {
        public static event EventHandler OnWindowSizeChanged;

        public static void ChangedWindowSize() => OnWindowSizeChanged?.Invoke(null, EventArgs.Empty);
    }
}
