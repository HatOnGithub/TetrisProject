using System;
using Microsoft.Xna.Framework;


public class TimeHelper
{
	int referenceTime;
	int currentTime;
	bool timerSet;
	public bool paused;

	/// <summary>
	/// Keeps track of Time
	/// </summary>
	public TimeHelper()
	{
		referenceTime = 0;
		TimerReset();
	}

	/// <summary>
	/// Updates Time
	/// </summary>
	/// <param name="gameTime"></param>
	public void Update(GameTime gameTime)
	{
		currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
	}

	/// <summary>
	/// Sets the current time as reference
	/// </summary>
	public void TimerReset()
	{
		referenceTime = currentTime;
		timerSet = true;
	}

	/// <summary>
	/// Returns true if Timer has reached the Delay 
	/// (currentTime - referenceTime) or Delta T in physics
	/// </summary>
	/// <param name="delayInMilliseconds"></param>
	/// <returns></returns>
	public bool TimerHasReached(int delayInMilliseconds)
	{
		if (paused) return false;
		if (timerSet && currentTime >= referenceTime + delayInMilliseconds)
			return true;
		return false;
	}

	public bool TimerHasReachedFrames(int frames)
	{
        if (paused) return false;

        // 1 frame is 16.7 milliseconds at 60 frames per second
        float Milliseconds = frames * 16.7f;

		if (timerSet && currentTime >= referenceTime + Milliseconds) 
            return true;
		return false;
	}

	public void Pause()
	{
		if (paused) TimerReset();
		paused = !paused;
	}
}
