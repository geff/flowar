using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Flowar.Tools
{
    public class Lerp
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public Boolean Loop { get; set; }
        public Boolean Reverse { get; set; }
        public int Duration { get; set; }
        public float Step { get; set; }

        private TimeSpan startTime;
        private float startValue;
        private float sens;

        private bool stopped;
        private float valueStopped;

        public Lerp(float min, float max, Boolean loop, Boolean reverse, int duration, float step)
        {
            this.Min = min;
            this.Max = max;
            this.Loop = loop;
            this.Reverse = reverse;
            this.Duration = duration;
            this.Step = step;
            this.startValue = min;

            this.startTime = TimeSpan.MinValue;
            this.sens = min < max ? 1f : -1f;
        }

        public float Eval(GameTime gameTime, Boolean stop)
        {
            valueStopped = Eval(gameTime);
            stopped = stop;
            return valueStopped;
        }

        public float Eval(GameTime gameTime)
        {
            if (stopped)
                return valueStopped;

            float ret;

            if (startTime == TimeSpan.MinValue)
                startTime = gameTime.TotalGameTime;

            if (Step != -1)
            {
                ret = startValue + this.sens * Step / 1000f * (float)gameTime.TotalGameTime.Subtract(startTime).TotalMilliseconds;
            }
            else
            {
                ret = startValue + this.sens * (float)gameTime.TotalGameTime.Subtract(startTime).TotalMilliseconds / (float)this.Duration * Math.Abs(this.Max - this.Min);
            }

            if (ret >= Max && this.Reverse)
            {
                this.sens = -1;
                this.startValue = this.Max;
                startTime = gameTime.TotalGameTime;
            }
            else if (ret <= this.Min && this.Reverse)
            {
                this.sens = 1;
                this.startValue = this.Min;
                startTime = gameTime.TotalGameTime;
            }

            if (ret >= Max && this.Loop)
            {
                startTime = gameTime.TotalGameTime;

                ret = startValue;
            }
			else if (
				(ret >= Max && sens == 1) ||
				(ret <= Max && sens == -1) && !this.Loop
			)
			{
				ret = Max;
			}
			

            return ret;
        }

		public bool IsFinished(GameTime gameTime)
		{
			return Eval(gameTime) == Max;
		}
    }
}
