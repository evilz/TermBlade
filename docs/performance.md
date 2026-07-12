# TermBlade performance guide

Terminal applications are usually limited by terminal I/O, but the per-cell diff loop and text editing can still dominate CPU and allocation profiles.

## Rendering checklist

- Render into a reusable frame representation.
- Skip unchanged cells before formatting ANSI sequences.
- Append directly to a reusable `StringBuilder`; do not create a `StringWriter` per cell.
- Keep color and attribute state so repeated ANSI sequences are omitted.
- Flush once per frame, not once per cell.
- Preserve exact reset ordering when attributes change.

The ANSI append helpers exist for this hot path. The `TextWriter` helpers remain available for callers that already own a writer.

## Text and collections

Use the rope-backed `EditBuffer` for repeated edits. Avoid converting the complete buffer to a string inside cursor loops. In renderables, LINQ is acceptable for setup and small collections, but prefer explicit loops in measured per-frame work. Do not replace dictionaries with `FrozenDictionary` unless construction is complete before lookup and profiling supports the trade-off.

## Measurement

Correctness comes first. For a performance change, compare a representative frame with no changes, a sparse update, and a full update. Record allocations and elapsed time with a benchmark or profiler. A local allocation reduction is not evidence of an end-to-end speedup until terminal I/O and layout are included.
