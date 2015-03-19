using UnityEngine;
using System.Collections;

public class GradualValue
{

    private float val, step, end;
    private int ticks, maxTicks;

    public GradualValue(float Start, float End, int NumTicks)
    {
        val = Start;
        step = (End - Start)/NumTicks;
        end = End;
        ticks = 0;
        maxTicks = NumTicks;
    }

    public void Tick()
    {
        if (ticks < maxTicks)
        {
            ticks++;
            if (ticks == maxTicks)
                val = end;
            else
                val += step;
        }
    }

    public float Value { get { return val; } }

    public float TickValue
    {
        get
        {
            Tick();
            return val;
        }
    }

    public static implicit operator float(GradualValue gv)
    {
        return gv.Value;
    }
}
