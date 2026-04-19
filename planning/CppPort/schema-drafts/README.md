# CppPort Per-Slice Schema Drafts (Phase A)

These schema drafts define **one canonical schema per slice stage** for parity `slice_io_pairs` entries.

## Scope

- Validates a single `slice_io_pairs[]` item (not the full report envelope).
- Intended to be embedded inside `parity_report_v1` payloads.
- Uses JSON Schema Draft 2020-12.

## Files

- `common-defs.schema.json` shared `$defs` and cross-slice fields.
- `slice-1-primitives.schema.json`
- `slice-2-blockmap-meta.schema.json`
- `slice-3-nodecore.schema.json`
- `slice-4-attach-chunk.schema.json`
- `slice-5-polychunk-strip.schema.json`
- `slice-6-modelfile-entry.schema.json`
- `slice-7-motion-keyframes.schema.json`
- `slice-8-animationfile-entry.schema.json`
- `slice-9-normalized.schema.json`

## Integration pattern (report envelope)

Each `slice_io_pairs[]` entry should include:

- `slice`: fixed integer for the slice
- `slice_schema`: schema id string (e.g. `sa3d_slice_4_attach_chunk_v1`)
- `stage`: stable stage label
- `run_id`, `fixture_id`, `operation_count`, `diagnostic_count`
- `inputs` and `outputs` objects validated by the selected slice schema

## Notes

- Draft status: **v1 proposal** for planning and early implementation.
- These schemas are strict (`additionalProperties: false`) to catch drift early.
