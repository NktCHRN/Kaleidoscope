namespace WebApi.IntegrationTests.Stubs;

public class TestTimeProvider : TimeProvider
{
    private DateTimeOffset? _presetTime;

    public override DateTimeOffset GetUtcNow()
    {
        return _presetTime ?? base.GetUtcNow();
    }

    public void ReturnRealTime()
    {
        _presetTime = null;
    }

    public void ReturnPresetTime(DateTimeOffset presetTime)
    {
        _presetTime = presetTime;
    }

    public void Reset()
    {
        ReturnRealTime();
    }
}
