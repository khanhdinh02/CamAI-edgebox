namespace CamAI.EdgeBox.Services.AI.Uniform;

public class UniformProcessor : IDisposable
{
    private bool disposed;

    public UniformProcessor(ClassifierWatcher watcher)
    {
        watcher.Notifier += ReceiveData;
    }

    // TODO: within last 300s, if there is less than 100 detected uniform than it is incident

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        // TODO: filter and add data
        // TODO: what if output count is still 0 after 10 seconds
        // if (output.Count > 0)
        //     classifierOutputs.Add(output);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool _)
    {
        if (disposed)
            return;

        disposed = true;
    }
}
