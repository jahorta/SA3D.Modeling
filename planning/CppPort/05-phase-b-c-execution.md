# Phase B/C Execution Notes

Date: 2026-04-19
Status: Implemented (initial)

## Implemented in this iteration

### Phase B

- Added capture options with default capture-disabled behavior (`ParityCaptureOptions`).
- Added deterministic report model + JSON serialization path (`ParityReport`, `SliceIOPair`, `ParityReportGenerator.WriteToFile`).
- Added no-op behavior for disabled capture to satisfy the "empty/no-op" acceptance shape.

### Phase C

- Added Slice 1 emission (`sa3d_slice_1_primitives_v1`):
  - primitive read checkpoints,
  - BAMS conversion checkpoints,
  - deterministic output hashes.
- Added Slice 2 emission (`sa3d_slice_2_blockmap_meta_v1`):
  - NJ block map extraction,
  - role classification (model/texture/animation/none),
  - deterministic output hash.

## Additional utilities

- Added public `NJDebugInfo` helper for NJ block inspection.

## Current limitations (intended follow-up)

- Slice 2 metadata shell currently emits an empty `meta_blocks` array.
- Slice 1 LUT operations summary is currently zeroed (no internal LUT event stream yet).
- Full fixture-driven acceptance test harness and CI integration are pending.


## Direction update (2026-04-21)

To support upcoming slices uniformly:

- Parity capture now targets instrumentation of normal parser execution instead of synthetic slice generation.
- A shared capture session concept is used to gate requested slices and cap pair counts per slice.
- `ParityReportGenerator` remains the entrypoint and now owns session lifecycle + adapter invocation + report projection.
- Slice 1/2 synthetic stubs are replaced by data assembled from captured parser events.
- This scaffolding is intended for extension to slices 3..9 using the same hook-and-projection model.

