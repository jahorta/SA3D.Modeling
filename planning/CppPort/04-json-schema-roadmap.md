# JSON Schema Roadmap for Canonical Per-Slice Contracts

Date: 2026-04-19
Status: Proposed

## Proposed schema family

Use a two-level validation model:

1. **Envelope schema (existing)**
   - `parity_report_v1` (Phase 0)
2. **Per-slice schemas (new)**
   - `sa3d_slice_1_primitives_v1`
   - `sa3d_slice_2_blockmap_meta_v1`
   - `sa3d_slice_3_nodecore_v1`
   - ...
   - `sa3d_slice_9_normalized_v1`

## Embedding pattern

Each `slice_io_pairs` entry should include:

- `slice` (integer)
- `slice_schema` (string id)
- `stage` (string, stable label used by diagnostics)
- `inputs` (validated against selected slice schema)
- `outputs` (validated against selected slice schema)

## Schema evolution rules

- Backward-compatible additions: increment minor in `$id` suffix only when optional fields are added.
- Breaking changes: new major schema id (e.g., `_v2`) and report-level reference to active schema map.
- Never reuse an id for different required fields.

## Determinism policy (must be documented in schema descriptions)

- Numeric precision policy for floats.
- Stable sort keys for arrays where order is semantically irrelevant.
- UTF-8 + LF output constraints.
- Nullability policy: absent vs explicit null must be meaningful and consistent.

## Minimum required common fields for every slice schema

- `run_id` (string)
- `fixture_id` (string)
- `slice` (integer)
- `operation_count` (integer)
- `diagnostic_count` (integer)

These common fields simplify generic tooling while allowing domain-specific per-slice payloads.

## Immediate next step

Draft concrete JSON Schema files under `planning/CppPort/schema-drafts/` for Slices 1, 2, and 3 first, then iterate outward.

