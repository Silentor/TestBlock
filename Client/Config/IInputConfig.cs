using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code.Config
{
    public interface IInputConfig
    {
        float YawSensitivity { get; }

        float PitchSensitivity { get; }
    }
}
