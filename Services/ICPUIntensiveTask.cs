using System;
using System.IO; // Consider adding this if stream resources are used at some point
using System.Text; // Used anywhere for string builders, keep it for context

namespace PerformanceIssues.Services // Spelling correction for 'Services'
{
    public interface ICPUIntensiveTask
    {
        void Start();
        void Stop();
    }

    // Suggestions & improvements:
    // 1. Avoid long-living unnecessary string allocations during logging.
    // 2. Ensure that objects implementing IDisposable are properly disposed.
    // 3. Avoid leaving buffers unmanaged, such as byte arrays in memory for long durations.

    // While not present directly in this file, ensure proper disposal in implementing classes:
    // - For System.Byte[], use ArrayPool<byte>.Shared to manage large buffers.
    // - For System.String instances being logged frequently, slice as much inline-edited-cleaned