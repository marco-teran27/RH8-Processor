using Commons.Interfaces;
using Config.Interfaces;
using Interfaces;

public class TheOrchestrator : ITheOrchestrator
{
    private readonly IConfigSelUI _selector;
    private readonly IConfigParser _parser;
    private readonly IRhinoCommOut _rhinoCommOut;
    private readonly IFileDirScanner _fileDirScanner;
    private readonly IBatchService _batchService;

    public TheOrchestrator(
        IConfigSelUI selector,
        IConfigParser parser,
        IRhinoCommOut rhinoCommOut,
        IFileDirScanner fileDirScanner,
        IBatchService batchService)
    {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        _fileDirScanner = fileDirScanner ?? throw new ArgumentNullException(nameof(fileDirScanner));
        _batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
    }

    public async Task<bool> RunBatchAsync(string? configPath, CancellationToken ct)
    {
        // ... existing code up to pre-parsing ...
        await _fileDirScanner.ScanAsync(ct);
        await _batchService.RunBatchAsync(ct);
        return true;
    }
}