# Phase 0 Schema Alignment Assessment

Date: 2026-04-19
Status: Proposed

## Question answered

"Can existing `phase0` JSON definitions serve as the canonical per-slice schema foundation?"

Short answer: **Yes, partially.**

- `PARITY_REPORT_SCHEMA.json` works well as the **top-level envelope** for each fixture run.
- `FIXTURE_MANIFEST.json` works well as the **fixture source contract**.
- We still need **new per-slice schemas** to make `slice_io_pairs` deterministic and strongly validated.

## Assessment by artifact

## 1) `PARITY_REPORT_SCHEMA.json`

### What is immediately reusable

- Stable top-level identity (`schema`, `fixture`, `reference`, `metrics`, `diagnostics`, `comparison`).
- Existing `slice_io_pairs` array shape gives us a direct integration point.
- Works as cross-language transport and archival format.

### Gaps for canonical slice-level contracts

The current `slice_io_pairs[*].inputs` and `outputs` are generic objects. That is too permissive for parity triage and CI gating because:
- missing required fields won’t be caught,
- type drift can happen silently,
- ordering/normalization constraints are not explicit.

### Decision

Keep `parity_report_v1` as the report envelope, but add per-slice schemas and validate each `slice_io_pairs` item against a schema selected by `slice` + `stage`.

## 2) `FIXTURE_MANIFEST.json`

### What is reusable

- Manifest-level schema and generation timestamp.
- Fixture policy (`auto_discover` + glob) aligns with parity harness ingestion.

### Needed extension

Add optional fixture metadata fields required by slice-specific expectations, for example:
- known model/motion offsets (when precomputed),
- fixture tags (e.g., `node_core`, `chunk_heavy`, `motion_edge_cases`),
- expected slice coverage hints.

These can be additive without breaking existing consumers.

## 3) `NAMING_AND_NAMESPACE_MAPPING.md`

### Relevance to schema planning

Not a JSON schema, but critical for traceability fields in diagnostics. We should emit names/paths that map directly back to C# source types and planned C++ locations.

## Compatibility strategy

1. Keep `parity_report_v1` unchanged as a transport envelope.
2. Add `slice_schema` metadata to each `slice_io_pairs` entry, e.g.:
   - `"slice_schema": "sa3d_slice_1_primitives_v1"`
3. Validate `inputs/outputs` against that schema.
4. Enforce deterministic JSON policy (stable key order, normalized float formatting, UTF-8, LF).

## Success criteria

- Existing Phase 0 report readers continue to parse reports.
- Slice-specific validators can fail fast when required parity fields are absent.
- CI can gate by slice stage, not only by whole-report pass/fail.

