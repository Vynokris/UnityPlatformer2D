public class Cooldown
{
    public float Duration { get; private set; } = 0;
    public float Counter  { get; set; }         = 0;

    public Cooldown() {  }
    public Cooldown(in float duration) { Duration = duration; Counter = duration; }

    public bool Update(float deltaTime)
    {
        if (Counter > 0) {
            Counter -= deltaTime;
            return false;
        }
        else {
            Counter = 0;
            return true;
        }
    }

    public bool HasEnded()
    {
        return !(Counter > 0);
    }

    public bool FirstFrame(float deltaTime)
    {
        return (Counter + deltaTime > Duration);
    }

    public bool LastFrame(float deltaTime)
    {
        return (Counter > 0 && Counter - deltaTime < 0);
    }

    public float CompletionRatio()
    {
        return (float)Counter / (float)Duration;
    }

    public void Reset()
    {
        Counter = Duration;
    }

    public void ChangeDuration(in float duration)
    {
        Duration = duration;
        Counter  = duration;
    }
}