# SA3D.Modeling ↔ SoaSimMLD C++ Port Planning Overview

Date: 2026-04-19
Status: Draft (alignment pass)

## Purpose

This planning set defines what SA3D.Modeling must emit so SoaSimMLD can run parity checks slice-by-slice during the C++ SA3D port.

The intent is to keep:
- **one parity report envelope** per fixture run,
- **one canonical JSON schema per slice stage** for deterministic input/output capture,
- **shared fixture and naming contracts** with the Phase 0 assets in SOASim.

## Source alignment inputs

The following upstream documents are treated as the current source of truth for parity workflow shape:
- `phase0/PARITY_REPORT_SCHEMA.json`
- `phase0/FIXTURE_MANIFEST.json`
- `phase0/NAMING_AND_NAMESPACE_MAPPING.md`
- `planning/NavigationPhase/SA3DPort/*` slice sequencing docs

## Planning documents in this folder

1. `01-schema-alignment.md`
   - Evaluates existing Phase 0 schema definitions for reuse in SA3D.Modeling output.
2. `02-slice-stage-goals.md`
   - Defines goals and required outputs for Slices 1, 2, and 3+.
3. `03-implementation-phases.md`
   - Defines implementation phases, deliverables, and acceptance gates.
4. `04-json-schema-roadmap.md`
   - Proposes a concrete schema family (`slice_1...slice_9`) and versioning approach.

## Non-goals for this planning set

- No C# runtime implementation details (code design follows in implementation tasks).
- No C++ algorithm design details.
- No fixture generation changes beyond schema compatibility.

