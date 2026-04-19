# Implementation Phases and Acceptance Gates

Date: 2026-04-19
Status: Proposed

## Phase A — Schema + contract definition

### Deliverables

- Finalized per-slice JSON schema set (`slice_1` ... `slice_9`).
- Updated parity report contract usage notes.
- Fixture manifest extension proposal.

### Acceptance gate

- Sample artifacts from at least 2 fixtures validate successfully against envelope + slice schema.

---

## Phase B — Capture pipeline scaffolding in SA3D.Modeling

### Deliverables

- Capture context model (fixture/run/slice metadata).
- Deterministic serializer configuration.
- Optional capture enablement flags (default off).

### Acceptance gate

- Empty/no-op run generates valid report envelope with declared schema version fields.

---

## Phase C — Slice 1 + Slice 2 emission

### Deliverables

- Primitive/LUT/BAMS capture output for Slice 1.
- NJ block map + metadata shell output for Slice 2.

### Acceptance gate

- SoaSimMLD can ingest generated outputs for Slice 1 and 2 and produce deterministic comparison results across repeated runs.

---

## Phase D — Slice 3 to Slice 5 emission

### Deliverables

- Node graph + attach + polychunk/strip stage outputs.

### Acceptance gate

- At least one fixture from each category (`node-heavy`, `chunk-heavy`) compares with actionable mismatch localization.

---

## Phase E — Slice 6 to Slice 8 emission

### Deliverables

- ModelFile/AnimationFile NJ entry diagnostics.
- Motion/keyframe decode parity summaries.

### Acceptance gate

- Whole-file ingestion stages (model + animation) are comparable in C++ harness without manual postprocessing.

---

## Phase F — Slice 9 normalized outputs + CI gates

### Deliverables

- Buffer/weighted normalization summaries.
- Optional canonical signatures for quick triage.
- CI gate policies by stage.

### Acceptance gate

- CI can fail on stage-specific mismatch thresholds and provide machine-readable diagnostics.

