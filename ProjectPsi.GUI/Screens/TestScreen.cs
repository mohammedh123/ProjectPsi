using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPsi.GUI.Screens
{
    class TestScreen : GameScreen
    {
        public override void Update(TimeSpan gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(TimeSpan gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
